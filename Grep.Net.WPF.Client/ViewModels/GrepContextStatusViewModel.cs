using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grep.Net.WPF.Client.Interfaces;
using Grep.Net.WPF.Client.Services;
using Grep.Net.WPF.Client.ViewModels.Entities;
using Grep.Net.WPF.Client.ViewModels;

using Caliburn.Micro;

namespace Grep.Net.WPF.Client.ViewModels
{
    public class GrepContextStatusViewModel : PropertyChangedBase
    {
        public BindableCollection<GrepContextViewModel> Contexts { get; set; }

        public IDataService DataService { get; set;  }


        public bool Closeable
        {
            get
            {
                return false;
            }
        }

        public string Name
        {
            get
            {
                return "GrepStatus";
            }

        }

        public GrepContextStatusViewModel(IDataService dataService)
            
        {
            Contexts = new BindableCollection<GrepContextViewModel>(); 
            DataService = dataService;
            DataService.GrepContextService.OneWaySyncTo(Contexts);
            
        }
    }
}
