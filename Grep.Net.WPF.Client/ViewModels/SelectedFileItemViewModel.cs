using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Grep.Net.WPF.Client.ViewModels
{
    public class SelectedFileItemViewModel
    {
        public BindableCollection<String> SelectedDirectoriesOrFiles { get; set; }

        public SelectedFileItemViewModel()
        {
            this.SelectedDirectoriesOrFiles = new BindableCollection<string>();
        }
    }
}
