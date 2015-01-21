using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Caliburn.Micro;
using Grep.Net.Entities;
using Grep.Net.WPF.Client.Commands;
using Grep.Net.WPF.Client.Data;
using Grep.Net.WPF.Client.ViewModels.Entities;
using Grep.Net.WPF.Client.Services;


namespace Grep.Net.WPF.Client.ViewModels
{
    public class FileTypeDefinitionTreeViewModel : PropertyChangedBase
    {
        private BindableCollection<FileTypeDefinitionTreeViewItemViewModel> _rootItems { get; set; }

        public ListCollectionView RootItems { get; set; }

        public ICommand AddNewFileTypeDefinition { get; set; }

        public ICommand Import
        {
            get
            {
                return UtilityCommands.ImportEntity;
            }
        }

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

        IDataService DataService { get; set; }
        public FileTypeDefinitionTreeViewModel(IDataService dataService)
        {
            DataService = dataService;

            AddNewFileTypeDefinition = new DelegateCommand(x =>
            {
                InputDialogViewModel idvm = new InputDialogViewModel() { Question = "Please enter a name for the package. (Examples: .Net, C#, Secrets, SQLi, Etc.)" };

                if (GTWindowManager.Instance.ShowDialog(idvm, 175, 325) == true)
                {
                    //Verify we do not already have a package with the same name. (We avoid that shit). 

                    if (!DataService.FileTypeDefinitionService.GetAll().Any(y => y.Name.Equals(idvm.Input)))
                    {
                        DataService.FileTypeDefinitionService.Add(new FileTypeDefinition()
                        {
                            Name = idvm.Input
                        });
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Name already exists.");
                    }
                }
            });

            RefreshChildren();
        }

        public void RefreshChildren()
        {
            if (_rootItems == null)
            {
                _rootItems = new BindableCollection<FileTypeDefinitionTreeViewItemViewModel>();
                RootItems = new ListCollectionView(_rootItems);
                RootItems.SortDescriptions.Add(new System.ComponentModel.SortDescription("Name", System.ComponentModel.ListSortDirection.Ascending));
            }
              
            
            _rootItems.Clear(); 

            foreach (FileTypeDefinitionViewModel vm in DataService.FileTypeDefinitionService.GetAll())
            {
                var tmp = new FileTypeDefinitionTreeViewItemViewModel(DataService) { FileTypeDefinitionViewModel = vm };
                _rootItems.Add(tmp);
            }
        }

        public void SelectedItemChanged(ActionExecutionContext context)
        {
            RoutedPropertyChangedEventArgs<Object> args = context.EventArgs as RoutedPropertyChangedEventArgs<Object>;
            if (args != null)
            {
                dynamic tmp = args.NewValue as FileTypeDefinitionTreeViewItemViewModel;
                if (tmp != null)
                {
                    SelectedItem = tmp.FileTypeDefinitionViewModel;
                }

                tmp = args.NewValue as FileExtensionTreeViewItemViewModel;
                if (tmp != null)
                {
                    SelectedItem = tmp.FileExtensionViewModel;
                }
            }
        }
    }

    public class FileTypeDefinitionTreeViewItemViewModel : TreeViewItemViewModel
    {
        private FileTypeDefinitionViewModel _fileTypeDefinition { get; set; }

        public FileTypeDefinitionViewModel FileTypeDefinitionViewModel
        {
            get
            {
                return _fileTypeDefinition;
            }
            set
            {
                _fileTypeDefinition = value;
                _fileExtensionViewModels = new SyncedBindableCollection<FileExtensionViewModel, FileExtensionTreeViewItemViewModel>(value.FileExtensions,
                    (x => new FileExtensionTreeViewItemViewModel() { FileExtensionViewModel = x }),
                    (x, y) => { return x.FileExtension == y.FileExtensionViewModel.FileExtension; });

                Children = new ListCollectionView(_fileExtensionViewModels);
                Children.SortDescriptions.Add(new System.ComponentModel.SortDescription("Extension", System.ComponentModel.ListSortDirection.Ascending));
                RefreshChildren();
                NotifyOfPropertyChange(() => FileTypeDefinitionViewModel);
            }
        }


        public String Name
        {
            get
            {
                return _fileTypeDefinition.Name;
            }
            set
            {
                _fileTypeDefinition.Name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        private SyncedBindableCollection<FileExtensionViewModel, FileExtensionTreeViewItemViewModel> _fileExtensionViewModels { get; set; }

        //Commands 
        public ICommand AddNewExtension { get; set; }

        public ICommand RemoveFileTypeDefinition { get; set; }

        public ICommand ExportAsXmlCommand
        {
            get
            {
                return UtilityCommands.ExportEntity;
            }
        }

        IDataService DataService { get; set; }

        public FileTypeDefinitionTreeViewItemViewModel(IDataService dataService)
        {
            DataService = dataService;
            AddNewExtension = new DelegateCommand((x) =>
            {
                if (x != null && x is FileTypeDefinition)
                {
                    InputDialogViewModel idvm = new InputDialogViewModel() { Question = "Please Enter a extension. (Examples: .txt, .cs, .xml)" };

                    if (GTWindowManager.Instance.ShowDialog(idvm, 175, 325) == true)
                    {
                        FileTypeDefinition ftd = x as FileTypeDefinition;
                        if (ftd.FileExtensions.FirstOrDefault(y => { return y.Extension.Equals(idvm.Input); }) == null)
                        {
                            ftd.FileExtensions.Add(new FileExtension() { Extension = idvm.Input });
                            RefreshChildren();
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("Extension already exists in File Type Definition.");
                        }
                    }
                }
            });
            RemoveFileTypeDefinition = new DelegateCommand((x) =>
            {
                if (x != null && x is FileTypeDefinition)
                {
                    FileTypeDefinition ftd = x as FileTypeDefinition;
                    DataService.FileTypeDefinitionService.Remove(ftd);
                }
            });
        }

        public override void RefreshChildren()
        {
            _fileExtensionViewModels.Clear();
            List<FileExtensionTreeViewItemViewModel> tmp = new List<FileExtensionTreeViewItemViewModel>();
            foreach (FileExtensionViewModel fe in this.FileTypeDefinitionViewModel.FileExtensions)
            {
                FileExtensionTreeViewItemViewModel vm = new FileExtensionTreeViewItemViewModel()
                {
                    FileExtensionViewModel = fe,
                    Parent = this
                };

                tmp.Add(vm);
            }

            _fileExtensionViewModels.AddRange(tmp);
        }
    }

    public class FileExtensionTreeViewItemViewModel : TreeViewItemViewModel 
    {
        private FileExtensionViewModel _fileExtensionViewModel;

        public FileExtensionViewModel FileExtensionViewModel
        {
            get
            {
                return _fileExtensionViewModel;
            }
            set
            {
                _fileExtensionViewModel = value;
                NotifyOfPropertyChange(() => FileExtensionViewModel);
            }
        }

        public String Extension
        {
            get
            {
                return this._fileExtensionViewModel.Extension;
            }
            set
            {
                this._fileExtensionViewModel.Extension = value;
                NotifyOfPropertyChange(() => Extension);
            }
        }

        public ICommand DeleteExtension { get; set; }
   
        public FileExtensionTreeViewItemViewModel()
        {
            DeleteExtension = new DelegateCommand((x) =>
            {
                if (x != null && x is FileExtensionTreeViewItemViewModel)
                {
                    FileExtensionTreeViewItemViewModel fevm = x as FileExtensionTreeViewItemViewModel;
                    if (fevm.Parent != null && fevm.Parent is FileTypeDefinitionTreeViewItemViewModel)
                    {
                        FileTypeDefinitionTreeViewItemViewModel parent = fevm.Parent as FileTypeDefinitionTreeViewItemViewModel;

                        var toDelete = parent.FileTypeDefinitionViewModel.FileExtensions.FirstOrDefault(y => y.FileExtension == fevm.FileExtensionViewModel.FileExtension);
                        if (toDelete != null)
                        {
                            parent.FileTypeDefinitionViewModel.FileExtensions.Remove(toDelete);
                            parent.RefreshChildren();
                        }
                    }
                }
            });
        }
    }
}