using System;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using Grep.Net.Model.Models;


namespace Grep.Net.Model
{
    public class GTApplication
    {
        #region Singleton
        
        private static GTApplication _instance;
        
        public static GTApplication Instance
        {
            get
            {
                lock (_lok)
                {
                    if (_instance == null)
                    {
                        _instance = new GTApplication();
                    }
                }
                return _instance;
            }
        }
        
        private static object _lok = new object();
        
        #endregion
        
        //Dealing with the core model logic for Greping. 
        public GreppingService GrepService { get; set; }
        
        public ObservableCollection<String> DirectoryFilters { get; set; }
        
        //Data Access and stuffs. 
        public DataModel DataModel { get; set; }
        
        public Properties.Settings Settings { get; set; }
        
        private GTApplication()
        {
            Settings = Properties.Settings.Default;
            BootStrap();
        }
        
        internal virtual void BootStrap()
        {
            this.GrepService = new GreppingService();
            
            DirectoryFilters = new BindableCollection<string>();
            
            this.DataModel = new DataModel(this);
            this.GrepService.DataModel = this.DataModel;
        }
        
        public void SaveDataToDB()
        {
            DataModel.SaveState();
        }
        
        public void Shutdown()
        {
            GrepService.Scheduler.Stop();
            GrepService.RunspacePool.Close();
            if (Settings.AutoSave)
            {
                Settings.Save();
                SaveDataToDB();
            }
        }
    }
}