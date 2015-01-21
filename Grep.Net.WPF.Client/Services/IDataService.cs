using System;
namespace Grep.Net.WPF.Client.Services
{
    public interface IDataService
    {
        RepositoryServiceBase<Grep.Net.WPF.Client.ViewModels.Entities.FileTypeDefinitionViewModel, Grep.Net.Entities.FileTypeDefinition> FileTypeDefinitionService { get; set; }
        RepositoryServiceBase<Grep.Net.WPF.Client.ViewModels.Entities.GrepResultViewModel, Grep.Net.Entities.GrepResult> GrepResultService { get; set; }
        RepositoryServiceBase<Grep.Net.WPF.Client.ViewModels.Entities.PatternPackageViewModel, Grep.Net.Entities.PatternPackage> PatternPackageService { get; set; }

        RepositoryServiceBase<Grep.Net.WPF.Client.ViewModels.Entities.GrepContextViewModel, Grep.Net.Entities.GrepContext> GrepContextService { get; set; }
    }
}
