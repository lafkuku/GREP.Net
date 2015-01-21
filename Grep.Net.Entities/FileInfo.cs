using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Grep.Net.Entities
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class FileInfo : BaseEntity
    {
        [XmlIgnore]
        [IgnoreDataMember]
        public String FullName
        {
            get
            {
                if (Path[Path.Length - 1] == '\\')

                    return Path + Name;
                else
                    return Path + "\\" + Name;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    string fName = System.IO.Path.GetFileName(value);
                    string path = value.Remove(value.IndexOf(fName));
                    this.Name = fName;
                    this.Path = path;
                    this.FileExtension = new FileExtension() { Extension = System.IO.Path.GetExtension(value) };
                }
                else
                {
                    this.Name = "";
                    this.Path = "";
                }
            }
        }
        [DataMember]
        [XmlElement]
        public String Path { get; set; }
        [DataMember]
        [XmlElement]
        public String Name { get; set; }
        [DataMember]
        [XmlElement]
        public string Notes { get; set; }
        [DataMember]
        [XmlElement]
        public virtual FileExtension FileExtension { get; set; }
        
        public FileInfo()
        {
            this.Name = "";
            this.Path = "";
            this.Notes = ""; 
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}