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
    public class PatternPackageTreeViewModel : TreeViewItemViewModel
    {
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

        private BindableCollection<CategoryTreeViewItemViewModel> _children;
     
        public ICommand AddCategory { get; set; }

        public ICommand Import
        {
            get
            {
                return UtilityCommands.ImportEntity;
            }
        }

        private IDataService DataService { get; set;  }

        public PatternPackageTreeViewModel(IDataService dataService)
        {
            DataService = dataService;
            //Set the commands
            AddCategory = new DelegateCommand((x) =>
            {
                InputDialogViewModel idvm = new InputDialogViewModel() { Question = "Please Enter a Category name. (Examples: C#, .Net, etc)" };

                if (GTWindowManager.Instance.ShowDialog(idvm, 175, 325) == true)
                {
                    CategoryTreeViewItemViewModel cat = new CategoryTreeViewItemViewModel(idvm.Input, DataService);
                    cat.Parent = this;
                    this._children.Add(cat);
                }
            });

            //Initilize members
            _children = new BindableCollection<CategoryTreeViewItemViewModel>();
            Children = new ListCollectionView(_children);
            Children.SortDescriptions.Add(new System.ComponentModel.SortDescription("Category", System.ComponentModel.ListSortDirection.Ascending));
            RefreshChildren();


            DataService.PatternPackageService.OnItemAdded += (x) =>
            {
                if (_children.FirstOrDefault(z => z.Category.Equals(x.Category)) == null)
                {
                    _children.Add(new CategoryTreeViewItemViewModel(x.Category, DataService) { Parent = this });
                }
            };

            DataService.PatternPackageService.OnItemRemoved += x =>
            {
                foreach (CategoryTreeViewItemViewModel categoryTreeViewItemViewModel in this._children.ToList())
                {
                    categoryTreeViewItemViewModel.TryRemove(x);
                    if (categoryTreeViewItemViewModel._children.Count < 1)
                    {
                        this._children.Remove(categoryTreeViewItemViewModel);
                    }
                }
            };
            DataService.PatternPackageService.OnDataReloaded += () => RefreshChildren();
        }

        public void SelectedItemChanged(ActionExecutionContext context)
        {
            RoutedPropertyChangedEventArgs<Object> args = context.EventArgs as RoutedPropertyChangedEventArgs<Object>;
            if (args != null)
            {
                dynamic tmp = args.NewValue as PatternPackageTreeViewItemViewModel; 
                if (tmp != null)
                {
                    SelectedItem = tmp.PatternPackageViewModel;
                }

                tmp = args.NewValue as PatternTreeViewItemViewModel;
                if (tmp != null)
                {
                    SelectedItem = tmp.PatternViewModel;
                }
            }
        }

        public override void RefreshChildren()
        {
            _children.Clear();
            var patternPackages = DataService.PatternPackageService.GetAll();

            var categories = patternPackages.Select(x => x.Category).Distinct();

            foreach (var category in categories)
            {
                CategoryTreeViewItemViewModel ctvivm = new CategoryTreeViewItemViewModel(category, DataService);
                if (patternPackages.Where(x=>x.Category.Equals(category)).All(x => x.IsEnabled))
                {
                    ctvivm.IsChecked = true;
                }
                else if (patternPackages.Where(x => x.Category.Equals(category)).Any(x => x.IsEnabled))
                {
                    ctvivm.IsChecked = null;
                }
                else
                    ctvivm.IsChecked = false;
                _children.Add(ctvivm);
            }
        }
    }
    public class CategoryTreeViewItemViewModel : TreeViewItemViewModel
    {
        public String Category { get; set; }

        internal BindableCollection<PatternPackageTreeViewItemViewModel> _children;

        public override bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                _isExpanded = value;
                NotifyOfPropertyChange(() => IsExpanded);
            }
        }

        internal bool? _isChecked;

        public bool? IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                CheckedStateChanged(value);
                NotifyOfPropertyChange(() => IsChecked);
            }
        }

        public ICommand RemoveCategory { get; set; }

        public ICommand AddPatternPackage { get; set; }

        private IDataService DataService { get; set; }

        public CategoryTreeViewItemViewModel(String category, IDataService dataService)
        {
            DataService = dataService;
            
            //This should be initilized before IsChecked is called (Otherwise null exception)
            _children = new BindableCollection<PatternPackageTreeViewItemViewModel>();


            _isChecked = false;
            this.Category = category;

            RemoveCategory = new DelegateCommand((x) =>
            {
                if (x != null && x is CategoryTreeViewItemViewModel)
                {
                    //This messagebox is bad =(. I need to abstract it out as well. 
                    if (MessageBox.Show("Are you sure you want to delete this category? All child PatternPackages will also be deleted.", "Are you sure?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        var cat = x as CategoryTreeViewItemViewModel;
                        foreach (PatternPackageTreeViewItemViewModel ppvm in cat._children.ToList())
                        {
                           DataService.PatternPackageService.Remove(ppvm.PatternPackageViewModel.Entity);
                        }
                    }
                }
            });

            AddPatternPackage = new DelegateCommand((x) =>
            {
                InputDialogViewModel idvm = new InputDialogViewModel() { Question = "Please Enter a Pattern Package name. (Examples: Bad Apis, Secrets, Etc)" };

                if (GTWindowManager.Instance.ShowDialog(idvm, 175, 325) == true)
                {
                    PatternPackage pp = new PatternPackage()
                    {
                        Category = this.Category,
                        Name = idvm.Input
                    };
                    DataService.PatternPackageService.Add(pp);
                }
            });

            Children = new ListCollectionView(_children);
            Children.SortDescriptions.Add(new System.ComponentModel.SortDescription("PatternPackageViewModel.Name", System.ComponentModel.ListSortDirection.Ascending));
            RefreshChildren();
        }

        public override void RefreshChildren()
        {
            _children.Clear();
            foreach (PatternPackageViewModel ppvm in DataService.PatternPackageService.GetAll())
            {
                if (ppvm.Category.Equals(this.Category, StringComparison.CurrentCultureIgnoreCase))
                {
                    var pptvvm = new PatternPackageTreeViewItemViewModel(ppvm, DataService);
                    pptvvm.Parent = this;
                    _children.Add(pptvvm);
                }
            }
        }

        public bool TryRemove(PatternPackageViewModel ppvm)
        {
            PatternPackageTreeViewItemViewModel pptvivm = null;
            foreach (PatternPackageTreeViewItemViewModel childPPTVIVM in this._children)
            {
                if (childPPTVIVM.PatternPackageViewModel == ppvm)
                {
                    pptvivm = childPPTVIVM;
                    break;
                }
            }
            if (pptvivm != null)
            {
                _children.Remove(pptvivm);
                return true;
            }
            return false;
        }

        public void CheckedStateChanged(bool? value, bool cascadeChildren = true)
        {
            if ((value == true || value == false) && cascadeChildren)
            {
                foreach (PatternPackageTreeViewItemViewModel pptvivm in this._children)
                {
                    pptvivm.IsChecked = value;
                }
            }
        }

        public void ResetCheckState()
        {
            var patternPackages = this._children.Select(x => x.PatternPackageViewModel.Entity);
            if (patternPackages.Where(x => x.Category.Equals(this.Category)).All(x => x.IsEnabled))
            {
                this._isChecked = true;
            }
            else if (patternPackages.Where(x => x.Category.Equals(this.Category)).Any(x => x.IsEnabled))
            {
                this._isChecked = null;
            }
            else
                this._isChecked = false;

            NotifyOfPropertyChange(() => IsChecked);
        }
    }
    public class PatternPackageTreeViewItemViewModel : TreeViewItemViewModel
    {
        public PatternPackageViewModel PatternPackageViewModel { get; private set; }

        
        internal SyncedBindableCollection<PatternViewModel, PatternTreeViewItemViewModel> _children;
        
        
        internal bool? _isChecked;

        
        public bool? IsChecked
        {
            get
            {
                return this._isChecked;
            }
            set
            {
                _isChecked = value;
                CheckedStateChanged(value);
                NotifyOfPropertyChange(() => IsChecked);
            }
        }

        public override bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                _isExpanded = value;
                NotifyOfPropertyChange(() => IsExpanded);
            }
        }
    
        public ICommand AddPattern { get; set; }

        public ICommand Remove { get; set; }

        public ICommand Export
        {
            get
            {
                return UtilityCommands.ExportEntity;
            }
        }

        public IDataService DataService { get; set; }

        public PatternPackageTreeViewItemViewModel(PatternPackageViewModel package, IDataService dataService)
        {
            this.DataService = dataService;
            this._isChecked = package.IsEnabled;
            
            _children = new SyncedBindableCollection<PatternViewModel, PatternTreeViewItemViewModel>(package.Patterns, (x) =>
            {
                return new PatternTreeViewItemViewModel(x, dataService) { Parent = this };
            }, (x, y) =>
            {
                return x.Pattern == y.PatternViewModel.Pattern;
            }); 
            this.PatternPackageViewModel = package;

            AddPattern = new DelegateCommand(x =>
            {
                InputDialogViewModel idvm = new InputDialogViewModel() { Question = "Please Enter a Pattern. (Examples: Bad Apis, Secrets, Etc)" };

                if (GTWindowManager.Instance.ShowDialog(idvm, 175, 325) == true)
                {
                    Pattern p = new Pattern()
                    {
                        PatternStr = idvm.Input,
                        PatternPackageId = this.PatternPackageViewModel.Entity.Id
                    };
                    this.PatternPackageViewModel.Entity.Patterns.Add(p);
                }
            });

            Remove = new DelegateCommand((x) =>
            {
                if (x != null && x is PatternPackageTreeViewItemViewModel)
                {
                    if (MessageBox.Show("Are you sure you want to delete this Package? All patterns will also be deleted.", "Are you sure?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        var pp = x as PatternPackageTreeViewItemViewModel;
                        
                        DataService.PatternPackageService.Remove(pp.PatternPackageViewModel.Entity);
                    }
                }
            });

            this.Children = new ListCollectionView(_children);
            Children.SortDescriptions.Add(new System.ComponentModel.SortDescription("PatternViewModel.PatternStr", System.ComponentModel.ListSortDirection.Ascending));
            RefreshChildren(); 
        }

        public override void RefreshChildren()
        {
            _children.Clear();
            foreach (PatternViewModel pvm in this.PatternPackageViewModel.Patterns)
            {
                _children.Add(new PatternTreeViewItemViewModel(pvm, DataService) { Parent = this });
            }
        }

        public void CheckedStateChanged(bool? value)
        {
            //Check the state of my siblings
            var parentTVIVM = (Parent as CategoryTreeViewItemViewModel);

            if (value == true)
                this.PatternPackageViewModel.IsEnabled = true;
            else
                this.PatternPackageViewModel.IsEnabled = false;

            if (parentTVIVM != null)
            {
                parentTVIVM.ResetCheckState();
                /*
                if (value == false || value == null)
                {
                    if (parentTVIVM._children.Any(x => x.PatternPackageViewModel.IsEnabled == true))
                        parentTVIVM.IsChecked = null;
                    else
                    {
                        if (value == false)
                        {
                            parentTVIVM._isChecked = false; //Set the state, but avoid this callback. 
                            parentTVIVM.CheckedStateChanged(false, false);
                            parentTVIVM.NotifyOfPropertyChange(() => parentTVIVM.IsChecked);
                        }
                        else
                        {
                            parentTVIVM.IsChecked = null;
                        }
                    }
                }
                else if (value == true)
                {
                    if (parentTVIVM._children.Any(x => x.PatternPackageViewModel.IsEnabled == false))
                        parentTVIVM.IsChecked = null;
                    else
                    {
                        if (value == true)
                        {
                            parentTVIVM._isChecked = true; //Set the state, but avoid this callback. 
                            parentTVIVM.CheckedStateChanged(true, false);
                            parentTVIVM.NotifyOfPropertyChange(() => parentTVIVM.IsChecked);
                        }
                        else
                        {
                            parentTVIVM.IsChecked = null;
                        }
                    }
                }
                */
            }
        }
    }
    public class PatternTreeViewItemViewModel : TreeViewItemViewModel
    {
        public PatternViewModel PatternViewModel { get; set; }

        public ICommand Remove { get; set; }

        IDataService DataService { get; set; }

        public PatternTreeViewItemViewModel(PatternViewModel pattern, IDataService dataService)
        {
            this.PatternViewModel = pattern;
            this.DataService = dataService;
            Remove = new DelegateCommand((x) =>
            {
                if (x != null && x is PatternTreeViewItemViewModel)
                {
                    if (MessageBox.Show("Are you sure you want to delete this Pattern?", "Are you sure?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        var patternVM = x as PatternTreeViewItemViewModel;
                        PatternPackage pp = DataService.PatternPackageService.Get(patternVM.PatternViewModel.Pattern.PatternPackageId).Entity;
                        pp.Patterns.Remove(patternVM.PatternViewModel.Pattern);
                    }
                }
            });
        }
    }
}