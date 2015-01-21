using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grep.Net.WPF.Client.Interfaces
{
    public interface IExpandable
    {
        bool? IsChecked { get; set; }
        bool IsExpanded { get; set; }
    }
}
