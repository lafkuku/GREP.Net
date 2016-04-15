using System;
using System.Linq;
using Caliburn.Micro;
using Grep.Net.WPF.Client.Services;
namespace Grep.Net.WPF.Client.ViewModels
{
    public class SelectItemsViewModel : Screen
    {
        public FileTypeDefinitionTreeViewModel FileTypeDefinitionTreeViewModel { get; set; }


       // public SelectedFileItemViewModel SelectedFileItemsViewModel { get; set; }
        public DirectoryExplorerViewModel DirectoryExplorer { get; set; }
      

        public PatternPackageTreeViewModel PatternPatckageTreeView { get; set; }

        public SearchViewModel SearchViewModel { get; set; }

        public IDataService DataService { get; set; }

        private Object _selectedItem;

        public Object SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }
        
        public SelectItemsViewModel(RootViewModel rvm, Interfaces.ISettingsManager settingsManager, IDataService dataService)
        {
            this.DataService = dataService;
            this.DirectoryExplorer = new DirectoryExplorerViewModel(settingsManager);
           // this.SelectedFileItemsViewModel = new SelectedFileItemViewModel(); 
            this.PatternPatckageTreeView = new PatternPackageTreeViewModel(dataService);
            this.FileTypeDefinitionTreeViewModel = new FileTypeDefinitionTreeViewModel(dataService);
            this.SearchViewModel = new SearchViewModel(rvm);

            this.PatternPatckageTreeView.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(PropertyChangedEventHandler);
            this.FileTypeDefinitionTreeViewModel.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(PropertyChangedEventHandler);
        }

        private void PropertyChangedEventHandler(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            if (args.PropertyName.Equals("SelectedItem"))
            {
                dynamic tmp = sender;
                this.SelectedItem = tmp.SelectedItem;
            }
        }
    }
}