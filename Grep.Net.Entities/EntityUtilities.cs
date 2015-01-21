using System;
using System.Linq;
using System.Reflection;

namespace Grep.Net.Entities
{
    public static class EntityUtilities
    {
        public static bool TryGetEntityType(String s, out Type type)
        {
            Assembly ass = Assembly.GetAssembly(typeof(EntityUtilities));

            var types = ass.GetTypes().Where((x) => x.Name == s);
            type = types.FirstOrDefault();
            return type != null;
        }

        public static TEntity Create<TEntity>() where TEntity : BaseEntity
        {
            Assembly thisAsm = Assembly.GetAssembly(typeof(BaseEntity));
            TEntity ret = null;

            try
            {
                ret = (TEntity)Activator.CreateInstance(typeof(TEntity));
            }
            catch
            {
                //TODO: Log/Handle
            }
            return ret;
        }

        public static BaseEntity Create(Type t)
        {
            Assembly thisAsm = Assembly.GetAssembly(typeof(EntityUtilities));

            try
            {
                return (BaseEntity)Activator.CreateInstance(t);
            }
            catch 
            {
                //TODO: Log/Handle
            }
            return null;
        }

        public static BaseEntity CreateFromTypeString(String s)
        {
            Type t = null;
            if (TryGetEntityType(s, out t))
            {
                return Create(t);
            }
            return null;
        }
    }
}