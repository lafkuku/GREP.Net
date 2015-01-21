using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grep.Net.Entities;
using Caliburn.Micro;
using Grep.Net.Model;
using Grep.Net.WPF.Client.Data;
using Grep.Net.WPF.Client.ViewModels.Entities;
using Grep.Net.Data.Repositories;
using Grep.Net.WPF.Client.Interfaces;
namespace Grep.Net.WPF.Client.Services
{
    public class DataService : Grep.Net.WPF.Client.Services.IDataService
    {
        public RepositoryServiceBase<FileTypeDefinitionViewModel, FileTypeDefinition> FileTypeDefinitionService { get; set; }
        public RepositoryServiceBase<PatternPackageViewModel, PatternPackage> PatternPackageService { get; set; }
        public RepositoryServiceBase<GrepResultViewModel, GrepResult> GrepResultService { get; set; }

        public RepositoryServiceBase<GrepContextViewModel, GrepContext> GrepContextService { get; set; }

        public DataService()
        {
            FileTypeDefinitionService = new RepositoryServiceBase<FileTypeDefinitionViewModel,FileTypeDefinition>(GTApplication.Instance.DataModel.FileTypeDefinitionRepository, (x)=>new FileTypeDefinitionViewModel(x));
            PatternPackageService = new RepositoryServiceBase<PatternPackageViewModel, PatternPackage>(GTApplication.Instance.DataModel.PatternPackageRepository, (x) => new PatternPackageViewModel(x));
            GrepResultService = new RepositoryServiceBase<GrepResultViewModel, GrepResult>(GTApplication.Instance.DataModel.GrepResultRepository, (x) => new GrepResultViewModel() { Entity = x });
            GrepContextService = new RepositoryServiceBase<GrepContextViewModel, GrepContext>(GTApplication.Instance.DataModel.GrepContextRepository, (x) => new GrepContextViewModel(x));
        }
    }
}
