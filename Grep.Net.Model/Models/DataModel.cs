using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using Grep.Net.Data.Contexts;
using Grep.Net.Entities;
using Grep.Net.Model.Extensions;
using Grep.Net.Data.Repositories;

namespace Grep.Net.Model.Models
{
    public class DataModel
    {
        private GTApplication App { get; set; }
    
        public ObservableCollection<GrepResult> GrepResults { get; set; }

        public String CurrentContextPath { get; set; }
      
        public delegate void Resync(DataModel context); 

        public event Resync OnRsync;


        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();


        public IRepository<PatternPackage> PatternPackageRepository { get; set; }
        public IRepository<FileTypeDefinition> FileTypeDefinitionRepository { get; set;}

        public IRepository<Template> TemplateRepository { get; set; }
        public IRepository<GrepResult> GrepResultRepository { get; set; }

        public IRepository<GrepContext> GrepContextRepository { get; set; }

        public DataModel(GTApplication app)
        {
            App = app;
            Init(); 
        }

        public void Init()
        {
            //Initlizing the runtime state.
          
            //Load value from the Repositories
            //LoadDatabaseValues();
            PatternPackageRepository = GetXmlRepositoryFromPath<PatternPackage>(App.Settings.PatternPackagesDir);
            FileTypeDefinitionRepository = GetXmlRepositoryFromPath<FileTypeDefinition>(App.Settings.FileTypeDefinitionsDir);
            TemplateRepository = new XmlDirRepository<Template>(App.Settings.TemplatesDir);
            GrepResultRepository = new InMemoryRepository<GrepResult>();
            GrepContextRepository = new InMemoryRepository<GrepContext>(); 
            FixRelations();
        }

        public XmlDirRepository<T> GetXmlRepositoryFromPath<T>(String path) where T :  class, IEntity
        {
            string root = System.IO.Path.GetDirectoryName(typeof(XmlDirRepository<>).Assembly.Location);
            if(System.IO.Path.IsPathRooted(path))
                return new XmlDirRepository<T>(path);
            else
                return new XmlDirRepository<T>(root + "\\" + path);
        }

        public void FixRelations()
        {

             //Need to rebuild the  parent-child relationship between the Patterns and their patternPackages. 
             this.PatternPackageRepository.GetAll().ForEach(x =>
             {
                 x.Patterns.ForEach(y => y.PatternPackageId = x.Id);
             });
        }

        public void SaveState()
        {
            PatternPackageRepository.Commit();
            FileTypeDefinitionRepository.Commit(); 
        }



        private string GetBaseDirectoryForType(Type t)
        {
            switch (t.Name)
            {
                case "FileTypeDefinition":
                    return App.Settings.FileTypeDefinitionsDir;
                case "PatternPackage":
                    return App.Settings.PatternPackagesDir;
                case "Template":
                    return App.Settings.TemplatesDir;
              
                
                default:
                    throw new ArgumentException("Unknown type");
            }
        }

        public IList GetListFor(Type t)
        {
            PropertyInfo[] infos = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo info in infos)
            {
                if (info.PropertyType.IsGenericType &&
                    info.PropertyType.GetInterfaces().Contains(typeof(System.Collections.IList)) &&
                    info.PropertyType.GetGenericArguments().Length > 0 &&
                    info.PropertyType.GetGenericArguments()[0] == t
                )
                {
                    object o = info.GetValue(this, null);
                    if (o != null)
                        return o as IList;
                }
            }
            return null;
        }

        public IList<T> GetListFor<T>()
        {
            PropertyInfo[] infos = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo info in infos)
            {
                if (info.PropertyType.IsGenericType &&
                    info.PropertyType.GetInterfaces().Contains(typeof(System.Collections.IList)) &&
                    info.PropertyType.GetGenericArguments().Length > 0 &&
                    info.PropertyType.GetGenericArguments()[0] == typeof(T)
                )
                {
                    object o = info.GetValue(this, null);
                    if (o != null)
                        return o as IList<T>;
                }
            }
            return null;
        }
    }
}