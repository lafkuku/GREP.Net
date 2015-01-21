using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grep.Net.WPF.Client.Interfaces;
using Grep.Net.Entities;
using Caliburn.Micro;

namespace Grep.Net.WPF.Client.ViewModels.Entities
{
    public class GrepContextViewModel : PropertyChangedBase, IViewModel<GrepContext>
    {
        public GrepContext Entity { get; set;  }


        public string RootPath
        {
            get { return Entity.RootPath; }
            set
            {
                Entity.RootPath = value;
                NotifyOfPropertyChange(() => RootPath);
            }
        }

        public Guid Id
        {
            get { return Entity.Id; }
            set
            {
                Entity.Id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }


        public SearchErrorType ErrorType
        {
            get { return Entity.ErrorType; }
            set
            {
                Entity.ErrorType = value;
                NotifyOfPropertyChange(() => ErrorType);
            }
        }


        public DateTime TimeStarted
        {
            get { return Entity.TimeStarted; }
            set
            {
                Entity.TimeStarted = value;
                NotifyOfPropertyChange(() => TimeStarted);
            }
        }

        public DateTime TimeCompleted
        {
            get { return Entity.TimeCompleted; }
            set
            {
                Entity.TimeCompleted = value;
                NotifyOfPropertyChange(() => TimeCompleted);
            }
        }

        public bool Completed
        {
            get { return Entity.Completed; }
            set
            {
                Entity.Completed = value;
                NotifyOfPropertyChange(() => Completed);
            }
        }


        private String _currentDirectory;

        public String CurrentDirectory
        {
            get { return _currentDirectory; }
            set
            {
                _currentDirectory = value;
                NotifyOfPropertyChange(() => CurrentDirectory);
            }
        }

        public virtual List<PatternPackage> PatternPackages { get; set; }

        public virtual List<FileExtension> FileExtensions { get; set; }


        public GrepContextViewModel(GrepContext context)
        {
            Entity = context;
            CurrentDirectory = "Hasn't Started";

            context.OnDirectory += (x, y) => CurrentDirectory = y;
            context.OnCompleted += (x, y) =>
            {
                NotifyOfPropertyChange(() => TimeStarted);
                NotifyOfPropertyChange(() => TimeCompleted);
                CurrentDirectory = "Completed";
            };
        }
    }
}
