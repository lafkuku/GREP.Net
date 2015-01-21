using System;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Grep.Net.Model;
using Grep.Net.WPF.Client.Commands;
using Grep.Net.WPF.Client.ViewModels.Entities;

namespace Grep.Net.WPF.Client.ViewModels
{
    public class OptionsViewModel : Screen
    {
        private SettingsViewModel _settings;

        public SettingsViewModel Settings
        {
            get
            {
                return _settings;
            }
            set
            {
                _settings = value;
                NotifyOfPropertyChange(() => Settings); 
            }
        }
        
        public bool ReuseCodeWindows { get; set; }

        public string NewExclusion { get; set; }

        public String SelectedPath { get; set; }

        public ICommand AddExclusionCommand { get; set; }

        public ICommand RemoveSavedPath { get; set; }

        public ICommand RemoveExclusionCommand { get; set; }

        public OptionsViewModel()
        {
            Settings = new SettingsViewModel(GTApplication.Instance.Settings);
            ReuseCodeWindows = Properties.Settings.Default.ReuseCodeViewWindow;

            this.DisplayName = "Options";
            NotifyOfPropertyChange(() => DisplayName);

            AddExclusionCommand = new DelegateCommand((x) => AddExclusion(x));

            RemoveSavedPath = new DelegateCommand(x =>
            {
                if (this.SelectedPath != null)
                {
                    if (Settings.PathShortCuts.Contains(SelectedPath))
                    {
                        Settings.PathShortCuts.Remove(SelectedPath);
                        SelectedPath = null;
                        NotifyOfPropertyChange(() => SelectedPath);
                    }
                }
            });

            RemoveExclusionCommand = new DelegateCommand(x =>
            {
                string selectedExclusion = x as String;
                if (!String.IsNullOrEmpty(selectedExclusion))
                {
                    if (Settings.Exclusions.Contains(selectedExclusion))
                    {
                        Settings.Exclusions.Remove(selectedExclusion);
                    }
                }
            });
        }

        public void Save()
        {
            Settings.Save();
            TryClose(true);
        }

        public void Cancel()
        {
            //Revert
            Settings.Settings.Reload();
            TryClose(false);
        }

        public void AddExclusion(object param)
        {
            if (!String.IsNullOrEmpty(this.NewExclusion))
            {
                Settings.Exclusions.Add(this.NewExclusion);
                this.NewExclusion = "";
                NotifyOfPropertyChange(() => NewExclusion);
            }
        }
    }
}