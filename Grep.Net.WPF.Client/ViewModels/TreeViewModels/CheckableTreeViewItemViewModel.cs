using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grep.Net.WPF.Client.ViewModels.TreeViewModels
{
    public abstract class CheckableTreeViewItemViewModel : TreeViewItemViewModel
    {
        public override bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                if (!_isExpanded &&
                    ContainsDummy())
                {
                    RefreshChildren();
                }

                _isExpanded = value;
                NotifyOfPropertyChange(() => IsExpanded);
            }
        }


        protected bool? _isChecked;

        public virtual bool? IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                CheckedStateChanged(value);
                NotifyOfPropertyChange(() => IsChecked);
            }
        }

        public abstract bool ContainsDummy(); 
        
        /// <summary>
        /// This logic is so god damn complex, could be refactored at some point. It's all for keeping checks in sync. Works and is quick, but just hard to read could probably condense the two "simialr" statements below. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cascadeToChildren"></param>
        private void CheckedStateChanged(bool? value, bool cascadeToChildren = true)
        {
            if ((value == true || value == false) &&
                cascadeToChildren &&
                !ContainsDummy())
            {
                foreach (CheckableTreeViewItemViewModel cvm in this.Children)
                {
                    if (cvm.IsExpanded)
                    {
                        cvm.IsChecked = value;
                    }
                    else
                    {
                        cvm._isChecked = value;
                        //Force the notification.
                        cvm.NotifyOfPropertyChange(() => cvm.IsChecked);
                    }
                }
            }
            //Check the state of my siblings
            var parent = (Parent as CheckableTreeViewItemViewModel);

            if (parent != null)
            {
                if (value == false || value == null)
                {
                    if (parent.Children.Cast<CheckableTreeViewItemViewModel>().Any(x => x.IsChecked == true))
                        parent.IsChecked = null;
                    else
                    {
                        if (value == false)
                        {
                            parent._isChecked = false; //Set the state, but avoid this callback. 
                            parent.CheckedStateChanged(false, false);
                            parent.NotifyOfPropertyChange(() => parent.IsChecked);
                        }
                        else
                        {
                            parent.IsChecked = null;
                        }
                    }
                }
                else if (value == true)
                {
                    if (parent.Children.Cast<CheckableTreeViewItemViewModel>().Any(x => x.IsChecked == false))
                        parent.IsChecked = null;
                    else
                    {
                        if (value == true)
                        {
                            parent._isChecked = true; //Set the state, but avoid this callback. 
                            parent.CheckedStateChanged(true, false);
                            parent.NotifyOfPropertyChange(() => parent.IsChecked);
                        }
                        else
                        {
                            parent.IsChecked = null;
                        }
                    }
                }
            }
        }
    }
}
