using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Grep.Net.Data;
using Grep.Net.Entities;
using Grep.Net.Model;
using Grep.Net.WPF.Client.Data;
using Grep.Net.WPF.Client.ViewModels.Entities;
using Grep.Net.WPF.Client.Commands;
using System.Windows.Data;
using Grep.Net.WPF.Client.Services;
using Grep.Net.WPF.Client.Interfaces;
using System.Text.RegularExpressions;


namespace Grep.Net.WPF.Client.ViewModels.CrumbListView
{
    public class PatternPackResultViewModel
    {
        public PatternPackageViewModel PatternPackage { get; set; }

        public GrepResultViewModel Result { get; set; }

        public int Count { get { return Result.MatchInfos.Cast<MatchInfo>().Where(x =>  (x.Pattern != null) && x.Pattern.PatternPackageId == PatternPackage.Entity.Id).Count(); } }

    }

    public class GrepResultCrumbListViewViewModel : CrumbNavigationListViewBaseViewModel
    {
        private IEnumerable<MatchInfo> _selectedMatchInfos;

        public IEnumerable<MatchInfo> SelectedMatchInfos
        {
            get
            {
                return _selectedMatchInfos;
            }
            set
            {
                _selectedMatchInfos = value;
                NotifyOfPropertyChange(() => SelectedMatchInfos);
            }
        }

        public IEnumerable<Object> SelectedItems { get; set; }

        public MatchInfoEditorViewModel MatchInfoEditorViewModel { get; set; }

        IDataService DataService { get; set; }

        RootViewModel RootViewModel { get; set; }
        public GrepResultCrumbListViewViewModel(RootViewModel rvm, IDataService dataService) : base()
        {
            DataService = dataService;
            RootViewModel = rvm;
            MatchInfoEditorViewModel = new ViewModels.MatchInfoEditorViewModel(rvm);
            GrepResultsCrumb grCrumb = new GrepResultsCrumb(DataService)
            {
                Display = "Results"
            };

            Crumbs.Add(grCrumb);

            Navigate = new DelegateCommand((dataContext) =>
            {
                if (dataContext is GrepResultViewModel)
                {
                    GrepResultViewModel gr = dataContext as GrepResultViewModel;
                   
                    //Set it to 60 if it's too long... 
                    //TODO add this to options at some point
                    String s = gr.Entity.GrepContext.RootPath;
                    if (s.Count() > 60)
                    {
                        s = s.Substring(0, 60);
                    }

                    var singlePackage = gr.MatchInfos.Cast<MatchInfo>().Select(x => x.Pattern.PatternPackageId).Distinct().Count() == 1;
                    if (singlePackage)
                    {
                        //If it's only 1 package, we just skip to the matchInfos. (This is also a hack to avoid the PatternPackage for "Searching" since there is only ever 1 package ;). 
                        Crumbs.Add(new MatchInfosCrumb(grCrumb, Guid.Empty, gr.Entity.MatchInfos)
                        {
                            Owner = this
                        });
                    }
                    else
                    {
                        //TODO:  Add a flow here that if there is only 1 result set (Search/1 package, etc) skip the PatternPackageCrumb and go straight to the MatchInfosCrumb
                        //Grep Results
                        Crumbs.Add(new PatternPackageCrumb(gr, DataService)
                        {
                            Owner = this,
                            Display = s
                        });
                    }
                   
                }
                else if (dataContext is PatternPackResultViewModel)
                {
                    //Match Info "PatternPackage" title
                    //We know the GrepResult is on the top of the stack. 
                    PatternPackageCrumb crumb = CurrentCrumb as PatternPackageCrumb;
                    PatternPackResultViewModel pp = dataContext as PatternPackResultViewModel;
                    if (crumb != null)
                    {
                        Crumbs.Add(new MatchInfosCrumb(crumb, pp.PatternPackage.Entity.Id,  crumb.GrepResult.Entity.MatchInfos)// new BindableCollection<MatchInfo>(crumb.GrepResult.MatchInfos.Cast<MatchInfo>().Where(x => x.Pattern.PatternPackageId == pp.PatternPackage.Entity.Id)))
                        {
                            Owner = this,
                            Display = pp.PatternPackage.Name,
                            Parent = crumb
                        });
                    }
                }
            });
        }
        public void GoBack(CrumbListViewModel crumb)
        {
            if (crumb == this.CurrentCrumb)
            {
                return;
            }

            //This loop will prevent it from ever going back past the 0 index in crumbs.

            for (int i = Crumbs.Count - 1; i > 0; i--)
            {
                if (!Crumbs[i].Display.Equals(crumb.Display))
                {
                    Crumbs.RemoveAt(i);
                }
            }
        }
        public ICommand Navigate { get; set; }
        public void OnNavigate(object param)
        {
            if (Navigate != null &&
                Navigate.CanExecute(param))
            {
                Navigate.Execute(param);
            }
        }
     
        /// <summary>
        /// This gets fired when selectedItems change on the ListView datatemplate. This should be moved also to the Crumb. But Whateva! 
        /// </summary>
        /// <param name="selectedItems"></param>
        public void SelectedItemChanged(IEnumerable<Object> selectedItems)
        {
            if (selectedItems == null)
                return;
            SelectedItems = selectedItems;
            SelectedMatchInfos = selectedItems.Where(x => x is MatchInfo).Cast<MatchInfo>();
            if (SelectedMatchInfos.Count() > 0)
            {
                MatchInfo mivm = (MatchInfo)SelectedMatchInfos.ToList()[0];
                this.MatchInfoEditorViewModel.MatchInfo = mivm;
            }
            else
            {
                this.MatchInfoEditorViewModel.MatchInfo = null;
            }
        }

        #region Commands
        /// <summary>
        /// Copies the MatchInfo details to the clipboard. Useful for writing up bugs or sharing results. 
        /// </summary>
        /// <param name="param"></param>
        public void ToClipboard(string param)
        {
            StringBuilder sb = new StringBuilder();
            foreach (MatchInfo minfo in this.SelectedMatchInfos)
            {
                switch (param)
                {
                    case "FileName":
                        sb.AppendLine(String.Format("{0}", minfo.FileInfo.Name));
                        break;
                    case "FilePath":
                        sb.AppendLine(String.Format("{0}", minfo.FileInfo.FullName));
                        break;
                    case "FilePathLine":
                        sb.AppendLine(String.Format("{0}:{1}", minfo.FileInfo.FullName, minfo.LineNumber));
                        break;
                    case "FilePathLinePattern":
                        sb.AppendLine(String.Format("{0}:{1} - {2}", minfo.FileInfo.FullName, minfo.LineNumber, minfo.Pattern.PatternStr));
                        break;
                }
            }
            Clipboard.SetText(sb.ToString());
        }

        /// <summary>
        /// Copies the MatchInfo details to the clipboard. Useful for writing up bugs or sharing results. 
        /// </summary>
        /// <param name="param"></param>
        public void Uniquify(string param)
        {

            if (!(CurrentCrumb is MatchInfosCrumb))
            {
                //This command only works when the current context is MatchInfosCrumb
                return;
            }
            MatchInfosCrumb crumb = CurrentCrumb as MatchInfosCrumb;
            if(crumb == null)
            {
                //Sanity
                return;
            }

            switch (param)
            {
                case "File":
                    //Get the unique FileNames
                    IList<String> fNames = crumb.ItemsSource.Cast<MatchInfo>().Select(x => x.FileInfo.FullName).Distinct().ToList();
                    Crumbs.Add(new UniqueCrumb()
                    {
                        Display = "Unique - FileNames",
                        ItemsSource = new ListCollectionView(fNames as System.Collections.IList)
                    });
                    break;
                case "Match":
                    IList<String> matchStrs = crumb.ItemsSource.Cast<MatchInfo>().Select(x => x.Match).Distinct().ToList();
                    Crumbs.Add(new UniqueCrumb()
                    {
                        Display = "Unique - Matchs",
                        ItemsSource = new ListCollectionView(matchStrs as System.Collections.IList)
                    });
                    break;
                case "Context":
                    IList<String> contextStrings = crumb.ItemsSource.Cast<MatchInfo>().Select(x => x.Context).Distinct().ToList();
                    Crumbs.Add(new UniqueCrumb()
                    {
                        Display = "Unique - Contexts",
                        ItemsSource = new ListCollectionView(contextStrings as System.Collections.IList)
                    });
                    break;
                case "Line":
                    IList<String> lineStrings = crumb.ItemsSource.Cast<MatchInfo>().Select(x => x.Line).Distinct().ToList();
                    Crumbs.Add(new UniqueCrumb()
                    {
                        Display = "Unique - Line",
                        ItemsSource = new ListCollectionView(lineStrings as System.Collections.IList)
                    });
                    break;
                default:
                    break;
            }
            
        }


        private String GetSaveFileName()
        {
            Microsoft.Win32.SaveFileDialog ofd = new Microsoft.Win32.SaveFileDialog();

            if (ofd.ShowDialog() == true)
            {
                return ofd.FileName;
            }
            return "";
        }
        public void Import(String from){
            
            //TODO: Add more options for import potentially??
            switch (from.ToLower())
            {
                case "xml":
                    ImportResults();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Export's the data from the CurrentCrumb
        /// </summary>
        /// <param name="exportAs"></param>
        public void ExportAs(String exportAs)
        {
            if (SelectedItems == null && 
                SelectedItems.Count() > 0)
            {
                //TODO: Do something
                return;
            }

            String fname = GetSaveFileName();

            if (String.IsNullOrEmpty(fname))
            {
                MessageBox.Show("Invalid (Blank or Null) FileName");
                return;
            }
            var entities = SelectedItems.Select<Object, IEntity>(x =>
            {
                if(x is PatternPackageViewModel){
                    return (x as PatternPackageViewModel).Entity;
                }
                else if (x is GrepResultViewModel) {
                    return (x as GrepResultViewModel).Entity;
                
                }else
                {
                    return (x as MatchInfoViewModel).Entity;
                }//x is MatchInfoViewModel)
                
            }).Where(x=> x != null);

            EntityContainer container = new EntityContainer();

            container.AddRange(entities.ToArray());
            switch (exportAs.ToLower())
            {
                case "xml":
                    SerializationHelper.SerializeIntoXmlFile<EntityContainer>(fname, container);
                    
                    break;
                case "tsv":
                    break;
                case "csv":
                    break;
                case "json":
                    File.WriteAllText(fname, SerializationHelper.SerializeAsJSON(container));
                    break;
                case "text":
                    break;
            }  
        }
        
        /// <summary>
        /// This method should get moved to the Crumb itself, however I'm too lazy to fix this atm. 
        /// </summary>
        public void RemoveSelected()
        {
            //We must be at the MatchInfoCrumb
            //TODO: Fix this to be modeled slightly better.. 
            CrumbListViewModel currentCrumb = CurrentCrumb as CrumbListViewModel;
            if (currentCrumb != null)
            {
                foreach (var item in this.SelectedItems.ToList())
                {
                    if (item is PatternPackageViewModel && currentCrumb is PatternPackageCrumb)
                    {
                        PatternPackageCrumb ppCrumb = currentCrumb as PatternPackageCrumb;
                        List<MatchInfo> toRemove = ppCrumb.GrepResult.Entity.MatchInfos.Where(x => x.Pattern.PatternPackageId == (item as PatternPackageViewModel).Entity.Id).ToList();
                        foreach (MatchInfo mi in toRemove)
                        {
                            ppCrumb.GrepResult.MatchInfos.Remove(mi);
                        }
                        ppCrumb.GrepResult.MatchInfos.CommitEdit();
                    }
                    if (item is MatchInfo)
                    {
                        var mi = item as MatchInfo;
                        var grepResult = DataService.GrepResultService.Get(mi.GrepResultId);
                        grepResult.Entity.MatchInfos.Remove(mi);
                    }
                    if (item is GrepResultViewModel)
                    {
                        DataService.GrepResultService.Remove((item as GrepResultViewModel).Entity);
                    }
                    
                    currentCrumb.ItemsSource.Remove(item);

                }
                currentCrumb.ItemsSource.CommitEdit();
            }
        }


        public void RemoveSelectedFileName()
        {
            //We must be at the MatchInfoCrumb
            //TODO: Fix this to be modeled slightly better.. 
            MatchInfosCrumb currentCrumb = CurrentCrumb as MatchInfosCrumb;
            if (currentCrumb != null)
            {
                foreach (var item in this.SelectedItems.ToList())
                {
                    if (item is MatchInfo)
                    {
                        var mi = item as MatchInfo;
                        var grepResult = DataService.GrepResultService.Get(mi.GrepResultId);


                        var toRemove = grepResult.Entity.MatchInfos.Where(x => x.FileInfo.Name == mi.FileInfo.Name &&
                            (x.Pattern.PatternPackageId == currentCrumb.PatternPackageId || currentCrumb.PatternPackageId == Guid.Empty)).ToList();

                        foreach (var removeMe in toRemove)
                        {
                            if (grepResult.Entity.MatchInfos.Contains(removeMe))
                            {
                                grepResult.Entity.MatchInfos.Remove(removeMe);
                            }
                        }
                    }
                }
                currentCrumb.RefreshChildren();
            }
        }

        public void RemoveFromPattern()
        {
            //We must be at the MatchInfoCrumb
            //TODO: Fix this to be modeled slightly better.. 
            MatchInfosCrumb currentCrumb = CurrentCrumb as MatchInfosCrumb;
            if (currentCrumb != null)
            {

                //Show a input dialog. 

                InputDialogViewModel iDialog = new InputDialogViewModel();
                iDialog.Question = "Enter regex pattern to remove";

                if (!(GTWindowManager.Instance.ShowDialog(iDialog, 175, 325) == true) || 
                     String.IsNullOrEmpty(iDialog.Input))
                {
                    return;
                }



                foreach (var item in this.SelectedItems.ToList())
                {
                    if (item is MatchInfo)
                    {
                        var mi = item as MatchInfo;
                        var grepResult = DataService.GrepResultService.Get(mi.GrepResultId);


                        var toRemove = grepResult.Entity.MatchInfos.Where(x =>  Regex.IsMatch(x.FileInfo.Name, iDialog.Input) &&
                            (x.Pattern.PatternPackageId == currentCrumb.PatternPackageId || currentCrumb.PatternPackageId == Guid.Empty)).ToList();

                        foreach (var removeMe in toRemove)
                        {
                            if (grepResult.Entity.MatchInfos.Contains(removeMe))
                            {
                                grepResult.Entity.MatchInfos.Remove(removeMe);
                            }
                        }
                    }
                }
                currentCrumb.RefreshChildren();

            }
        }


        public void RemoveSelectedFileNameOnPath()
        {
            //We must be at the MatchInfoCrumb
            //TODO: Fix this to be modeled slightly better.. 
            MatchInfosCrumb currentCrumb = CurrentCrumb as MatchInfosCrumb;
            if (currentCrumb != null)
            {
                foreach (var item in this.SelectedItems.ToList())
                {
                    if (item is MatchInfo)
                    {
                        var mi = item as MatchInfo;
                        var grepResult = DataService.GrepResultService.Get(mi.GrepResultId);


                        var toRemove = grepResult.Entity.MatchInfos.Where(x => x.FileInfo.FullName == mi.FileInfo.FullName && 
                            (x.Pattern.PatternPackageId == currentCrumb.PatternPackageId || currentCrumb.PatternPackageId == Guid.Empty)).ToList();

                        foreach (var removeMe in toRemove)
                        {
                            if (grepResult.Entity.MatchInfos.Contains(removeMe)) {
                                grepResult.Entity.MatchInfos.Remove(removeMe);
                            }
                        }
                    }
                }
                currentCrumb.RefreshChildren();

            }
        }

        public void RemoveSelectedFileExtension()
        {
            //We must be at the MatchInfoCrumb
            //TODO: Fix this to be modeled slightly better.. 
            MatchInfosCrumb currentCrumb = CurrentCrumb as MatchInfosCrumb;
            if (currentCrumb != null)
            {
                foreach (var item in this.SelectedItems.ToList())
                {
                    if (item is MatchInfo)
                    {
                        var mi = item as MatchInfo;
                        var grepResult = DataService.GrepResultService.Get(mi.GrepResultId);


                        var toRemove = grepResult.Entity.MatchInfos.Where(x => {
                            return x.FileInfo.FileExtension.Equals(mi.FileInfo.FileExtension) && (x.Pattern.PatternPackageId == currentCrumb.PatternPackageId || currentCrumb.PatternPackageId == Guid.Empty);
                        }).ToList();

                        foreach (var removeMe in toRemove)
                        {
                            if (grepResult.Entity.MatchInfos.Contains(removeMe))
                            {
                                grepResult.Entity.MatchInfos.Remove(removeMe);
                            }
                        }
                    }
                }
                currentCrumb.RefreshChildren();

            }

        }

        public void ImportResults()
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".xml"; // Default file extension
            ofd.Filter = "Xml documents (.xml)|*.xml"; // Filter files by extension 
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    GrepResult gr = SerializationHelper.DeserializeXmlFromFile<GrepResult>(ofd.FileName);
                    DataService.GrepResultService.Add(gr);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        public void ShowResultInWin()
        {
            if (this.SelectedMatchInfos != null && this.SelectedMatchInfos.Count() > 0)
            {
                MatchInfoEditorViewModel mievm = new MatchInfoEditorViewModel(RootViewModel) { MatchInfo = this.SelectedMatchInfos.ToList()[0] };

                RootViewModel.Documents.Add(mievm);
            }
        }
        #endregion

    }

    public class GrepResultsCrumb : CrumbListViewModel
    {
        BindableCollection<GrepResultViewModel> Results { get; set; }
        IDataService DataService { get; set; }
        public GrepResultsCrumb(IDataService dataService)
        {
            DataService = dataService;
            Results = new BindableCollection<GrepResultViewModel>();
            DataService.GrepResultService.OneWaySyncTo(Results);

            this.ItemsSource = new ListCollectionView(Results);
        }
    }
    public class PatternPackageCrumb : CrumbListViewModel
    {

      

        public GrepResultViewModel GrepResult;
        public IDataService DataService { get; set; }

        public BindableCollection<PatternPackResultViewModel> PatternPackages { get; set; }

        public int Count { get { return PatternPackages.Count; } }

        public PatternPackageCrumb(GrepResultViewModel gr, IDataService dataService)
        {
            DataService = dataService;
            GrepResult = gr;
            PatternPackages = new BindableCollection<PatternPackResultViewModel>();

            var matches = GrepResult.MatchInfos.Cast<MatchInfo>();
             var patternPackageIds = matches.Where(x=>x.Pattern.PatternPackageId != Guid.Empty).Select(x => x.Pattern.PatternPackageId);
             var patternPackages = patternPackageIds.Select(x => DataService.PatternPackageService.Get(x)).Distinct().ToList();
             foreach (var patternPackage in patternPackages)
             {
                 PatternPackResultViewModel pprv = new PatternPackResultViewModel();
                 pprv.PatternPackage = patternPackage;
                 pprv.Result = gr;
                 PatternPackages.Add(pprv);
             }

             ItemsSource = new ListCollectionView(PatternPackages as System.Collections.IList);
                    
        }

    }
    public class MatchInfosCrumb : CrumbListViewModel
    {
        //public SyncedBindableCollection<MatchInfo, MatchInfoViewModel> _matchInfos { get; set; }

        private IEnumerable<MatchInfo> _matchInfos;


        public CrumbListViewModel Parent { get; set; }
        public Guid PatternPackageId { get; set; }

        private String _filterStr;
        public String FilterStr
        {
            get
            {
                return _filterStr;
            }
            set
            {
                _filterStr = value;
                ItemsSource.Refresh();
                NotifyOfPropertyChange(() => FilterStr);
            }
        }

        public MatchInfosCrumb(CrumbListViewModel parent, Guid patternPackageId, IEnumerable<MatchInfo> matchInfos)
        {

            Display = "";
            _matchInfos = matchInfos;
            PatternPackageId = patternPackageId;
            RefreshChildren(); 
        }

        public void RefreshChildren()
        {

            ItemsSource = new ListCollectionView(new BindableCollection<MatchInfo>(_matchInfos.Where(x=> x.Pattern.PatternPackageId == PatternPackageId || PatternPackageId == Guid.Empty)));
            ItemsSource.Filter = new Predicate<object>((x) =>
            {
                if (!(x is MatchInfo) || String.IsNullOrEmpty(FilterStr))
                    return true;

                MatchInfo mi = x as MatchInfo;

                if (mi != null)
                {
                    if (mi.Context.Contains(FilterStr))
                        return true;

                    if (mi.FileInfo.FullName.Contains(FilterStr))
                        return true;
                }

                return false;
            });
        }
        public void RemoveSelected(IEnumerable<Object> selectedItems)
        {
        }

        public void RemoveSelected()
        {
        }
    }
    public class UniqueCrumb : CrumbListViewModel
    {
        
        public UniqueCrumb()
        {

        }

        public void ToTextFile()
        {
            Microsoft.Win32.SaveFileDialog ofd = new Microsoft.Win32.SaveFileDialog();

            if (ofd.ShowDialog() == true)
            {
                string fname = ofd.FileName;

                using (StreamWriter sw = new StreamWriter(fname))
                {
                    foreach (object o in ItemsSource)
                    {
                        sw.WriteLine(o.ToString());
                    }
                    sw.Flush();
                }
            }
        }
    }

}