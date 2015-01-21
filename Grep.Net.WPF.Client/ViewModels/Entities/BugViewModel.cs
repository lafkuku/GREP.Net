using System;
using System.Linq;
using Caliburn.Micro;
using Grep.Net.Entities;

namespace Grep.Net.WPF.Client.ViewModels.Entities
{
    public class BugViewModel : PropertyChangedBase
    {
        private Bug _bug;

        public Bug Bug
        {
            get
            {
                return _bug;
            }
            set
            {
                _bug = value;
                NotifyOfPropertyChange(() => Bug); 
            }
        }
    }
}