using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grep.Net.Entities;

namespace Grep.Net.Data.Repositories
{
    public interface IRepository<T> 
    {
        IQueryable<T> GetAll();

        void Remove(T item);

        void Modify(T item);

        void Add(T item);

        void Commit();


        void Merge(IEnumerable<T> items); 
    }
}
