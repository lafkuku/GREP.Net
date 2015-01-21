using System;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;


namespace Grep.Net.WPF.Client.Data
{
    /// <summary>
    /// This collection will keep two collections in sync using the mapper between the two collections. This is to be used in cases where we have a collection of Model objects that implements 
    /// NotifyCollectionChanged event to keep it in sync with a collection of ViewModels which will be bound in the View. 
    /// 
    /// This also syncs back to the source in the case of removal. This can safely occur at this layer. However, add would cause recusion unless addressed. TODO: Should address this. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="VMT"></typeparam>
    public class SyncedBindableCollection<T, VMT> : BindableCollection<VMT>, ISyncedCollection
    {
        private object lok = new object();

        private ObservableCollection<T> _syncSourceCollection;

        public ObservableCollection<T> SyncSourceCollection
        {
            get
            {
                return _syncSourceCollection;
            }

            set
            {
                lock (lok)
                {
                    _syncSourceCollection = value;
                    this.ResyncFromSource();
                }
            }
        }

        private Func<T, VMT> SyncMapper { get; set; }

        private Func<T, VMT, Boolean> Comparer { get; set; }

        private Func<VMT, T> Getter { get; set; }

        public SyncedBindableCollection(ObservableCollection<T> syncSourceCollection,
            Func<T, VMT> syncMapper,
            Func<T, VMT, Boolean> comparer)
        {
            SyncSourceCollection = syncSourceCollection;
            SyncMapper = syncMapper;
            Comparer = comparer;

            this.SyncSourceCollection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SyncSourceCollection_CollectionChanged);
            this.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SyncedBindableCollection_CollectionChanged);
            ResyncFromSource();
        }

        private void SyncedBindableCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (VMT vmt in e.OldItems)
                    {
                        T t = GetSourceFrom(vmt);
                        if (t != null)
                            _syncSourceCollection.Remove(t);
                    }
                    break;
            }
        }

        /// <summary>
        /// O(N) search through source to locate. 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private T GetSourceFrom(VMT viewModel)
        {
            foreach (T t in this._syncSourceCollection)
            {
                if (Comparer(t, viewModel))
                {
                    return t;
                }
            }

            return default(T);
        }
    
        /// <summary>
        /// From Source to This
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SyncSourceCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (T t in e.NewItems)
                    {
                        VMT newItem = SyncMapper(t); //This should work. Prevents a duplicate Model in the collection. 
                        if (newItem != null)
                        {
                            this.Add(newItem);
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (T t in e.OldItems)
                    {
                        VMT destItem = GetItemFrom(t);
                        this.Remove(destItem);
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    this.Move(e.OldStartingIndex, e.NewStartingIndex);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    T tmpNewItem = (T)e.NewItems[0];
                    VMT tmpNewVMT = SyncMapper(tmpNewItem);
                    if (tmpNewVMT != null)
                    {
                        this[e.OldStartingIndex] = tmpNewVMT;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    ResyncFromSource();
                    break;
            }
        }

        public VMT GetItemFrom(T sourceItem)
        {
            foreach (VMT vmt in this)
            {
                if (Comparer(sourceItem, vmt))
                {
                    return vmt;
                }
            }

            return default(VMT);
        }

        private void ResyncFromSource()
        {
            this.Clear();
            foreach (T t in this._syncSourceCollection)
            {
                if (SyncMapper != null)
                {
                    VMT vmt = SyncMapper(t);
                    if (vmt != null)
                    {
                        this.Add(vmt);
                    }

                }
             
            }
        }

        public object GetSyncedItemFromSource(object sourceItem)
        {
            Type t = sourceItem.GetType();
            if (typeof(T) != t)
            {
                return null; 
            }
            
            foreach (VMT vm in this)
            {
                if (Comparer((T)sourceItem, vm))
                {
                    return vm;
                }
            }
              
            return null;
        }
    }
}