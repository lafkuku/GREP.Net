using System;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Grep.Net.WPF.Client.Commands;

namespace Grep.Net.WPF.Client.ViewModels
{
    public class InputDialogViewModel : Screen
    {
        public String Question { get; set; }

        public String Input { get; set; }

        public ICommand OkCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        public InputDialogViewModel()
        {
            this.Input = "";

            OkCommand = new DelegateCommand(Ok);
            CancelCommand = new DelegateCommand(Cancel); 
        }

        public void Ok(Object o)
        {
            this.TryClose(true);
        }

        public void Cancel(Object o)
        {
            this.TryClose(false);
        }
    }
}