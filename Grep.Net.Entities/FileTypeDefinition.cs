using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Grep.Net.Entities
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class FileTypeDefinition : IEntity
    {
        [Key]
        [DataMember]
        [XmlElement]
        public Guid Id { get; set; }

        [XmlElement("Name")]
        [DataMember(Name = "Name")]
        public String Name { get; set; }

        [XmlArray(ElementName = "FileExtensions")]
        [DataMember(Name = "FileExtensions")]
        public List<FileExtension> FileExtensions { get; set; }

        [XmlElement("IsEnabled")]
        [DataMember(Name = "IsEnabled")]
        public bool IsEnabled { get; set; }

        public long Version { get; set; }

        public FileTypeDefinition()
        {
            this.Name = "Unknown";
            //this.Id = Guid.NewGuid(); 
            FileExtensions = new  List<FileExtension>();
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj != null &&
                obj is FileTypeDefinition)
            {
                return (obj as FileTypeDefinition).Name.Equals(this.Name);
            }
            return base.Equals(obj);
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(String.Format("File Type Definiton: {0}", this.Name));

            foreach (FileExtension fe in FileExtensions)
            {
                sb.Append(String.Format("[{0}] ,", fe.ToString()));
            }
            //Remove the last index as that'll be a , 
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }
    }
}