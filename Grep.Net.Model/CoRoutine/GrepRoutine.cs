using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.IO;
using System.Windows;
using GTServices.Entities;
using GTServices.Model.Extensions;
using System.Text.RegularExpressions;


namespace GTServices.Model.CoRoutine
{
    /// <summary>
    /// This class actually does the "Grepping" 
    /// </summary>
    public class GrepRoutine : IResult
    {
    
        //Events
        public event EventHandler<ResultCompletionEventArgs> Completed;

        public IList<MatchInfo> Matches { get; set; }
        public IList<Pattern> Patterns { get; set; }
        public IList<Entities.FileInfo> Files { get; set; } 

        //Settings 
        int LinesBefore { get; set; }
        int LinesAfter  { get; set; }



        public GrepRoutine()
        {
            Files = filestoScan;
            Patterns = patterns;
            Matches = new List<MatchInfo>();
            LinesAfter = linesAfter;
            LinesBefore = linesBefore; 
        }



        /// <summary>
        /// TODO: Add access to the context parameter. This will be useful for tooltip displaying contextual information to prevent having to "open" a full blown window to evaluate a result. 
        /// TODO: Expose SimpleMatch for non-grep matching. 
        /// </summary>
        /// <param name="context"></param>
        public void Execute(ActionExecutionContext context)
        {
            try
            {
                List<String> rawPatterns = Patterns.Select(x => x.PatternStr).ToList(); 

                pipeline = runspace.CreatePipeline();
                Command selectString = new Command("select-string");
                selectString.Parameters.Add("Path", Files.Select((x) => x.FullName).Distinct().ToArray());
                selectString.Parameters.Add("Pattern", rawPatterns.ToArray());

                selectString.Parameters.Add("Context", new int[] { LinesBefore, LinesAfter }); 

                pipeline.Commands.Add(selectString);

                // execute the script
                System.Collections.ObjectModel.Collection<PSObject> results = pipeline.Invoke();
                    
                // convert the script result into a single object

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

                    MatchInfo mi = new MatchInfo()
                    {
                        FileInfo = Files.FirstOrDefault((x) => x.FullName == msMatchInfo.Path),
                        Line = msMatchInfo.Line.Trim(),
                        LineNumber = msMatchInfo.LineNumber,
                        Context = contextStr.ToString()
                    };

                    //Normalize the context and line to a reasonable length. 
                    if (mi.Line.Length > 1000)
                        mi.Line = mi.Line.Substring(0, 1000);

                    if (mi.Context.Length > 1000)
                        mi.Context = mi.Context.Substring(0, 1000); 


                    string patternStr = msMatchInfo.Pattern;  

                    Pattern p = Patterns.FirstOrDefault((x) => x.PatternStr.Equals(patternStr));

                    if (p == null)
                    {
                        //TODO: Log? Error?
                        throw new ArgumentException("Null Pattern?!?! You should not see this"); 
                    }

                    mi.Pattern = p;

                    Matches.Add(mi); 
                }
                
            }
            catch (Exception e)
            {
                //TODO: Log
                System.Xml.XmlTextReader xtr = new System.Xml.XmlTextReader(new MemoryStream());
                completionArgs.Error = e;
            }
            finally
            {
                if (runspace != null)
                    runspace.Dispose();
                if (pipeline != null)
                    pipeline.Dispose();

                if (Completed != null)
                {
                    Completed(this, completionArgs);
                }
            }
        }
    }
}
