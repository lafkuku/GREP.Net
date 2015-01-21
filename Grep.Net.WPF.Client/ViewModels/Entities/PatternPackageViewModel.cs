using System;
using System.Linq;
using Caliburn.Micro;
using Grep.Net.Entities;
using Grep.Net.WPF.Client.Data;
using Grep.Net.WPF.Client.Interfaces;
using Grep.Net.WPF.Client.Services;

 
namespace Grep.Net.WPF.Client.ViewModels.Entities
{
    public class PatternPackageViewModel : PropertyChangedBase, IViewModel<PatternPackage>
    {
        private PatternPackage _patternPackage;

        public PatternPackage Entity
        {
            get
            {
                return _patternPackage;
            }
            set
            {
                _patternPackage = value;

                Patterns.Clear();

                if (_patternPackage.Patterns != null)
                {
                    ResetPattrens();
                }
             

                NotifyOfPropertyChange(() => Entity);
                NotifyOfPropertyChange(() => Patterns);
                NotifyOfPropertyChange(() => Name);
                NotifyOfPropertyChange(() => BasePattern);
                NotifyOfPropertyChange(() => Category);
            }
        }

        public string Name
        {
            get
            {
                return _patternPackage.Name;
            }
            set
            {
                _patternPackage.Name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public string Category
        {
            get
            {
                return _patternPackage.Category;
            }
            set
            {
                _patternPackage.Category = value;
                NotifyOfPropertyChange(() => Category);
            }
        }

        public string BasePattern
        {
            get
            {
                return _patternPackage.BasePattern;
            }
            set
            {
                _patternPackage.BasePattern = value;
                NotifyOfPropertyChange(() => BasePattern);
            }
        }

        public BindableCollection<PatternViewModel> Patterns { get; set; }
        public bool IsEnabled
        {
            get
            {
                return Entity.IsEnabled;
            }
            set
            {
                Entity.IsEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
            }
        }

        public Object SelectedDataGridItem { get; set; }

        public PatternPackageViewModel(PatternPackage pPackage)
        {


            Patterns = new BindableCollection<PatternViewModel>();
            this.Entity = pPackage;

            Patterns.CollectionChanged += (x, y) =>
            {
                if (y.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (var item in y.NewItems.Cast<PatternViewModel>())
                    {
                        _patternPackage.Patterns.Add(item._pattern);
                    }
                }
                if (y.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    foreach (var item in y.NewItems.Cast<PatternViewModel>())
                    {
                        _patternPackage.Patterns.Remove(item._pattern);
                    }
                }

                if (y.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                {
                    ResetPattrens();
                }
            };
        }
        public void DeleteSelectedPattern()
        {
            if (this.SelectedDataGridItem != null &&
                this.SelectedDataGridItem is Pattern &&
                this.Entity.Patterns.Contains(SelectedDataGridItem as Pattern))
            {
                this.Entity.Patterns.Remove(this.SelectedDataGridItem as Pattern);
            }
        }

        public void ResetPattrens()
        {
            this.Patterns.Clear(); 
            foreach (Pattern p in this._patternPackage.Patterns)
            {
                this.Patterns.Add(new PatternViewModel() { Pattern = p });
            }
        }
    }
}