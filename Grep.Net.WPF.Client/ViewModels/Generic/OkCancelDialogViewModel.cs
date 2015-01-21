using System;
using System.Linq;
using Caliburn.Micro;

namespace Grep.Net.WPF.Client.ViewModels
{
    public class OkCancelDialogViewModel : Screen
    {
        public bool? Result { get; set; }

        private PropertyChangedBase _viewModel;

        public PropertyChangedBase ViewModel
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

        public string OkText { get; set; }

        public string CancelText { get; set; }

        public OkCancelDialogViewModel(string okText = "Ok", string cancelText = "Cancel")
        {
            this.OkText = okText;
            this.CancelText = cancelText;
        }

        public void Ok()
        {
            Result = true;
            TryClose(Result);
        }

        public void Cancel()
        {
            Result = false;
            TryClose(Result);
        }
    }
}