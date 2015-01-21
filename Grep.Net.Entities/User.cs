using System;
using System.Linq;

namespace Grep.Net.Entities
{
    /// <summary>
    /// Stubbing for when we eventually have a server side solution aswell. 
    /// </summary>
    public class User : BaseEntity
    {
        public String FirstName { get; set; }

        public String LastName { get; set; }

        public String LiveId { get; set; }

        public Guid Puid { get; set; }
    }
}