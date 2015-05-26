using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Grep.Net.Entities;
using Caliburn.Micro;

namespace Grep.Net.WPF.Client.ViewModels
{
    public class DirectorySelectionViewModel
    {
        public BindableCollection<FileInfo> Test { get; set;  }
    }
}
