using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Grep.Net.Entities
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class MatchInfo : BaseEntity
    {
        [DataMember]
        [XmlElement]
        public String Line { get; set; }
        [DataMember]
        [XmlElement]
        public String Context { get; set; }
        [DataMember]
        [XmlElement]
        public int LineNumber { get; set; }
        [DataMember]
        [XmlElement]
        public bool Reviewed { get; set; }
        [DataMember]
        [XmlElement]
        public string Match { get; set; }
        [DataMember]
        [XmlElement]
        //public virtual GrepResult GrepResult { get; set; }

        public Guid GrepResultId { get; set;  }
        [DataMember]
        [XmlElement]
        public virtual FileInfo FileInfo { get; set; }
        [DataMember]
        [XmlElement]
        public virtual Pattern Pattern { get; set; }

        public MatchInfo()
        {
            this.FileInfo = new FileInfo();
        }

        public override string ToString()
        {
            return String.Format("Match: {0}:{1} [{2}] [{3}] {4}", FileInfo.FullName, LineNumber, Pattern, Match, Line);
        }
    }
}