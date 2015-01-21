using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Grep.Net.Data.Contexts;
using Grep.Net.Entities;

namespace Grep.Net.Data
{
    public class DefaultDataPopulatorHelper
    {
        /// <summary>
        /// This isn't called directly atm. Could be called later, keeping it around. 
        /// </summary>
        /// <param name="connectionStringOrConnectionStringName"></param>
        public static void PopulateDefaultData(String connectionStringOrConnectionStringName)
        {
            //Get the default XML files
            IList<FileTypeDefinition> ftdefs = GetAllEntitiesFromDirectory<FileTypeDefinition>();
            IList<PatternPackage> pps = GetAllEntitiesFromDirectory<PatternPackage>();

            IList<Template> templates = GetAllEntitiesFromDirectory<Template>();

            //Setup the contexts
            FileTypeDefinitionContext ftContext = new FileTypeDefinitionContext(connectionStringOrConnectionStringName);
            PatternPackageContext ppContext = new PatternPackageContext(connectionStringOrConnectionStringName);
            
            TemplateContext templateContext = new TemplateContext(connectionStringOrConnectionStringName);

            //Add the defaults
            ftContext.FileTypeDefinitions.AddRange(ftdefs);
            ppContext.PatternPackages.AddRange(pps);
            templateContext.Templates.AddRange(templates);

            //Save the changes and dispose. 
            ftContext.SaveChanges();
            ppContext.SaveChanges();
            templateContext.SaveChanges();
        }

        public static List<T> GetAllEntitiesFromDirectory<T>(string path = "")  where T : IEntity
        {
            List<T> items = new List<T>();

            if (String.IsNullOrEmpty(path))
                path = GetDefaultDirectoryForType<T>();

            foreach (string filePath in Directory.GetFiles(path, "*.xml", SearchOption.AllDirectories))
            {
                T entity = SerializationHelper.DeserializeXmlFromFile<T>(filePath);
                if (entity != null)
                    items.Add(entity);
            }
            return items;
        }

        public static string GetDefaultDirectoryForType<T>()
        {
            Assembly ass = Assembly.GetCallingAssembly();

            string basePath = AppDomain.CurrentDomain.RelativeSearchPath; // Path.GetDirectoryName(ass.Location); 
            //if null then we are running local. 
            if (basePath == null)
            {
                basePath = AppDomain.CurrentDomain.BaseDirectory;
            }
            switch (typeof(T).Name)
            {
                case "FileTypeDefinition":
                    return basePath + @"\Data\FileTypeDefinitions";
                case "PatternPackage":
                    return basePath + @"\Data\PatternPackages";
                case "Template":
                    return basePath + @"\Data\Templates";
                case "Classification":
                    return basePath + @"\Data\Classifications";
                
                default:
                    throw new ArgumentException("Unknown type");
            }
        }
    }
}