using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grep.Net.Entities;
using Grep.Net.WPF.Client.Interfaces;
using Caliburn.Micro;

namespace Grep.Net.WPF.Client.Services
{

    /// <summary>
    /// I think this needs to be this way to allow the generic in the callback. /Shrug. need to investigate a cleaner solution. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    public class TMP<T, K> 
        where T : IViewModel<K>
        where K : IEntity
    {
        public delegate void ItemAdded(T Item);
        public delegate void ItemRemoved(T Item);
        public delegate void DataReloaded();
    }
   

    public interface IRepoService<T, K> where T : IViewModel<K>
                                        where K : IEntity
    {

        void ReloadData();

        IQueryable<T> GetAll();

        T Get(Guid id); 

        T Add(K item);
        T Remove(Guid id);
        T Remove(K ftd);

        

        event TMP<T, K>.ItemAdded OnItemAdded;
        event TMP<T, K>.ItemRemoved OnItemRemoved;
        event TMP<T, K>.DataReloaded OnDataReloaded; 
       
    }
}
