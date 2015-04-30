using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Grep.Net.Entities;
using Grep.Net.Model;
using Grep.Net.Model.Models;
using Grep.Net.Data;
using System.Threading;
using System.Collections.Concurrent;


namespace Grep.Net.Powershell
{
    [Cmdlet("Invoke","Grep")]
    public class Grep : PSCmdlet
    {
        [Parameter(Position=0, Mandatory=true)]
       
        public String RootDirectory { get; set; }

        [Parameter(Mandatory = false, ParameterSetName="ByCategory")]
        [Alias("c")]
        public String[] Categories { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "ByPackages")]
        [Alias( "p")]
        public String[] Packages { get; set; }

        [Alias("ft")]
        [Parameter(Position=2, Mandatory=false)]
        public String[] FileTypes { get; set; }

        [Alias("o")]
        [Parameter(Position=3, Mandatory=false)]
        public String OutFile { get; set; }
        
        [Alias("r")]
        [Parameter(Mandatory=false)]
        public SwitchParameter Recurse { get; set;}


        [Alias("n")]
        [Parameter(Mandatory = false)]
        public SwitchParameter Noisy { get; set; }


        private GTApplication App { get; set; }

        private ConcurrentQueue<ProgressRecord> ProgressQueue { get; set; }

        private ConcurrentQueue<ErrorRecord> ErrorQueue { get; set; }

        public Grep(){ 
            Recurse = true;
            Noisy = false;
            ProgressQueue = new ConcurrentQueue<ProgressRecord>();
            ErrorQueue = new ConcurrentQueue<ErrorRecord>();
            OutFile = "";
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            try
            {
                WriteProgress(new ProgressRecord(1, "Loading...",  "Initilizing the Application Model"));
                App = GTApplication.Instance;
            }
            catch (Exception e)
            {
                WriteObject(e.Message);
            }
        }
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (String.IsNullOrEmpty(RootDirectory))
            {
                WriteObject("Invalid RootDirectory. The value is null or empty");
                return;
            }
            if (!System.IO.Directory.Exists(RootDirectory))
            {
                WriteObject("The supplied RootDirectory does not exist");
                return;
            }

            List<PatternPackage> packages = null;
          
            switch (ParameterSetName)
            {
                case "ByCategory":
                     if(Categories.Count() < 1){
                         throw new ArgumentException("Categories selected, but no categories were passed in.");
                     }
                     else
                     {
                         HashSet<String> categoryNames = new HashSet<string>(Categories.ToList());
                         packages = App.DataModel.PatternPackageRepository.GetAll().Where(x => categoryNames.Contains(x.Category)).ToList();
                     }
                     break;

                case "ByPackages":
                     if(Packages.Count() < 1){
                         throw new ArgumentException("Packages selected, but no packages were actually passed in.");
                     }
                     else
                     {
                         HashSet<String> packageNames = new HashSet<string>(Packages.ToList());
                         packages = App.DataModel.PatternPackageRepository.GetAll().Where(x => packageNames.Contains(x.Name)).ToList();
                     }
                     break;

                default:
                     throw new ArgumentException("Bad ParameterSet, must include Categories or PatternPackages.");
            } // switch (ParameterSetName...

            if (packages.Count < 1)
            {
                throw new ArgumentException("Failed to find packages or categories with the names passed in.");
            }
           
            HashSet<String> fileTypeNames = new HashSet<string>(FileTypes.ToList());
            List<FileTypeDefinition> fileTypes = App.DataModel.FileTypeDefinitionRepository.GetAll().Where(x => fileTypeNames.Contains(x.Name)).ToList();
           

            if (fileTypes.Count < 1)
            {
                var ftd = App.DataModel.FileTypeDefinitionRepository.GetAll().FirstOrDefault(x => x.Name.Equals("all"));
                if (ftd == null)
                {
                    WriteObject("Failed to resolve all file extensions group. Manually creating and adding a package for this instance only....");
                    FileTypeDefinition newDef = new FileTypeDefinition();
                    newDef.Name = "All";
                    newDef.FileExtensions.Add(new FileExtension() { Extension = "*.*" });
                }
            }
            
            var extensions = fileTypes.SelectMany(x => x.FileExtensions);

            if (extensions == null || extensions.Count() < 1)
            {
                WriteObject("The FileTypeDefinitions with the Name: " + FileTypes + " does not contain any file extensions?");
                return;
            }

            WriteObject("Starting to Grep Directory: " + RootDirectory);
            WriteObject("Recurse?: " + Recurse);
            WriteObject("Packages: ");
            foreach (PatternPackage package in packages)
            {
                WriteObject("\t" + package.Name);
            }
            foreach (FileTypeDefinition ftd in fileTypes)
            {
                WriteObject("FileTypeDefinition: " + ftd.Name);
                foreach (FileExtension extension in ftd.FileExtensions)
                {
                    WriteObject("\t" + extension.Extension);
                }

            }
           

            GrepContext gc = new GrepContext()
            {
                RootPath = RootDirectory,
                PatternPackages = packages,
                FileExtensions = extensions.ToList(),
                TimeStarted = DateTime.Now
            };

            bool done = false;

            gc.OnCompleted += new GrepContext.CompletedAction((x, y) =>
            {
                done = true;
            });

            gc.OnDirectory += new GrepContext.NextDirectory((x, y) =>
            {
                ProgressRecord pr = new ProgressRecord(1, "Working...", y);
                ProgressQueue.Enqueue(pr);      
            });

            gc.OnError += new GrepContext.Error((x, y) =>
            {
                ErrorRecord er = new ErrorRecord(new Exception(y), "0", ErrorCategory.NotSpecified, null);
                ErrorQueue.Enqueue(er);
            });

            WriteObject("Starting..");
            Task<GrepResult> task = null;
            try
            {
                task = App.GrepService.Grep(gc);
            }
            catch (Exception e)
            {
                done = true;
            }
            WriteObject("Working..");
            while ( !done)
            {
                ProgressRecord pr = null;
                if(ProgressQueue.TryDequeue(out pr)){
                    WriteProgress(pr);
                }

                ErrorRecord er = null;
                if (ErrorQueue.TryDequeue(out er))
                {
                    WriteError(er);
                }

                Thread.Sleep(100);
            }

            //WriteObject("Completed!");

            //Check Queue for Progress. 
            GrepResult gr = task.Result;

            if (!string.IsNullOrEmpty(OutFile))
            {
                if (!System.IO.Path.IsPathRooted(OutFile))
                    OutFile = SessionState.Path.CurrentFileSystemLocation.Path + "\\" + OutFile;
                EntityContainer ec = new EntityContainer();
                ec.Add(gr);
                SerializationHelper.SerializeIntoXmlFile<EntityContainer>(OutFile,  ec);
            }
            
            WriteObject("Done!...");

            if (gr.MatchInfos.Count > 0)
            {
                if (Noisy)
                {
                    WriteObject(gr.MatchInfos);
                }
                else
                {
                    WriteObject("Output surpressed, -Noisy true to see results or -OutFile to write the results to an xml file for import to the GUI.");
                }
            }
            else
            {
                WriteObject("No Matches =("); 
            }

        }
    }

    [Cmdlet("Get", "GrepPackages")]
    public class GrepPatterns : PSCmdlet
    {

        [Parameter(Position = 0, Mandatory = false)]
        public String Filter { get; set; }


        [Parameter( Mandatory = false)]
        [Alias( "c")]
        public SwitchParameter Category { get; set; }


        public GrepPatterns()
        {
            Filter = "";
            Category = false;
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            try
            {
               var app =  GTApplication.Instance;
            }
            catch (Exception e)
            {
                WriteObject(e.Message);
            }


        }
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            List<PatternPackage> packages = null;

            if (!this.Category)
            {
                packages = GTApplication.Instance.DataModel.PatternPackageRepository.GetAll().Where(x => x.Name.Contains(Filter)).ToList();
            }
            else
            {
                packages = GTApplication.Instance.DataModel.PatternPackageRepository.GetAll().Where(x => x.Category.Contains(Filter)).ToList();
            }

            WriteObject(packages);
        }
    }

    [Cmdlet("Get", "FileTypes")]
    public class GrepFileTypes : PSCmdlet
    {

        [Parameter(Position = 0, Mandatory = false)]
        public String Filter { get; set; }


        public GrepFileTypes()
        {
            Filter = "";
         
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            try
            {
                var app = GTApplication.Instance;
            }
            catch (Exception e)
            {
                WriteObject(e.Message);
            }


        }
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            List<FileTypeDefinition> fileTypes = null;


            fileTypes = GTApplication.Instance.DataModel.FileTypeDefinitionRepository.GetAll().Where(x => x.Name.Contains(Filter)).ToList();
           

            WriteObject(fileTypes);
        }
    }
}
