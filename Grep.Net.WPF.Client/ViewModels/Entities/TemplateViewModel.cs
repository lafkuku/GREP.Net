using System;
using System.Linq;
using Caliburn.Micro;
using Grep.Net.Entities;
using Grep.Net.WPF.Client.Interfaces;

namespace Grep.Net.WPF.Client.ViewModels.Entities
{
    public class TemplateViewModel : PropertyChangedBase, IFileEntity
    {
        private Template _template;

        public Template Template
        {
            get
            {
                return _template;
            }
            set
            {
                _template = value;
                NotifyOfPropertyChange(() => Template);
            }
        }

        public String Name
        {
            get
            {
                return Template.Name;
            }
            set
            {
                Template.Name = value;
                NotifyOfPropertyChange(() => this.Name);
            }
        }

        public String RawTemplate
        {
            get
            {
                return Template.RawTemplate;
            }
            set
            {
                Template.RawTemplate = value;
                NotifyOfPropertyChange(() => this.RawTemplate);
            }
        }

        public TemplateViewModel()
        {
            this.Template = new Template(); 
        }

        public void Save()
        {
        }

        public void Delete()
        {
        }
    }
}