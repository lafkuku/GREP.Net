using System;
using System.Linq;

namespace Grep.Net.WPF.Client.Interfaces
{
    public interface ISelectable
    {
        bool IsSelected { get; set; }
    }
}