using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Caliburn.Micro;
using Grep.Net.WPF.Client.Commands;
using Microsoft.WindowsAPICodePack.Dialogs;
using Grep.Net.WPF.Client.ViewModels.TreeViewModels;
using Grep.Net.WPF.Client.Data;
using Grep.Net.WPF.Client.Interfaces;
using Grep.Net.WPF.Client.Services;
using Grep.Net.Model;


namespace Grep.Net.WPF.Client.ViewModels
{
    public class DirectoryExplorerViewModel : DirectoryViewModelTreeViewItem
    {

        public ListCollectionView RootItems { get; set; }

        private BindableCollection<DirectoryViewModelTreeViewItem> _rootItems { get; set; }

        public ICommand AddShortCutCommand { get; set; }


        public ICommand AddShortCutsCommand { get; set; }

        public ICommand RemoveShortCutCommand { get; set; }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private Interfaces.ISettingsManager SettingsManager { get; set; }


        public DirectoryExplorerViewModel(Interfaces.ISettingsManager settingsManager)
        {
            SettingsManager = settingsManager;
       
            _rootItems = new BindableCollection<DirectoryViewModelTreeViewItem>();

            RootItems = new ListCollectionView(_rootItems);
            Init();

            #region Commands

            AddShortCutsCommand = new DelegateCommand((x) =>
            {
                var dialog = new CommonOpenFileDialog();
          

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var file = dialog.FileName;
                    if (File.Exists(file))
                    {
                        var contents = File.ReadAllLines(file);
                        foreach (var dir in file)
                        {
                            if (!settingsManager.PathShortCuts.Contains(dir.ToString()))
                            {
                                this.SettingsManager.PathShortCuts.Add(Path);
                                this.SettingsManager.Save();
                            }
                        }
                    }
                    settingsManager.Save();
                    //AddFolderToRoot(path);
                }
            });

            AddShortCutCommand = new DelegateCommand((x) =>
            {
                var dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = true;

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var path = dialog.FileName;
                    if (!settingsManager.PathShortCuts.Contains(path))
                    {
                        settingsManager.PathShortCuts.Add(path);
                        settingsManager.Save();
                    }
                   
                    //AddFolderToRoot(path);
                }
            });

            RemoveShortCutCommand = new DelegateCommand((x) =>
            {
                if (x != null && x is String)
                {
                    if (SettingsManager.PathShortCuts.Contains(x))
                    {
                        SettingsManager.PathShortCuts.Remove(x as String);

                        //Since this can only remove "ShortCuts" we only have to look at the root items path. 
                        SettingsManager.Save(); 

                        var item = _rootItems.FirstOrDefault(y => y.Path.Equals(x as String, StringComparison.InvariantCultureIgnoreCase));
                        if (item != null)
                        {
                            RootItems.Remove(item);
                            RootItems.CommitEdit();
                        }
                    }
                }

            });
            #endregion

        }

        private IList<DirectoryViewModelTreeViewItem> CustomPaths { get; set; }

        protected virtual void Init()
        {
            CustomPaths = new List<DirectoryViewModelTreeViewItem>();
            _rootItems.Clear(); 

            //Adding the drives. 
            foreach (DriveInfo di in System.IO.DriveInfo.GetDrives())
            {
               
                AddFolderToRootInternal(di.RootDirectory.FullName);
            }
            
            //Adding special folders. 
            AddFolderToRootInternal(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            AddFolderToRootInternal(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            AddFolderToRootInternal(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            
            //Adding saved paths. 
            AddAllPathsFromSettings();

            //hook the Settings on change events to know if we need to remove a path. This is kind of a dirty hack. 
            Grep.Net.Model.GTApplication.Instance.Settings.SettingsSaving += new System.Configuration.SettingsSavingEventHandler((x, y) =>
            {
                ResetCustomPaths();
                AddAllPathsFromSettings();
            });
        }
        private bool ContainsPath(string path)
        {
            foreach(var item in this._rootItems)
            {
                if (item.Path.Equals(path, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }
            return false;
        }
        private DirectoryViewModelTreeViewItem AddFolderToRootInternal(string path, string name = null)
        {
            if (path == null)
            {
                logger.Error("Invalid argument, Path");
                throw new ArgumentException("Invalid Arugment: Path");
            }
            if (this.ContainsPath(path))
            {
                logger.Error("Already contains this path");
            }
            //If we get passed a null name, we try to get the name from the path. 
            if (name == null)
            {
                DirectoryInfo di = new DirectoryInfo(path);
                name = di.Name;
            }
             
            //If it's still null, we are probably a root. so the path is the name. 
            if (name == null)
                name = path;

            var ret = new DirectoryViewModelTreeViewItem()
            {
                Path = path,
                Name = name,
                Parent = null
            }; 
            this._rootItems.Add(ret);

            return ret;
        }

        public void AddFolderToRoot(String path)
        {
            if (!this.ContainsPath(path))
            {
                var ret = AddFolderToRootInternal(path);
                if (ret != null)
                {
                    CustomPaths.Add(ret);
                }

            }
          
        }

        public void ResetCustomPaths()
        {
            foreach (var dir in this.CustomPaths.ToList())
            {
                if (this._rootItems.Contains(dir))
                {
                    this._rootItems.Remove(dir);
                }
            }
            this.CustomPaths.Clear();
        }

        public void AddAllPathsFromSettings()
        {
            foreach (var path in SettingsManager.PathShortCuts)
            {
                if (Grep.Net.Model.Utilities.DirectoryExists(path, 3000))
                {
                    AddFolderToRoot(path);
                }
            }
        }

        public List<String> GetSelectedDirectories()
        {
            List<String> dirs = new List<string>(); 
            
            //The logic should be .. If the directory IsChecked == true, return the direcotry. If the directory IsChecked == null, we have to check the children. If == false we skip. 
            foreach (var tvm in _rootItems)
            {

                if (tvm.IsChecked == true)
                    dirs.Add(tvm.Path);
                else if (tvm.IsChecked == null)
                    dirs.AddRange(GetSelectedDirectories_helper(tvm));
                
            }
            return dirs;
        }

        private List<String> GetSelectedDirectories_helper(DirectoryViewModelTreeViewItem info)
        {
            List<String> dirs = new List<string>();
            foreach (DirectoryViewModelTreeViewItem dirTVM in info.Children)
            {
                if (dirTVM.IsChecked == true)
                    dirs.Add(dirTVM.Path);
                else if (dirTVM.IsChecked == null)
                    dirs.AddRange(GetSelectedDirectories_helper(dirTVM));
                //else skip. 
            }
            return dirs;
        }
    }

    /*
    public class DriveViewModelTreeViewItem : TreeViewItemViewModel
    {
    public override bool IsExpanded
    {
    get { return _isExpanded; }
    set
    {
    if (_isExpanded == false && _children.Contains(DummyChild))
    {
    RefreshChildren();
    }
    _isExpanded = value;
    NotifyOfPropertyChange(() => IsExpanded);
    }
    }
    public override bool? IsSelected
    {
    get { return _isSelected; }
    set
    {
    _isSelected = value;
    NotifyOfPropertyChange(() => IsSelected);
    }
    }

    private String _rootDirectory;
    public String RootDirectory
    {
    get { return _rootDirectory; }
    set
    {
    _rootDirectory = value;
    NotifyOfPropertyChange(() => RootDirectory);
    }
    }

    public DriveInfo DriveInfo { get; set; }

    public Boolean IsNetworkDrive { get { return this.DriveInfo.DriveType == DriveType.Network; } }

    private BindableCollection<DirectoryViewModelTreeViewItem> _children;

    private static DirectoryViewModelTreeViewItem DummyChild = new DirectoryViewModelTreeViewItem();
    static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger(); 

    public DriveViewModelTreeViewItem()
    {
    _children = new BindableCollection<DirectoryViewModelTreeViewItem>();
    _children.Add(DummyChild); 

    Children = new System.Windows.Data.ListCollectionView(_children);
    }

    public override void RefreshChildren()
    {
    _children.Clear();

    if (!String.IsNullOrEmpty(RootDirectory) && Directory.Exists(RootDirectory))
    {
    try {
    DirectoryInfo rootDir = new DirectoryInfo(RootDirectory);

    foreach (DirectoryInfo di in rootDir.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
    {
    try
    {
                       
    DirectoryViewModelTreeViewItem newItem = new DirectoryViewModelTreeViewItem() { 
    DirectoryInfo = di,
    Parent = this
    };
    _children.Add(newItem); 
    }
    catch (Exception e)
    {
    logger.Error(e);
    }
    }
    }
    catch (Exception e)
    {
    logger.Error(e);
    }
    }
    }
    }
    */
    /// <summary>
    /// Need to remember that in this case "IsChecked" indicates it's been "selected' by the user, verus my normal pattern of IsSelected.. IN this case IsSelected, means that is it actually "highlighted' (although highlighting will be disabled)
    /// </summary>
    public class DirectoryViewModelTreeViewItem : CheckableTreeViewItemViewModel
    {
    
        private String _path;

        public String Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
                NotifyOfPropertyChange(() => Path);
            }
        }

        private String _name;

        public String Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        private BindableCollection<DirectoryViewModelTreeViewItem> _children;
        private static DirectoryViewModelTreeViewItem DummyChild = new DirectoryViewModelTreeViewItem() { Name = "Dummy", Path = "Dummy" };

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger(); 

        public DirectoryViewModelTreeViewItem()
        {
            _children = new BindableCollection<DirectoryViewModelTreeViewItem>();
            _children.Add(DummyChild);
            _isChecked = false;

            Children = new System.Windows.Data.ListCollectionView(_children);
        }

        public override bool ContainsDummy()
        {
            return _children.Contains(DummyChild);
        }
       
        public override void RefreshChildren()
        {
            _children.Clear(); 
            if (!String.IsNullOrEmpty(Path) && Grep.Net.Model.Utilities.DirectoryExists(Path))
            {
                try
                {
                    FileSystemEnumerable fse = new FileSystemEnumerable(new DirectoryInfo(this.Path), "*", SearchOption.TopDirectoryOnly);
                    foreach (DirectoryInfo di in fse.OfType<DirectoryInfo>())
                    {
                        try
                        {
                            DirectoryViewModelTreeViewItem newItem = new DirectoryViewModelTreeViewItem()
                            {
                                Path = di.FullName,
                                Name = di.Name,
                                Parent = this
                            };
                            if (Grep.Net.Model.GTApplication.Instance.Settings.Recurse)
                            {
                                newItem._isChecked = this._isChecked;
                            }
                            _children.Add(newItem);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e);
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
            }
        }

      
    }
}