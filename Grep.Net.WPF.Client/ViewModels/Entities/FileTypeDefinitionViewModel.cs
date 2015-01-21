using System;
using System.Linq;
using Caliburn.Micro;
using Grep.Net.Entities;
using Grep.Net.WPF.Client.Data;
using Grep.Net.WPF.Client.Interfaces;

namespace Grep.Net.WPF.Client.ViewModels.Entities
{
    public class FileTypeDefinitionViewModel : PropertyChangedBase,  IViewModel<FileTypeDefinition>
    {
        private FileTypeDefinition _fileTypeDefinition;

        public FileTypeDefinition Entity
        {
            get
            {
                return _fileTypeDefinition;
            }
            set
            {
                _fileTypeDefinition = value;
                if (_fileTypeDefinition.FileExtensions != null)
                {
                    ResetExtensions(); 
                }
                NotifyOfPropertyChange(() => Entity); 
            }
        }

        public Guid Id
        {
            get
            {
                return _fileTypeDefinition.Id;
            }
            set
            {
                _fileTypeDefinition.Id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        public String Name
        {
            get
            {
                return _fileTypeDefinition.Name;
            }
            set
            {
                _fileTypeDefinition.Name = value;
                NotifyOfPropertyChange(() => Name); 
            }
        }

        public bool IsEnabled
        {
            get
            {
                return _fileTypeDefinition.IsEnabled;
            }
            set
            {
                _fileTypeDefinition.IsEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
            }
        }

        public BindableCollection<FileExtensionViewModel> FileExtensions { get; set; }
       
        private bool? _isSelected; 

        public bool? IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        public Object SelectedDataGridItem { get; set; }

        public FileTypeDefinitionViewModel(FileTypeDefinition ftd)
        {
            IsSelected = false;

            FileExtensions = new BindableCollection<FileExtensionViewModel>(); 
            Entity = ftd;
         

            FileExtensions.CollectionChanged += (x, y) =>
            {
                if (y.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (var item in y.NewItems.Cast<FileExtensionViewModel>())
                    {
                        _fileTypeDefinition.FileExtensions.Add(item.FileExtension);
                    }
                }
                if (y.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    foreach (var item in y.NewItems.Cast<FileExtensionViewModel>())
                    {
                        _fileTypeDefinition.FileExtensions.Remove(item.FileExtension);
                    }
                }

                if (y.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                {
                    ResetExtensions();
                }
            };


        }

        public void Save()
        {
        }

        public void Delete()
        {
        }

        public void DeleteSelectedFileExtension()
        {
            if (this.SelectedDataGridItem != null &&
                this.SelectedDataGridItem is FileExtension &&
                this.Entity.FileExtensions.Contains(this.SelectedDataGridItem as FileExtension))
            {
                this.Entity.FileExtensions.Remove(this.SelectedDataGridItem as FileExtension);
            }
        }
        public void ResetExtensions()
        {
            this.FileExtensions.Clear();
            foreach (FileExtension fe in this._fileTypeDefinition.FileExtensions)
            {
                this.FileExtensions.Add(new FileExtensionViewModel(fe));
            }
        }
    }
}