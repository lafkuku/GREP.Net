using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Collections;

namespace Grep.Net.Entities
{
    [Serializable]
    [DataContract(Namespace = "")]
    [XmlRootAttribute("EntityContainer", Namespace = "")]
    public class EntityContainer : ArrayList
    {
    }
}
