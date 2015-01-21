using System;
using System.Linq;
using Caliburn.Micro;
using System.Windows.Data;

namespace Grep.Net.WPF.Client.ViewModels.CrumbListView
{
    public class CrumbListViewModel : PropertyChangedBase
    {
        private String _display { get; set; }

        public String Display
        {
            get
            {
                return _display;
            }
            set
            { 
                _display = value;
                NotifyOfPropertyChange(() => Display);
            }
        }
        private CrumbNavigationListViewBaseViewModel _owner;

        public CrumbNavigationListViewBaseViewModel Owner
        {
            get
            {
                return _owner;
            }
            set
            {
                _owner = value;
                NotifyOfPropertyChange(() => Owner);
            }
        }

        private ListCollectionView _itemsSource;
        public ListCollectionView ItemsSource
        {
            get
            {
                return _itemsSource;
            }
            set
            {
                _itemsSource = value;
                NotifyOfPropertyChange(() => ItemsSource);
            }
        }
    }
}