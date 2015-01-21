using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grep.Net.Entities;

namespace Grep.Net.WPF.Client.Interfaces
{
    public interface IViewModel<T> where T : IEntity
    {
        T Entity { get; set; }
    }
}
