using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Grep.Net.Entities
{
    [Serializable]
    [DataContract(IsReference = true, Namespace = "")]
    public abstract class BaseEntity : IEntity
    {
        [Key]
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        [XmlElement]
        public long Version { get; set; }

        public virtual bool Validate()
        {
            return true;
        }

        public BaseEntity()
        {
            Id = Guid.NewGuid(); 
        }
    }
}