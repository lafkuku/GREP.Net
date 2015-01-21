using System;
using System.Linq;
using Caliburn.Micro;

namespace Grep.Net.WPF.Client.ViewModels.CrumbListView
{
    public class CrumbNavigationListViewBaseViewModel : PropertyChangedBase
    {
        public BindableCollection<CrumbListViewModel> Crumbs { get; set; }

        private CrumbListViewModel _currentCrumb;

        public CrumbListViewModel CurrentCrumb
        {
            get
            {
                return _currentCrumb;
            }
            set
            {
                _currentCrumb = value;
                NotifyOfPropertyChange(() => CurrentCrumb);
            }
        }

        public CrumbNavigationListViewBaseViewModel()
        {
            Crumbs = new BindableCollection<CrumbListViewModel>();

            Crumbs.CollectionChanged += Crumbs_CollectionChanged;
        }

        private void Crumbs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (Crumbs.Count > 0)
            {
                //Set to the last item..
                this.CurrentCrumb = Crumbs[Crumbs.Count - 1];
            }
            else
            {
                this.CurrentCrumb = null;
            }
        }
        public virtual void OnNavigate()
        {

        }
        
        public virtual void GoForward()
        {
        }

        public virtual void GoBack()
        {
            if (Crumbs.Count > 0)
            {
                //Should figure out if Remove is O(1) or O(N) but there wont be enough items in this collection to even matter.. //Whatever
                Crumbs.RemoveAt(Crumbs.Count - 1);
            }
        }
    }
}