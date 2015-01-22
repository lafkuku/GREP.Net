using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Grep.Net.Entities;
using Grep.Net.Model.Extensions;
using Grep.Net.Model.Types;

namespace Grep.Net.Model.Models
{
    public class GrepModel
    {
        public static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger(); 
        
        public static int SearchID = 1; 

        public DataModel DataModel { get; set; }

        internal ActionScheduler Scheduler { get; set; }

        public RunspacePool RunspacePool { get; set; }

        public Properties.Settings Settings
        {
            get
            {
                return Properties.Settings.Default;
            }
        }

        public GrepModel()
        {
            Scheduler = new ActionScheduler(Settings.GrepThreadsMax);

            //TODO: Add a Setting to control these values??
            this.RunspacePool = RunspaceFactory.CreateRunspacePool(5, 15);
            this.RunspacePool.Open();
        }

        public delegate void NextDirectoryEvent(String dir, object sender);

        public NextDirectoryEvent OnNextDirectory { get; set; }

        public delegate void CompletedEvent(GrepContext gc, GrepResult result);

        public CompletedEvent OnCompleted { get; set; }

        private List<MatchInfo> ExecuteInternal(String[] filesToGrep, Pattern[] patterns, String[] fileExtensions)
        {
            List<MatchInfo> matchInfos = new List<MatchInfo>(); 
            try
            {
                PowerShell ps = PowerShell.Create();
                ps.RunspacePool = this.RunspacePool;

                Command selectString = new Command("select-string");
                selectString.Parameters.Add("Path", filesToGrep);
                selectString.Parameters.Add("Pattern", patterns.Select(x => x.PatternStr).ToArray());
                selectString.Parameters.Add("Context", new int[] { Settings.LinesBefore, Settings.LinesAfter });

                if (this.Settings.Exclusions.Count > 0)
                    selectString.Parameters.Add("Exclude", Settings.Exclusions.Cast<string>().ToArray());

                selectString.Parameters.Add("Include", fileExtensions);

                ps.Commands.AddCommand(selectString);

                // execute the script
                var results = ps.Invoke();
                foreach (PSObject obj in results)
                {
                    //So the BaseObject is of type MatchInfo defined in the Microsoft.Powershell.Commands.Utilities... Short cutting referencing it and that jazz.
                    dynamic msMatchInfo = obj.BaseObject;

                    StringBuilder contextStr = new StringBuilder();

                    foreach (String line in msMatchInfo.Context.PreContext)
                    {
                        contextStr.AppendLine(line);
                    }
                    contextStr.AppendLine(msMatchInfo.Line);
                    foreach (String line in msMatchInfo.Context.PostContext)
                    {
                        contextStr.AppendLine(line);
                    }

                    string matchStr = "";
                    try
                    {
                        matchStr =  msMatchInfo.Matches[0].Value;
                    }catch{


                    }

                    MatchInfo mi = new MatchInfo()
                    {
                        FileInfo = new Entities.FileInfo() { FullName = msMatchInfo.Path },
                        Line = msMatchInfo.Line.Trim(),
                        LineNumber = msMatchInfo.LineNumber,
                        Context = contextStr.ToString(),
                        Match = matchStr
                    };

                    //Normalize the context and line to a reasonable length. 
                    if (mi.Line.Length > Settings.MaxLineSize)
                        mi.Line = mi.Line.Substring(0, Settings.MaxLineSize);

                    if (mi.Context.Length > Settings.MaxContextSize)
                        mi.Context = mi.Context.Substring(0, Settings.MaxContextSize);

                    string patternStr = msMatchInfo.Pattern;

                    Pattern p = patterns.FirstOrDefault((x) => x.PatternStr.Equals(patternStr));

                    if (p == null)
                    {
                        //TODO: Log? Error?
                        throw new ArgumentException("Null Pattern?!?! You should not see this");
                    }

                    mi.Pattern = p;

                    matchInfos.Add(mi);
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            return matchInfos;
        }

        private List<String> GetDirectories(GrepContext gc)
        {
            SearchOption so = Settings.Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            List<String> dirs = new List<string>();
            try
            {
                
                DirectoryInfo rootDi = new DirectoryInfo(gc.RootPath);

                foreach (DirectoryInfo dir in rootDi.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        dirs.Add(dir.FullName);

                        if (Settings.Recurse)
                        {
                            foreach (DirectoryInfo subDir in dir.EnumerateDirectories("*", SearchOption.AllDirectories))
                            {
                                try
                                {
                                    dirs.Add(subDir.FullName);
                                }
                                catch (UnauthorizedAccessException uae)
                                {
                                    logger.Error(uae);
                                    if (gc.OnError != null)
                                    {
                                        gc.OnError(gc, uae.Message);
                                    }
                                }
                            }
                        }
                    }
                    catch (UnauthorizedAccessException uae)
                    {
                        logger.Error(uae);
                        if (gc.OnError != null)
                        {
                            gc.OnError(gc, uae.Message);
                        }
                    }
                }
            }catch (Exception e)
            {
                if (gc.OnError != null)
                {
                    gc.OnError(gc, e.Message);
                }
            }
            if (!dirs.Contains(gc.RootPath))
                dirs.Add(gc.RootPath);

            return dirs;
        }

        private List<String> GetFiles(GrepContext gc, string directory, HashSet<String> extensions = null)
        {
         
            List<String> files = new List<string>();

            try
            {
                DirectoryInfo di = new DirectoryInfo(directory);

                if (extensions == null)
                {
                    extensions = new HashSet<string>(gc.FileExtensions.Select(x => x.Extension).ToList());
                }

                foreach (var fi in di.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        if (extensions.Contains(fi.Extension))
                        {
                            files.Add(fi.FullName);
                        }
                    }
                    catch (UnauthorizedAccessException uae)
                    {
                        logger.Error(uae);
                        if (gc.OnError != null)
                        {
                            gc.OnError(gc, uae.Message);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                if (gc.OnError != null)
                {
                    logger.Error(e);
                    gc.OnError(gc, e.Message);
                }
            }
            return files;
        }
        public async Task<GrepResult> Grep(GrepContext grepContext)
        {

            if (grepContext == null)
            {
                throw new ArgumentException("Invalid GrepContext");
            }
            if (String.IsNullOrWhiteSpace(grepContext.RootPath))
            {
                throw new ArgumentException("Null or empty rootDir");
            }

            if (grepContext.PatternPackages == null || grepContext.PatternPackages.Count < 1)
            {
                throw new ArgumentException("No patterns selected to grep for.");
            }

            if (grepContext.FileExtensions == null)
            {
                throw new ArgumentException("Extensions paramter is null");
            }
            
            
           
            grepContext.TimeStarted = DateTime.Now;

            ConcurrentBag<MatchInfo> tmpResults = new ConcurrentBag<MatchInfo>();
            GrepResult result = new GrepResult();
            result.GrepContext = grepContext;
            //First we need to build out the FileInfo for this grep. We use await here because we want to wait for all the dirs to complete, but want each directory search to run indepdendently. 
            await Task.Factory.StartNew(async (pObj) =>
            {
                GrepContext _gc = pObj as GrepContext;
                List<Task> tasks = new List<Task>();

                if (_gc == null)
                {
                    //PRoblem. 
                    logger.Error("Failed to get the correct GrepContext from the task factory. the passed in param is null????");
                   
                    return;
                }
                try{

                    List<String> dirs = GetDirectories(_gc);
  
                    //Create a lookup table, this will be faster than enumerating the two sets against each other. 
                    HashSet<String> extensions = new HashSet<string>(_gc.FileExtensions.Select(x => x.Extension).ToList());

                    foreach (string dir in dirs)
                    {
                        var files = GetFiles(_gc, dir, extensions);
                        if (files.Count() > 0)
                        {
                            tasks.Add(Task.Factory.StartNew(() =>
                            {
                                try{
                                if (_gc.OnDirectory != null)
                                {
                                    _gc.OnDirectory(grepContext, dir);
                                }

                                List<MatchInfo> matchInfos = ExecuteInternal(files.ToArray(),
                                                                            _gc.PatternPackages.ToList().SelectMany(x => x.Patterns).ToArray(),
                                                                            _gc.FileExtensions.Select(x => "*" + x.Extension).ToArray());
                           
                                 //Set the GrepResult
                                //add to the thread safe collection.
                                matchInfos.ForEach(x =>{
                                    x.GrepResultId = result.Id;
                                    tmpResults.Add(x);
                                });
                                }
                                catch (Exception e)
                                {
                                    _gc.Status = e.Message;
                                    logger.Error(e);
                                    if (_gc.OnError != null)
                                    {
                                        _gc.OnError(_gc, e.Message);
                                    }
                                }
                            }, CancellationToken.None, TaskCreationOptions.None, Scheduler));
                        }
                    }
                    if (tasks.Count() > 0)
                    {
                        await Task.Factory.ContinueWhenAll(tasks.ToArray(), (x) =>
                        {
                            tmpResults.ForEach(y => result.MatchInfos.Add(y));
                        }, CancellationToken.None);
                    }
                    else
                    {
                        string msg = "No directories to scan?.";
                        _gc.Status = msg;
                        logger.Info(msg);
                    }
                }
                catch (Exception e)
                {
                    _gc.Status = e.Message;
                    if (_gc.OnError != null)
                    {
                        _gc.OnError(_gc, e.Message);
                    }
                }

                 _gc.TimeCompleted = DateTime.Now;
                //Add back to the non thread safe collection. 
                _gc.Completed = true;
                if (_gc.OnCompleted != null)
                {
                    _gc.OnCompleted(grepContext, result);
                }
            
            }, grepContext);

            return result;
        }
    }
}