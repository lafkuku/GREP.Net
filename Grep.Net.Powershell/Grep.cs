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

        
        [Parameter(Position = 1, Mandatory = true)]
        [Alias("Category", "p", "c")]
        public String Package { get; set; }

        [Alias("FileType", "ft")]
        [Parameter(Position=2, Mandatory=false)]
        public String FileTypes { get; set; }

        [Alias("o", "out")]
        [Parameter(Position=3, Mandatory=false)]
        public String OutFile { get; set; }
        
        [Alias("r")]
        [Parameter(Mandatory=false)]
        public SwitchParameter Recurse { get; set;}

        private GTApplication App { get; set; }

        private ConcurrentQueue<ProgressRecord> ProgressQueue { get; set; }

        private ConcurrentQueue<ErrorRecord> ErrorQueue { get; set; }

        public Grep(){ 
            Recurse = true;
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
            if (String.IsNullOrEmpty(Package))
            {
                WriteObject("Package parameter is null or empty. ");
                return;
            }


            List<PatternPackage> packages = new List<PatternPackage>(); 

            PatternPackage pp = App.DataModel.PatternPackageRepository.GetAll().FirstOrDefault(x => x.Name.Equals(Package, StringComparison.InvariantCultureIgnoreCase));
            if (pp != null)
            {
                packages.Add(pp);
            }
            else
            {
                var catPackages = App.DataModel.PatternPackageRepository.GetAll().Where(x => x.Category.Equals(this.Package, StringComparison.InvariantCultureIgnoreCase));
                packages.AddRange(catPackages);
            }

            if (packages.Count < 1)
            {
                WriteObject("Error: No Packages selected!");
                return;
            }

            List<FileTypeDefinition> fileTypes = new List<FileTypeDefinition>();

            if (String.IsNullOrEmpty(FileTypes))
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
            else {
                var ftds = App.DataModel.FileTypeDefinitionRepository.GetAll().Where(x => x.Name.Equals(FileTypes, StringComparison.InvariantCultureIgnoreCase));
                if (ftds == null || ftds.Count() < 1)
                {
                    WriteObject("Failed to resolve any FileTypeDefinitions with the Name: " + FileTypes);
                    return;
                }

                fileTypes.AddRange(ftds);
            }
            var extensions = fileTypes.SelectMany(x => x.FileExtensions);

            if (extensions == null || extensions.Count() < 1)
            {
                WriteObject("The FileTypeDefinitions with the Name: " + FileTypes + " does not contain any file extensions?");
                return;
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

            //WriteObject("Starting Grep");
            Task<GrepResult> task = null;
            try
            {
                task = App.GrepModel.Grep(gc);
            }
            catch (Exception e)
            {
                done = true;
            }
       
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
            WriteProgress(new ProgressRecord(1, "Working...", "Completed"));

            if (gr.MatchInfos.Count > 0)
            {
                WriteObject(gr.MatchInfos);
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
