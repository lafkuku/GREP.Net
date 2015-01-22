using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Caliburn.Micro;
using Grep.Net.Model;
using Grep.Net.WPF.Client.Commands;
using NLog;


namespace Grep.Net.WPF.Client.ViewModels
{
    public class SearchViewModel : PropertyChangedBase
    {
        private String _searchText;

        public String SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                _searchText = value;
                NotifyOfPropertyChange(() => SearchText);
            }
        }

        public ListCollectionView RecentSearches { get; set; }

        public ICommand SearchCommand { get; set; }

        public ICommand RemoveAllSearches { get; set; }

        public ICommand RemoveSearch { get; set; }

        static Logger logger = NLog.LogManager.GetCurrentClassLogger(); 
        
        public SearchViewModel()
        {
            RecentSearches = new ListCollectionView(GTApplication.Instance.Settings.RecentSearches);
            SearchCommand = new DelegateCommand(this.Search);
            RemoveAllSearches = new DelegateCommand((x) =>
            {
                this.RecentSearches.Cast<String>().ToList().ForEach(y => this.RecentSearches.Remove(y));
                this.RecentSearches.CommitEdit();
            });

            RemoveSearch = new DelegateCommand((x) =>
            {
                if (x is String)
                {
                    string searchText = x as string;
                    if (RecentSearches.Contains(searchText))
                    {
                        RecentSearches.Remove(searchText);
                        RecentSearches.CommitEdit();
                    }
                }
            });
        }

        public void MouseDoubleClick(ActionExecutionContext context)
        {
            string value = context.Source.DataContext as string;
            if (!String.IsNullOrEmpty(value))
            {
                this.SearchText = value;
            }
        }

        public void PreviewKeyPress(ActionExecutionContext context)
        {
        }

        public void Search(object param)
        {
            if (!String.IsNullOrWhiteSpace(SearchText))
            {
                try
                {
                    RootViewModel.Instance.Search(this.SearchText);
                    
                    if (!this.RecentSearches.Contains(SearchText))
                    {
                        RecentSearches.AddNewItem(SearchText);
                        RecentSearches.CommitNew();
                    }
                    this.SearchText = "";
                    
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
            }
            else
            {
                MessageBox.Show("Null or Empty SearchText, cannot search!");
            }
        }
    }
}