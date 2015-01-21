using System;
using System.Linq;
using Caliburn.Micro;
using Grep.Net.Entities;
using Grep.Net.WPF.Client.Interfaces;

namespace Grep.Net.WPF.Client.ViewModels.Entities
{
    public class FileExtensionViewModel : PropertyChangedBase
    {
        private FileExtension _fileExtension;

        public FileExtension FileExtension
        {
            get
            {
                return _fileExtension;
            }
            set
            {
                this._fileExtension = value;
                NotifyOfPropertyChange(() => FileExtension);
            }
        }

        public bool IsEnabled
        {
            get
            {
                return _fileExtension.IsEnabled;
            }
            set
            {
                _fileExtension.IsEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
            }
        }

        public String Extension
        {
            get
            {
                return FileExtension.Extension;
            }
            set
            {
                FileExtension.Extension = value;
                NotifyOfPropertyChange(() => Extension); 
            }
        }

        public FileExtensionViewModel(FileExtension fe)
        {
            this.FileExtension = fe; 
        }

        public FileExtensionViewModel()
            : this(new FileExtension())
        {


        }
    }
}