using System;
using System.Linq;
using System.Windows.Data;
using Caliburn.Micro;
using Grep.Net.Entities;
using Grep.Net.WPF.Client.Controls;
using Grep.Net.WPF.Client.Interfaces;
using ICSharpCode.AvalonEdit.Search;
using Grep.Net.WPF.Client.Services;

namespace Grep.Net.WPF.Client.ViewModels
{
    public class ManageMasterTreeViewViewModel : Screen
    {
        public String Name
        {
            get
            {
                return "Manage";
            }
        }

        public ListCollectionView FileTypeDefinitions { get; set; }

        public ListCollectionView PatternPackages { get; set; }

        private BindableCollection<TreeViewItemViewModel> _treeViewItems; 

        public ListCollectionView TreeViewItems { get; set; }

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

        public AvalonEditControl Editor { get; set; }

        public SearchPanel SearchPanel { get; set; }

        IDataService DataService { get; set; }

        public ManageMasterTreeViewViewModel(IDataService dataService)
        {
            DataService = dataService;
            FileTypeDefinitions = new ListCollectionView(DataService.FileTypeDefinitionService.GetAll().ToList()); 
            PatternPackages = new ListCollectionView(DataService.PatternPackageService.GetAll().ToList());

            PatternPackages.SortDescriptions.Add(new System.ComponentModel.SortDescription("Category", System.ComponentModel.ListSortDirection.Ascending));
            
            _treeViewItems = new BindableCollection<TreeViewItemViewModel>();
            TreeViewItems = new ListCollectionView(_treeViewItems);

            //Setup the editor
            this.Editor = new AvalonEditControl();
            SearchPanel.Install(Editor);

            InitTreeViewItems(); 
        }

        public void InitTreeViewItems()
        {
            ManageTreeViewItem mtvi = new ManageTreeViewItem("Pattern Packages", PatternPackages);
            _treeViewItems.Add(mtvi);

            mtvi = new ManageTreeViewItem("File Type Definitions", FileTypeDefinitions);
            _treeViewItems.Add(mtvi);

         
        }

        public void OnSelectedItemChanged(ActionExecutionContext context)
        {
            var selectedItem = (context.Source as System.Windows.Controls.TreeView).SelectedItem;
            if (selectedItem == null || selectedItem is ManageTreeViewItem)
            {
                //TODO: Log
                return;
            }

            this.SelectedItem = selectedItem;
        }

        public void Reload()
        {
            if (this.SelectedItem is IFileEntity)
            {
                IFileEntity ife = this.SelectedItem as IFileEntity;
            }
        }
  
        public void Delete()
        {
        }
    }

    public class ManageTreeViewItem : TreeViewItemViewModel
    {
        public String DisplayName { get; set; }

        public ManageTreeViewItem(String name, ListCollectionView children)
        {
            this.DisplayName = name;
            this.Children = children;
        }

        public void AddNewItem(ActionExecutionContext context)
        {
            dynamic tvi = context; 

            //Hack I know.. Being lazy atm. 
            string headerName = tvi.Source.DataContext.DisplayName; 
            switch(headerName)
            {
                case "Pattern Packages":
                    PatternPackage pp = new PatternPackage();
                    Grep.Net.Model.GTApplication.Instance.DataModel.PatternPackageRepository.Add(pp); 
                    break;
                case "File Type Definitions":
                    FileTypeDefinition ftd = new FileTypeDefinition(); 
                    Grep.Net.Model.GTApplication.Instance.DataModel.FileTypeDefinitionRepository.Add(ftd); 
                    break;
                
                case "Templates":
                    Template t = new Template();
                    Grep.Net.Model.GTApplication.Instance.DataModel.TemplateRepository.Add(t);
                    break;
                default:
                    break;
            }
        }
    }
}