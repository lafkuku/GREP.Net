using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Grep.Net.Entities
{
  
    public interface IEntity : IEquatable<Object>
    {
        Guid Id { get; set; }
    }
}