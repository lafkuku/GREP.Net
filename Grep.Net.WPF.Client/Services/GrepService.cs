using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grep.Net.Model.Models;
using Grep.Net.Model;
using Grep.Net.Entities;
using Grep.Net.WPF.Client.ViewModels.Entities;


namespace Grep.Net.WPF.Client.Services
{
    public class GrepService
    {

        IDataService DataService { get; set; }
        GrepModel Model { get; set; }
        public GrepService(GrepModel model, IDataService dataService)
        {
            Model = model;
            DataService = dataService;
        }

        public async Task<GrepContextViewModel> StartGrep(string dir, IList<PatternPackage> patterns, IList<FileExtension> extensions)
        {
            GrepContext gc = new GrepContext()
            {
                RootPath = dir,
                PatternPackages = patterns.ToList(),
                FileExtensions = extensions.ToList(),
                TimeStarted = DateTime.Now
            };

            gc.OnCompleted += new GrepContext.CompletedAction((x, y) =>
            {
                DataService.GrepResultService.Add(y);
            });

            gc.CancelToken.Token.Register(new Action(() =>
            {
                if (gc.OnDirectory != null)
                {
                    gc.OnDirectory(gc, "Cancelled");
                }

            }));

            var vm = DataService.GrepContextService.Add(gc);
            await Model.Grep(gc);
            return vm;
        }
    }
}
