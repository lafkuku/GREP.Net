using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Caliburn.Micro;
using Grep.Net.Model.Extensions;

namespace Grep.Net.WPF.Client.ViewModels
{
    public class ListPickerViewModel : Screen
    {
        public ListCollectionView Items { get; set; }

        public Object SelectedItem { get; set; }

        public string DisplayMemberPath { get; set; }

        private String _filterString;

        public String FilterString
        {
            get
            {
                return _filterString;
            }
            set
            {
                _filterString = value;
                NotifyOfPropertyChange(() => FilterString);
                if (Items != null)
                    Items.Refresh(); 
            }
        }
        
        public string FilterOnProperty { get; set; }

        public List<Object> SelectedItems { get; set; }

        public SelectionMode SelectionMode { get; set; }

        public ListPickerViewModel(IList list, String displayMemberPath = "", string filterOnProperty = "", SelectionMode mode = System.Windows.Controls.SelectionMode.Single)
        {
            this.FilterString = "";
            if (list != null)
                this.Items = new ListCollectionView(list);

            this.DisplayMemberPath = displayMemberPath;

            this.SelectedItems = new List<Object>();
            this.SelectionMode = mode;
            this.FilterOnProperty = filterOnProperty;
            Items.Filter += new Predicate<object>(ItemsFilterDelegate);

            NotifyOfPropertyChange(() => Items);
            NotifyOfPropertyChange(() => DisplayMemberPath);
        }

        public bool ItemsFilterDelegate(Object o)
        {
            if (!String.IsNullOrWhiteSpace(FilterOnProperty))
            {
                object property = o.GetPropertyValue(FilterOnProperty);
                if (property != null)
                {
                    String s = property.ToString();
                    if (!String.IsNullOrWhiteSpace(s))
                    {
                        return s.Contains(FilterString);
                    }
                }
            }
            return true;
        }

        public void SelectedItemChanged(ActionExecutionContext context)
        {
            SelectionChangedEventArgs eventArgs = context.EventArgs as SelectionChangedEventArgs;
            if (eventArgs != null)
            {
                eventArgs.RemovedItems.ForEach((x) =>
                {
                    SelectedItems.Remove(x);
                });

                eventArgs.AddedItems.ForEach((x) =>
                {
                    SelectedItems.Add(x);
                });
            }
        }

        public void Ok()
        {
            TryClose(true); 
        }

        public void Cancel()
        {
            TryClose(false); 
        }
    }
}