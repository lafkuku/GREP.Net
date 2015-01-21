using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Grep.Net.Entities
{
    public class Bug : IEntity
    {
        [Key]
        public Guid Id { get; set; }

        public String Text { get; set; }

        public virtual Template Template { get; set; }

        public virtual List<MatchInfo> MatchInfos { get; set; }

        public Bug()
        {
            Id = Guid.NewGuid();
            MatchInfos = new List<MatchInfo>();
            Text = "";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Bug))
            {
                return false;
            }
            return (obj as Bug).Id == this.Id;
        }
    }
}