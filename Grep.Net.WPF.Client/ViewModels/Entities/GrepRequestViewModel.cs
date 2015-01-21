using System;
using System.Linq;
using Caliburn.Micro;
using Grep.Net.Entities;

namespace Grep.Net.WPF.Client.ViewModels.Entities
{
    public class GrepRequestViewModel : PropertyChangedBase
    {
        private GrepContext _grepRequest;

        public GrepContext GrepRequest
        {
            get
            {
                return _grepRequest;
            }
            set
            {
                _grepRequest = value;
                NotifyOfPropertyChange(() => GrepRequest); 
            }
        }
   
        public GrepRequestViewModel()
        {
        }
    }
}