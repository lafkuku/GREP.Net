using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;

namespace Grep.Net.WPF.Client.ViewModels
{
    public class MultiSelectListViewModel<T> : Screen
    {
        public ICollection<T> Items { get; set; }

        public ICollection<T> SelectedItems { get; set; }

        public MultiSelectListViewModel(ICollection<T> Items)
        {
        }

        public void Ok()
        {
            TryClose(true); 
        }

        public void Cancel()
        {
            TryClose(false); 
        }
    }
}