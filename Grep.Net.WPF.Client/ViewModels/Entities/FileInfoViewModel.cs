using System;
using System.Linq;
using Caliburn.Micro;
using Grep.Net.Entities;

namespace Grep.Net.WPF.Client.ViewModels.Entities
{
    public class FileInfoViewModel : PropertyChangedBase
    {
        private FileInfo _fileInfo;

        public FileInfo FileInfo
        {
            get
            {
                return _fileInfo;
            }
            set
            {
                _fileInfo = value;
                NotifyOfPropertyChange(() => FileInfo); 
            }
        }
   
        public FileInfoViewModel()
        {
        }
    }
}