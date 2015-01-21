using System;
using System.Linq;
using System.Windows.Data;
using Caliburn.Micro;
using Grep.Net.Model.Properties;

namespace Grep.Net.WPF.Client.ViewModels.Entities
{
    public class SettingsViewModel : PropertyChangedBase, Grep.Net.WPF.Client.ViewModels.Interfaces.ISettingsManager
    {
        private Settings _settings;

        public Settings Settings
        {
            get
            {
                return _settings;
            }
            set
            {
                _settings = value;
                if (value != null)
                {
                    PopulateExclusions();
                    PopulateShortCuts(); 
                }
                NotifyOfPropertyChange(() => Settings); 
            }
        }

        public string PatternPackagesDir
        {
            get
            {
                return Settings.PatternPackagesDir;
            }
            set
            {
                Settings.PatternPackagesDir = value;
                NotifyOfPropertyChange(() => PatternPackagesDir); 
            }
        }

        public string FileTypeDefinitionsDir
        {
            get
            {
                return Settings.FileTypeDefinitionsDir;
            }
            set
            {
                Settings.FileTypeDefinitionsDir = value;
                NotifyOfPropertyChange(() => FileTypeDefinitionsDir);
            }
        }

        public string ClassificationsDir
        {
            get
            {
                return Settings.ClassificationsDir;
            }
            set
            {
                Settings.ClassificationsDir = value;
                NotifyOfPropertyChange(() => ClassificationsDir);
            }
        }

        public string TemplatesDir
        {
            get
            {
                return Settings.TemplatesDir;
            }
            set
            {
                Settings.TemplatesDir = value;
                NotifyOfPropertyChange(() => TemplatesDir);
            }
        }

        public bool AutoSave
        {
            get
            {
                return Settings.AutoSave;
            }
            set
            {
                Settings.AutoSave = value;
                NotifyOfPropertyChange(() => AutoSave);
            }
        }

        public bool SaveLayout
        {
            get
            {
                return Settings.SaveLayout;
            }
            set
            {
                Settings.SaveLayout = value;
                NotifyOfPropertyChange(() => SaveLayout);
            }
        }

        public bool Recurse
        {
            get
            {
                return Settings.Recurse;
            }
            set
            {
                Settings.Recurse = value;
                NotifyOfPropertyChange(() => Recurse);
            }
        }

        public int LinesBefore
        {
            get
            {
                return Settings.LinesBefore;
            }
            set
            {
                Settings.LinesBefore = value;
                NotifyOfPropertyChange(() => LinesBefore);
            }
        }

        public int LinesAfter
        {
            get
            {
                return Settings.LinesAfter;
            }
            set
            {
                Settings.LinesAfter = value;
                NotifyOfPropertyChange(() => LinesAfter);
            }
        }

        public int MaxContextSize
        {
            get
            {
                return Settings.MaxContextSize;
            }
            set
            {
                Settings.MaxContextSize = value;
                NotifyOfPropertyChange(() => MaxContextSize);
            }
        }

        public int MaxLineSize
        {
            get
            {
                return Settings.MaxLineSize;
            }
            set
            {
                Settings.MaxLineSize = value;
                NotifyOfPropertyChange(() => MaxLineSize);
            }
        }

        public int GrepThreadsMax
        {
            get
            {
                return Settings.GrepThreadsMax;
            }
            set
            {
                Settings.GrepThreadsMax = value;
                NotifyOfPropertyChange(() => GrepThreadsMax);
            }
        }

        public int PoolRunspaceMin
        {
            get
            {
                return Settings.PoolRunspaceMin;
            }
            set
            {
                Settings.PoolRunspaceMin = value;
                NotifyOfPropertyChange(() => PoolRunspaceMin);
            }
        }

        public int PoolRunspaceMax
        {
            get
            {
                return Settings.PoolRunspaceMax;
            }
            set
            {
                Settings.PoolRunspaceMax = value;
                NotifyOfPropertyChange(() => PoolRunspaceMax);
            }
        }

        public BindableCollection<String> Exclusions { get; set;  }

        public BindableCollection<String> PathShortCuts { get; set;  }


        public SettingsViewModel(Settings settings)
        {
            PathShortCuts = new BindableCollection<String>();
            Exclusions = new BindableCollection<String>();

            //this needs to be called after the initilizing members. Otherwise will cause null exception since populate is called. 
            Settings = settings;

        }

        private void PopulateExclusions()
        {
            if (Exclusions != null)
            {
                Exclusions.Clear();
                Exclusions.AddRange(Settings.Exclusions.Cast<string>().ToList());
            }
        
        }

        private void PopulateShortCuts()
        {
            if (PathShortCuts != null)
            {
                PathShortCuts.Clear();
                PathShortCuts.AddRange(Settings.PathShortCuts.Cast<string>().ToList());
            }
            
        }

        private void UpdateExclusions()
        {
            if (_settings != null)
            {
                _settings.Exclusions.Clear();
                _settings.Exclusions.AddRange(Exclusions.ToArray());
            }
           
        }

        private void UpdateShortCuts()
        {
            if (_settings != null)
            {
                _settings.PathShortCuts.Clear();
                _settings.PathShortCuts.AddRange(PathShortCuts.ToArray());
            }
          
        }

        public void Save()
        {
            UpdateExclusions();
            UpdateShortCuts();
            if (_settings != null)
            {
                _settings.Save();
            }
           
        }


    }
}