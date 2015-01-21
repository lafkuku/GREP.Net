using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grep.Net.Entities;
namespace Grep.Net.Data.Repositories
{
    public class InMemoryRepository<T> : IRepository<T> 
    {
        public IList<T> Data { get; set; }

        public InMemoryRepository()
        {
            Data = new List<T>();
        }
        public IQueryable<T> GetAll()
        {
            return Data.AsQueryable<T>();
        }

        public void Remove(T item)
        {
            if (Data.Contains(item))
            {
                Data.Remove(item);
            }
        }

        public void Modify(T item)
        {
            //There really is no real "Modify" on in memory data stores.. Maybe consider "mapping" technology here. 
        }

        public void Add(T item)
        {
            Data.Add(item);
        }

        public void Commit()
        {
            //No real commit on InMemory Datastores... 
        }

        public void Merge(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                if (!Data.Contains(item))
                {
                    Data.Add(item);
                }
            }
        }
    }
}
