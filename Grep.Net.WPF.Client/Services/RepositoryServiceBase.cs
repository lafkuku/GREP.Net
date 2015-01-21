using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grep.Net.Entities;
using Caliburn.Micro;
using Grep.Net.Model;
using Grep.Net.WPF.Client.Data;
using Grep.Net.WPF.Client.ViewModels.Entities;
using Grep.Net.Data.Repositories;
using Grep.Net.WPF.Client.Interfaces;

namespace Grep.Net.WPF.Client.Services
{
    public class RepositoryServiceBase<T, K> : IRepoService<T, K>  where T : IViewModel<K>
                                                                   where K : IEntity
    {
        BindableCollection<T> Data { get; set; }

        IRepository<K> Repo { get; set; }

        Func<K, T> _createViewModel { get; set; }


        public event TMP<T, K>.ItemAdded OnItemAdded;
        public event TMP<T, K>.ItemRemoved OnItemRemoved;
        public event TMP<T, K>.DataReloaded OnDataReloaded; 

        public RepositoryServiceBase(IRepository<K> sourceRepository, Func<K, T> _createViewModelFunc = null)
        {
            _createViewModel = _createViewModelFunc;
            Repo = sourceRepository;
            ReloadData();
        }
        private bool DefaultConstraint(T item)
        {
            return true;
        }
        public void OneWaySyncTo(BindableCollection<T> syncCollection, Func<T, bool> constraint = null)
        {
            OnItemAdded += (x) =>
            {
                if (constraint != null)
                {

                    if (constraint(x))
                    {
                        syncCollection.Add(x);
                    }
                }
                else
                {
                    syncCollection.Add(x);
                }

            };
            OnItemRemoved += (x) => syncCollection.Remove(x);
            OnDataReloaded += () =>
            {
                syncCollection.Clear();
                if (constraint == null)
                    syncCollection.AddRange(Data);
                else
                    syncCollection.AddRange(Data.Where(x => constraint(x)).ToList());
            };
        }

        public void ReloadData()
        {
            if (Data == null)
            {
                Data = new BindableCollection<T>();
            }
            //Reload? 
            foreach (K item in Repo.GetAll())
            {
                T vm = _createViewModel(item);
                Data.Add(vm);
            }
            if (OnDataReloaded != null)
            {
                OnDataReloaded(); 
            }
        }

        public IQueryable<T> GetAll()
        {
            return Data.AsQueryable<T>();
        }

        public T Get(Guid id)
        {
            return Data.FirstOrDefault(x => x.Entity.Id == id);
        }

        public T Add(K item)
        {
            T vm = _createViewModel(item);
            Data.Add(vm);

            if (OnItemAdded != null)
            {
                OnItemAdded(vm);
            }
            return vm;
        }

        public T Remove(Guid id)
        {

            T vm = Data.FirstOrDefault(x => x.Entity.Id == id);

            Data.Remove(vm);
            if (OnItemRemoved != null)
            {
                OnItemRemoved(vm);
            }
            return vm;
        }

        public T Remove(K ftd)
        {
            T vm = Data.FirstOrDefault(x => x.Entity.Id == ftd.Id);

            Data.Remove(vm);
            if (OnItemRemoved != null)
            {
                OnItemRemoved(vm);
            }
            return vm;
        }
    }
}
