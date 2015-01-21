using System;
using System.Linq;
using System.Windows.Data;
using Caliburn.Micro;

namespace Grep.Net.WPF.Client.ViewModels
{
    public class NLogViewModel : Screen
    {
        public string Name
        {
            get
            {
                return "Log";
            }
        }
        
        public ListCollectionView Messages { get; set; }

        public NLogViewModel()
        {
            Closeable = false;
            Messages = new ListCollectionView(NLogModel.Messages);
        }

        private bool _closeable;

        public bool Closeable
        {
            get
            {
                return _closeable;
            }
            set
            {
                _closeable = value;
                NotifyOfPropertyChange(() => Closeable);
            }
        }
    }
}