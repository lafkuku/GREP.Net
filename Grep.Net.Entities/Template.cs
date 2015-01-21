using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Grep.Net.Entities
{
    [XmlRootAttribute("Template", Namespace = "", IsNullable = false)]
    public class Template : IEntity
    {
        [Key]
        [DataMember]
        [XmlElement]
        public Guid Id { get; set; }

        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string RawTemplate { get; set; }

        public Template()
        {
            Name = "Unknown_" + Guid.NewGuid().ToString();
        }
    }
}