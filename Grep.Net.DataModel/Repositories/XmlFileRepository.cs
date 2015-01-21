using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

using System.Threading.Tasks;

using Grep.Net.Entities;
using NLog; 


namespace Grep.Net.Data.Repositories
{
    public  class XmlDirRepository<T> : IRepository<T>  where T :  class, IEntity
    {
        public IList<T> Entities { get; set; }

        public String EntityXmlDirectory { get; private set; }

        public Dictionary<Guid, String> EntityIdToPath { get; set; }

        static Logger logger = LogManager.GetCurrentClassLogger(); 

        public XmlDirRepository(String dir)
        {
            EntityXmlDirectory = dir;
            Entities = new List<T>();
            EntityIdToPath = new Dictionary<Guid, string>(); 
            LoadEntities(); 
        }

        public virtual void LoadEntities()
        {
            if (Entities == null)
                Entities = new List<T>();

            Entities.Clear(); 

            if (Directory.Exists(EntityXmlDirectory))
            {

                foreach (string filePath in Directory.GetFiles(EntityXmlDirectory, "*.xml", SearchOption.AllDirectories))
                {
                    T entity = SerializationHelper.DeserializeXmlFromFile<T>(filePath);
                    if (entity != null)
                    {
                        if (entity.Id == null ||
                            entity.Id == Guid.Empty)
                        {
                            entity.Id = Guid.NewGuid();
                        }
                        Entities.Add(entity);
                        EntityIdToPath.Add(entity.Id, filePath);
                    }
                }
              
            }
        }

        public IQueryable<T> GetAll()
        {
            return Entities.AsQueryable<T>(); 
        }

        public void Remove(T item)
        {
            if (item == null)
            {
                logger.Error("Attempting to remove null item in Repsository <{0}>", typeof(T));
                return;
            }

            if (this.Entities.Contains(item))
            {
                Entities.Remove(item);
            }
        }

        public void Modify(T item)
        {
            if (item == null)
            {
                logger.Error("Attempting to modify null item in Repsository <{0}>", typeof(T));
                return;
            }
            
        }

        public void Add(T item)
        {
            if (item == null)
            {
                logger.Error("Attempting to add null item to Repsository <{0}>", typeof(T));
                return;
            }
            this.Entities.Add(item);
        }
        public void Merge(IEnumerable<T> items)
        {
            throw new NotImplementedException(); 
        }
        public void Commit()
        {
            //Clear the directory. 

            if (Directory.Exists(EntityXmlDirectory))
            {
                //Clear
                Directory.Delete(EntityXmlDirectory, true);

                //Create a new one
                Directory.CreateDirectory(EntityXmlDirectory);
            }

            //Write the entities. 

            foreach (T item in Entities)
            {
                string path = string.Empty;

                if (!EntityIdToPath.TryGetValue(item.Id, out path))
                {
                    path = EntityXmlDirectory + "\\" + String.Format("{0}.xml", Guid.NewGuid());
                }
                SerializationHelper.SerializeIntoXmlFile(path, item);
            }
        }
    }
}
