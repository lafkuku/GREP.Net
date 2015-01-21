using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

using Grep.Net.Entities;

namespace Grep.Net.Data.Repositories
{
    public class DataContextRepository<K, T> : IRepository<T> where K : DbContext
                                                              where T : class, IEntity
    {
        K DataContext { get; set; }


        public DataContextRepository(K context)
        {
            DataContext = context;
        }

        public virtual void Merge(IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }
        
        public virtual void Modify(T item)
        {
            try
            {
                this.DataContext.Set<T>().Attach(item);
                this.DataContext.Entry(item).State = EntityState.Modified;
                this.DataContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (this.DataContext.Set<T>().Any(x => x.Equals(item)))
                {
                    return;
                }
                else
                {
                    throw;
                }
            }
        }

        public virtual void Add(T item)
        {
            try
            {
                this.DataContext.Set<T>().Add(item);
                this.DataContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (this.DataContext.Set<T>().Any(x=> x.Equals(item)))
                {
                    return;
                }
                else
                {
                    throw;
                }
            }
        }

        public virtual void Remove(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            DataContext.Set<T>().Remove(item);
            DataContext.SaveChanges();
        }

        /*
            if( children == null || child+ren.Length == 0 )
            {
                return Objects.SingleOrDefault( e => e.ID == id );
            }
            DbQuery<T> query = children.Aggregate<string, DbQuery<T>>( Objects, ( current, child ) => current.Include( child ) );
            return query.SingleOrDefault( e => e.ID == id );
        */

        public virtual IQueryable<T> GetAll()
        {
            return DataContext.Set<T>().AsQueryable<T>();
        }

        public virtual void Commit()
        {
            this.DataContext.SaveChanges();
        }


        /* Old save method if i ever come back to a database for this. 
        public void SaveState()
        {
            using (FileTypeDefinitionContext ftContext = new FileTypeDefinitionContext())
            {
                ftContext.FileTypeDefinitions.RemoveRange(ftContext.FileTypeDefinitions.ToList());

                foreach (FileTypeDefinition def in this.FileTypeDefinitions)
                {
                    ftContext.FileTypeDefinitions.Add(def);
                }
                bool Saved = true;
                do
                {
                    try
                    {
                        ftContext.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        Saved = false;
                        var entry = ex.Entries.Single();
                        entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    }
                }
                while (!Saved);

                ftContext.SaveChanges();
            }

            using (PatternPackageContext ppContext = new PatternPackageContext())
            {
                ppContext.PatternPackages.RemoveRange(ppContext.PatternPackages.ToList());
                //ppContext.SaveChanges();

                foreach (PatternPackage pp in this.PatternPackages)
                {

                    //ppContext.PatternPackages.Attach(pp);
                    //ppContext.Entry(pp).State = EntityState.Modified;

                    ppContext.PatternPackages.Add(pp);
                }

                //TODO: Add a retry count here. Like ASAP. 
                bool Saved = true;
                do
                {
                    try
                    {
                        ppContext.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        Saved = false;
                        var entry = ex.Entries.Single();
                        entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    }
                }
                while (!Saved);
            }
        }
        */
    }
}
