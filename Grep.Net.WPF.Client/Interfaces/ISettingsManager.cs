using System;
using Caliburn.Micro;

namespace Grep.Net.WPF.Client.ViewModels.Interfaces
{
    public interface ISettingsManager
    {
        bool AutoSave { get; set; }
        string ClassificationsDir { get; set; }
        BindableCollection<String> Exclusions { get; set; }
        string FileTypeDefinitionsDir { get; set; }
        int GrepThreadsMax { get; set; }
        int LinesAfter { get; set; }
        int LinesBefore { get; set; }
        int MaxContextSize { get; set; }
        int MaxLineSize { get; set; }
        BindableCollection<String> PathShortCuts { get; set; }
        string PatternPackagesDir { get; set; }
        int PoolRunspaceMax { get; set; }
        int PoolRunspaceMin { get; set; }
        bool Recurse { get; set; }
        void Save();
        bool SaveLayout { get; set; }
        Grep.Net.Model.Properties.Settings Settings { get; set; }
        string TemplatesDir { get; set; }
    }
}
