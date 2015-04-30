using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using Grep.Net.Entities;
using Grep.Net.Model;
using Grep.Net.WPF.Client.ViewModels.CrumbListView;
using Grep.Net.WPF.Client.ViewModels.Entities;
using Grep.Net.WPF.Client.Interfaces;
using Grep.Net.WPF.Client.Services;
using Grep.Net.Data;
using System.Threading.Tasks;
using System.ComponentModel.Composition;


namespace Grep.Net.WPF.Client.ViewModels
{
        [Export(typeof(IRoot))]
    public class RootViewModel : Screen, IRoot
    {
        #region Singleton
        
        private static RootViewModel _instance;
        
        public static RootViewModel Instance
        {
            get
            {
                lock (_lok)
                {
                    if (_instance == null)
                    {
                        _instance = new RootViewModel();
                    }
                }
                return _instance;
            }
        }
        
        private static object _lok = new object();
        
        #endregion
        
        public object _activeViewModel;
        
        public object ActiveViewModel
        {
            get
            {
                return _activeViewModel;
            }
            set
            {
                _activeViewModel = value;
                NotifyOfPropertyChange(() => ActiveViewModel);
            }
        }
        
        public SelectItemsViewModel SelectItemsViewModel { get; set; }
        
        public DetailsViewModel DetailsViewModel { get; set; }
        
        public GrepResultCrumbListViewViewModel GrepResultCrumbListViewViewModel { get; set; }
        
        public String Path { get; set; }
        
        public ObservableCollection<PropertyChangedBase> Documents { get; set; }
     
        public NLogViewModel NLogViewModel { get; set; }

        public SettingsViewModel SettingsViewModel { get; set; }
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public IDataService DataService { get; set; }
        public GrepService GrepService { get; set; }

        public GrepContextStatusViewModel GrepContextStatusViewModel { get; set; }
        private RootViewModel()
        {

            Documents = new ObservableCollection<PropertyChangedBase>();
            //Setup logging. 
            NLogViewModel = new NLogViewModel();
            //log something
            
            logger.Info("Log First Message");
            
            

            try
            {
                GTApplication app = GTApplication.Instance;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw e;
            }

            DataService = new DataService();

            GrepService = new GrepService(GTApplication.Instance.GrepService, DataService);

            SettingsViewModel = new Entities.SettingsViewModel(GTApplication.Instance.Settings);

            //Add the main control panel. 
            SelectItemsViewModel = new SelectItemsViewModel(this.SettingsViewModel, DataService);

            GrepContextStatusViewModel = new ViewModels.GrepContextStatusViewModel(DataService);
          
            GrepResultCrumbListViewViewModel = new GrepResultCrumbListViewViewModel(DataService);
            GrepResultCrumbListViewViewModel.MatchInfoEditorViewModel.Closeable = false;
            GrepResultCrumbListViewViewModel.MatchInfoEditorViewModel.Name = "Editor";
            GrepResultCrumbListViewViewModel.MatchInfoEditorViewModel.Editor.MaxFileSize = SettingsViewModel.Settings.DisplayMaxFileSize;
            Documents.Add(this.GrepResultCrumbListViewViewModel.MatchInfoEditorViewModel);
            
            DetailsViewModel = new DetailsViewModel();
            
            SelectItemsViewModel.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler((sender, args) =>
            {
                if (args.PropertyName.Equals("SelectedItem"))
                {
                    this.DetailsViewModel.ViewModel = this.SelectItemsViewModel.SelectedItem;
                }
            });
            
            DetailsViewModel.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler((sender, args) =>
            {
                if (args.PropertyName.Equals("ViewModel"))
                {
                    this.ActiveViewModel = this.DetailsViewModel;
                }
            });
            
            GrepResultCrumbListViewViewModel.MatchInfoEditorViewModel.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler((sender, args) =>
            {
                if (args.PropertyName.Equals("MatchInfo"))
                {
                    ActiveViewModel = GrepResultCrumbListViewViewModel.MatchInfoEditorViewModel;
                }
            });
            Documents.Add(DetailsViewModel);
            Documents.Add(NLogViewModel);
            Documents.Add(GrepContextStatusViewModel);
        }
        
        public void SelectCategories()
        {
            SelectItemsViewModel sivm = new SelectItemsViewModel(this.SettingsViewModel, DataService);
            GTWindowManager.Instance.ShowWindow(sivm, 300, 300); 
            // winMgr.ShowWindow(sivm); 
        }
        
        public void SaveAll()
        {
            throw new NotImplementedException(); 
        }
        
        /// <summary>
        /// Todo move all this logic into the model applicaiton. No point in being here. 
        /// </summary>
        public async void Start()
        {
          
            var patterns = DataService.PatternPackageService.GetAll().Select(x=>x.Entity).Where(x => x.IsEnabled == true).ToList();
            if (patterns.Count() < 1)
            {
                MessageBox.Show("No PatternPackages Selected..");
                return;
            }
            
            if (patterns.SelectMany(x => x.Patterns).Count() < 1)
            {
                MessageBox.Show("No Patterns Selected..");
                return;
            }
            
            //Get all the selected directories.. 
            HashSet<string> selectedDirs = new HashSet<string>(this.SelectItemsViewModel.DirectoryExplorer.GetSelectedDirectories());
            
            if (selectedDirs.Count < 1)
            {
                MessageBox.Show("Try Selecting a Directory First...");
                return;
            }
            
            var fileTypes = DataService.FileTypeDefinitionService.GetAll().Select(x=>x.Entity).Where(x => x.IsEnabled).SelectMany(x => x.FileExtensions.Where(y => y.IsEnabled)).ToList();
            if (fileTypes.Count() < 1)
            {
                logger.Info("No file types selected, defaulting to *.*");
                fileTypes.Add(new FileExtension() { Extension = "*.*" });
            }
            
            foreach (String dir in selectedDirs)
            { 
                try
                {
                    if (!String.IsNullOrWhiteSpace(dir))
                    {
                        this.ActiveViewModel = GrepContextStatusViewModel;
                        await GrepService.StartGrep(dir, patterns, fileTypes);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
        public void ImportResult(){
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".xml"; // Default file extension
            ofd.Filter = "Xml documents (.xml)|*.xml"; // Filter files by extension 
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    EntityContainer gr = SerializationHelper.DeserializeXmlFromFile<EntityContainer>(ofd.FileName);
                    DataService.GrepResultService.Add(gr[0] as GrepResult);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
        
        public async void Search(String searchRegexText)
        {
          
            PatternPackage pp = new PatternPackage();
            pp.Patterns.Add(new Pattern() { PatternStr = searchRegexText,
                                            PatternPackageId = pp.Id});
            pp.Name = "Search (" + searchRegexText + ")";
            var patterns = new List<PatternPackage>() { pp };
            
            //Get all the selected directories.. 
            HashSet<string> selectedDirs = new HashSet<string>(this.SelectItemsViewModel.DirectoryExplorer.GetSelectedDirectories());
            
            if (selectedDirs.Count < 1)
            {
                MessageBox.Show("Try Selecting a Directory First...");
                return;
            }
            
            var fileTypes = DataService.FileTypeDefinitionService.GetAll().Select(x=>x.Entity).Where(x => x.IsEnabled).SelectMany(x => x.FileExtensions.Where(y => y.IsEnabled)).ToList();
            if (fileTypes.Count() < 1)
            {
                logger.Info("No file types selected, defaulting to *.*");
                //TODO: Default to *.*. 
                fileTypes.Add(new FileExtension() { Extension = "*.*" });
            }
         
            foreach (String dir in selectedDirs)
            { 
                try
                {
                    if (!String.IsNullOrWhiteSpace(dir))
                    {
                        this.ActiveViewModel = GrepContextStatusViewModel;
                        await GrepService.StartGrep(dir, patterns, fileTypes);

                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
        
        public void ResetLayout()
        {
            var dm = GetDockingManager(); 
        }
        
        public void ShowOptions()
        {
            OptionsViewModel ovm = new OptionsViewModel();
            
            GTWindowManager.Instance.ShowDialog(ovm, 450, 350); 
        }
        
        public void Exit()
        {
            //Save latout?
            //Need to try to get the view. 
            Grep.Net.WPF.Client.Views.RootView rv = this.GetView(null) as Grep.Net.WPF.Client.Views.RootView;
            
            Application.Current.Shutdown(); 
        }
        
      
       
        public void ShowManageTypes()
        {
            ManageMasterTreeViewViewModel mmtrv = new ManageMasterTreeViewViewModel(DataService);
            Documents.Add(mmtrv);
            ActiveViewModel = mmtrv;
        }
        
        #region CreateTypes
        
        public void CreatePatternPackageTextFile()
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            try
            {
                if (ofd.ShowDialog() == true)
                {
                    string fileName = ofd.FileName;
                    if (!String.IsNullOrEmpty(fileName) &&
                        File.Exists(fileName))
                    {
                        StreamReader sr = new StreamReader(fileName);
                        PatternPackage pp = PatternPackage.CreateFromFile(sr.ReadToEnd());
                        PatternPackageViewModel ppvm = new PatternPackageViewModel(pp);
                        string fname = System.IO.Path.GetFileNameWithoutExtension(fileName);
                        pp.Name = fname; 
                        if (GTWindowManager.Instance.ShowOkCanelDialog(ppvm, 300, 300) == true)
                        {
                            DataService.PatternPackageService.Add(pp);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        
        internal Xceed.Wpf.AvalonDock.DockingManager GetDockingManager()
        {
            Grep.Net.WPF.Client.Views.RootView rv = (Grep.Net.WPF.Client.Views.RootView)this.GetView(null);
            if (rv != null)
            {
                var dm = rv.dockingManager;
                return dm;
            }
            
            return null;
        }
        
        public void CreatePatternPackageXmlFile()
        {
        }
        
        public void CreateFileDefTextFile()
        {
        }
        
        public void CreateFileDefXmlFile()
        {
        }
        
        #endregion
    }
    
    /// <summary>
    /// Style selector for the DockingManager. This will allow us to 
    /// </summary>
    internal class LayoutItemStyleSelector : StyleSelector
    {
        public Style StaticDocumentStyle { get; set; }

        public Style DocumentStyle { get; set; }
        
        public Style AnchorableStyle { get; set; }
        
        public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            if (item is MatchInfoEditorViewModel ||
                item is DetailsViewModel ||
                item is NLogViewModel ||
                item is GrepContextStatusViewModel)
                return StaticDocumentStyle;

            if (container is Xceed.Wpf.AvalonDock.Controls.LayoutAnchorableItem)
                return AnchorableStyle;
            
            if (container is Xceed.Wpf.AvalonDock.Controls.LayoutDocumentItem)
                return DocumentStyle;

           
            
            return DocumentStyle;
        }
    }
    
    public class DockingTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PropertyChangedTemplate { get; set; }
        
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            
            if (element != null && item != null && item is PropertyChangedBase)
            {
                return PropertyChangedTemplate;  
            }
            return null;
        }
        
        public void LayoutChanged(ActionExecutionContext context)
        {
        }
    }
}