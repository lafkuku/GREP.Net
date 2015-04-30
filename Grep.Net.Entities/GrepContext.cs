using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Threading;

namespace Grep.Net.Entities
{
    public enum SearchErrorType
    {
        NoPatterns = 0,
        NoFiles,
        NoFilesWithSelectedExtensions,
    }

    [Serializable]
    [XmlRootAttribute("GrepContext", Namespace = "")]
    [DataContract(Namespace = "")]
    public class GrepContext : IEntity
    {
        [DataMember]
        [XmlElement]
        public Guid Id { get; set;  }


        [DataMember]
        [XmlElement]
        public string RootPath { get; set; }

    
        [DataMember]
        [XmlElement]
        public String Status { get; set; }

        [DataMember]
        [XmlElement]
        public DateTime TimeStarted { get; set; }
        [DataMember]
        [XmlElement]
        public DateTime TimeCompleted { get; set; }

        public bool Completed { get; set; }
        [DataMember]
        [XmlArray(ElementName = "Patterns")]
        public virtual List<PatternPackage> PatternPackages { get; set; }
        [DataMember]
         [XmlArray(ElementName = "Extensions")]
        public virtual List<FileExtension> FileExtensions { get; set; }



        public delegate void CompletedAction(GrepContext _this, GrepResult result);
        [XmlIgnore]
        [IgnoreDataMember]
        public CompletedAction OnCompleted;


        public delegate void NextDirectory(GrepContext _this, String path);
        [XmlIgnore]
        [IgnoreDataMember]
        public NextDirectory OnDirectory;


        public delegate void Error(GrepContext _this, String errorMessage);
        [XmlIgnore]
        [IgnoreDataMember]
        public Error OnError;

        [XmlIgnore]
        [IgnoreDataMember]
        public CancellationTokenSource CancelToken;

        public GrepContext()
        {
            PatternPackages = new List<PatternPackage>();
            FileExtensions = new List<FileExtension>();
            CancelToken = new CancellationTokenSource(); 
        }
    }
}