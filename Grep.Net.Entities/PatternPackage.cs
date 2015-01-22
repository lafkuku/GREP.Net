using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Caliburn.Micro;



namespace Grep.Net.Entities
{
    [Serializable]
    [XmlRootAttribute("PatternPackage", Namespace = "")]
    [DataContract(Namespace = "")]
    public class PatternPackage : IEntity
    {

        [Key, Column(Order = 0)]
        [DataMember]
        [XmlElement]
        public Guid Id { get; set; }
         
        [XmlElement]
        [DataMember]
        public string Name { get; set; }

        [XmlElement]
        [DataMember]
        public long Version { get; set; }

  
        
        [Key, Column(Order=1)]
        [XmlElement]
        [DataMember]
        public string Category { get; set; }

        /// <summary>
        /// Base Pattern can be used as a way to "wrap" all the children patterns to avoid comments and what not. 
        /// </summary>
        [XmlElement]
        [DataMember]
        public string BasePattern { get; set; }

        [XmlElement]
        [DataMember]
        public bool IsEnabled { get; set; }

        [XmlArray]
        [DataMember]
        public List<Pattern> Patterns { get; set; }

        public PatternPackage()
        {
            //Id = Guid.NewGuid(); 
            Name = "Unknown_" + Guid.NewGuid();
            Category = "Unknown"; 
            Patterns = new List<Pattern>();
            Version = 0; 
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj != null &&
                obj is PatternPackage)
            {
                return (obj as PatternPackage).Name.Equals(this.Name);
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// Creates a default Pattern Package from raw File Contents. 
        /// File must be a "return" seperated list of Regex Patterns. 
        /// </summary>
        /// <param name="fileContents"></param>
        /// <returns></returns>
        public static PatternPackage CreateFromFile(String fileContents)
        {
            PatternPackage ret = new PatternPackage(); 

            using (StringReader sr = new StringReader(fileContents))
            {
                String line = sr.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    Pattern p = new Pattern();
                    p.PatternStr = line;

                    ret.Patterns.Add(p);
                    line = sr.ReadLine(); 
                }
            }
            return ret; 
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(String.Format("PatternPackage: {0}", this.Name));

            foreach (Pattern p in Patterns)
            {
                sb.Append(String.Format("[{0}] ,", p.ToString()));
            }
            //Remove the last index as that'll be a , 
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }
    }
}