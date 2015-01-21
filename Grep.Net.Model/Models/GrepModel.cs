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

        public async Task<GrepContext> Grep(GrepContext grepContext)
        {
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
            
            
            GrepResult result = new GrepResult();
            
         
            result.GrepContext = grepContext;
            grepContext.TimeStarted = DateTime.Now;

            ConcurrentBag<MatchInfo> tmpResults = new ConcurrentBag<MatchInfo>();
           
            //First we need to build out the FileInfo for this grep. We use await here because we want to wait for all the dirs to complete, but want each directory search to run indepdendently. 
            await Task.Factory.StartNew((param) =>
            {
                string tmp = param as String;

                SearchOption so = Settings.Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

                //var fse = new FileSystemEnumerable(new DirectoryInfo(tmp), "*", so);

                //var dirs = fse.OfType<DirectoryInfo>().Select(x=>x.FullName).ToList();

                var dirs = new DirectoryInfo(tmp).EnumerateDirectories("*", so).Select(x => x.FullName).ToList();

                if (!dirs.Contains(tmp))
                    dirs.Add(tmp);

                List<Task> tasks = new List<Task>();

                foreach (string dir in dirs)
                {
                    var files = grepContext.FileExtensions.ToList().SelectMany(x => Directory.GetFiles(dir, "*" + x.Extension, SearchOption.TopDirectoryOnly));
                    if (files.Count() > 0)
                    {
                        tasks.Add(Task.Factory.StartNew(() =>
                        {
                            if (grepContext.OnDirectory != null)
                            {
                                grepContext.OnDirectory(grepContext, dir);
                            }

                            List<MatchInfo> matchInfos = ExecuteInternal(files.ToArray(),
                                grepContext.PatternPackages.ToList().SelectMany(x => x.Patterns).ToArray(),
                                grepContext.FileExtensions.Select(x => "*" + x.Extension).ToArray());
                            //Set the GrepResult
                            matchInfos.ForEach(x => x.GrepResultId = result.Id);

                            //add to the thread safe collection.
                            matchInfos.ForEach(x => tmpResults.Add(x));
                        }, CancellationToken.None, TaskCreationOptions.None, Scheduler));
                    }
                }
                if (tasks.Count() > 0)
                {
                    var tmpTask = Task.Factory.ContinueWhenAll(tasks.ToArray(), (x) =>
                    {
                        tmpResults.ForEach(y => result.MatchInfos.Add(y));
                        grepContext.TimeCompleted = DateTime.Now;
                        //Add back to the non thread safe collection. 
                        grepContext.Completed = true;
                        if (grepContext.OnCompleted != null)
                        {
                            grepContext.OnCompleted(grepContext, result);
                        }

                       
                     
                    }, CancellationToken.None);
                }
                else
                {
                    grepContext.ErrorType = SearchErrorType.NoFiles;
                    logger.Info("No directories to scan. hrm?");
                    grepContext.TimeCompleted = DateTime.Now;
                    //Add back to the non thread safe collection. 
                    grepContext.Completed = true;
                    if (grepContext.OnCompleted != null)
                    {
                        grepContext.OnCompleted(grepContext, result);
                    }
            
                }
               
            }, grepContext.RootPath);

            return grepContext;
        }
    }
}