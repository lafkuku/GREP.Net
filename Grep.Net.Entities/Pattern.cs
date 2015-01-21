using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grep.Net.Entities
{
    [XmlRootAttribute("Pattern", Namespace = "")]
    [DataContract(Namespace = "")]
    public partial class Pattern 
    {
        [Key, Column(Order=0)]
        [XmlElement]
        [DataMember]
        public string PatternStr { get; set; }
      
        [DataMember]
        [XmlElement]
        public bool IsEnabled { get; set; }
      

        /// <summary>
        /// Why is this pattern here? 
        /// </summary>
        [XmlElement]
        [DataMember]
        public string PatternInfo { get; set; }

        [XmlElement]
        [DataMember]
        public string ReferenceUrl { get; set; }

        [XmlElement]
        [DataMember]
        public string Recommendation { get; set; }

        [Key, Column(Order=1)]
        [DataMember]
        public Guid PatternPackageId { get; set; }
    
     
        public Pattern()
        {
            this.Recommendation = "";
            this.ReferenceUrl = "";
            this.PatternStr = "";
            this.PatternInfo = "";
            this.IsEnabled = true;
        }
     
        public bool IsValidPattern()
        {
            try
            {
                Regex.Match("", this.PatternStr);
                return true;
            }
            catch 
            {
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Pattern))
            {
                return false;
            }
            return (obj as Pattern).PatternStr.Equals(this.PatternStr, StringComparison.InvariantCultureIgnoreCase); 
        }

        public override string ToString()
        {
            return PatternStr;
        }
    }
}