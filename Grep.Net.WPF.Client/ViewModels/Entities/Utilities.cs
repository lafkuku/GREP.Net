using System;
using System.Linq;
using Grep.Net.Model;

namespace Grep.Net.WPF.Client.ViewModels.Entities
{
    public static class Utilities
    {
        public static string GetBaseDirectoryForViewModel(Type t)
        {
            switch (t.Name)
            {
                case "FileTypeDefinitionViewModel":
                    return GTApplication.Instance.Settings.FileTypeDefinitionsDir;
                case "PatternPackageViewModel":
                    return GTApplication.Instance.Settings.PatternPackagesDir;
                case "TemplateViewModel":
                    return GTApplication.Instance.Settings.TemplatesDir;
                case "ClassificationViewModel":
                    return GTApplication.Instance.Settings.ClassificationsDir;
                
                default:
                    throw new ArgumentException("Unknown type");
            }
        }
    }
}