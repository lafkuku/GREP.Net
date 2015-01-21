using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Data.Entity;



namespace Grep.Net.Entities
{
    [XmlRootAttribute(Namespace = "")]
    [DataContract(Namespace = "")]
    [Serializable]
    public class FileExtension
    {
        [Key, Column(Order = 0)]
        [XmlElement]
        [DataMember]
        public String Extension { get; set; }

        [XmlElement]
        [DataMember]
        public bool IsEnabled { get; set; }
        [DataMember]
        [Key, Column(Order=1)]
        public String FileTypeDefinitionId { get; set; }
 
        [XmlIgnore]
        public virtual FileTypeDefinition FileTypeDefinition { get; set; }
       
        public FileExtension()
        {
            //this.Id = Guid.NewGuid();
            this.Extension = "";
            this.IsEnabled = true;
        }
        public override string ToString()
        {
            return this.Extension;
        }

        public override bool Equals(object obj)
        {
            if (obj is FileExtension)
            {
                FileExtension other = obj as FileExtension;

                if (other.Extension.Equals(this.Extension, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Extension.GetHashCode(); 
        }
    }
}