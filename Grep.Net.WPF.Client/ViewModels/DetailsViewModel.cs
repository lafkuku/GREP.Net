using System;
using System.Linq;
using Caliburn.Micro;

namespace Grep.Net.WPF.Client.ViewModels
{
    public class DetailsViewModel : PropertyChangedBase
    {
        private Object _viewModel;

        public Object ViewModel
        {
            get
            {
                return _viewModel;
            }
            set
            {
                _viewModel = value;
                NotifyOfPropertyChange(() => ViewModel);
            }
        }

        public string Name
        {
            get
            {
                return "Details";
            }
        }

        public bool Closeable
        {
            get
            {
                return false;
            }
        }

        public DetailsViewModel()
        {
        }
    }
}