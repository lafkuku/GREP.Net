namespace Grep.Net.WPF.Client
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using Caliburn.Micro;
    using Grep.Net.WPF.Client.ViewModels;

    [Export(typeof(IShell))]
    public class ShellViewModel : Screen, IShell 
    {
        public RootViewModel RootViewModel { get; set; }
  
        public ShellViewModel()
        {
            this.RootViewModel = RootViewModel.Instance;
        }
    }
}