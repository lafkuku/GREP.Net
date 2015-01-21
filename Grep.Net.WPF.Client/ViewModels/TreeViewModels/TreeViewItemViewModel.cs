using System;
using System.Linq;
using System.Windows.Data;
using Caliburn.Micro;
using Grep.Net.WPF.Client.Interfaces;

namespace Grep.Net.WPF.Client.ViewModels
{
    public class TreeViewItemViewModel : PropertyChangedBase
    {
        protected bool _isExpanded;

        public virtual bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                if (_isExpanded == false && value == true)
                {
                    RefreshChildren();
                }
                _isExpanded = value;
                NotifyOfPropertyChange(() => IsExpanded);
            }
        }

        protected bool _isSelected;

        public virtual bool IsSelected
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

        public virtual TreeViewItemViewModel Parent { get; set; }

        public ListCollectionView Children { get; set; }

        public TreeViewItemViewModel(TreeViewItemViewModel parent, System.Collections.IList _children)
        {
            this.Parent = parent;
            this.Children = new ListCollectionView(_children);
        }

        public TreeViewItemViewModel(TreeViewItemViewModel parent, ListCollectionView _children)
        {
            this.Parent = parent;
            this.Children = _children;
        }

        public TreeViewItemViewModel(TreeViewItemViewModel parent)
        {
            this.Parent = parent;
        }

        protected TreeViewItemViewModel()
        {
        }

        public virtual void RefreshChildren()
        {
            if(this.Children != null)
                this.Children.Refresh();
        }
    }
}