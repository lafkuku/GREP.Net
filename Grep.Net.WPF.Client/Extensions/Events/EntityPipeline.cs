using System;
using System.Linq;
using Caliburn.Micro;

namespace Grep.Net.WPF.Client.Events
{
    public class EntityPipeline : EventAggregator
    {
        #region Singleton Pattern
    
        private static EntityPipeline m_instance;
        
        public static EntityPipeline Instance
        {
            get
            {
                if (m_instance == null)
                {
                    lock (lok)
                    {
                        m_instance = new EntityPipeline();
                    }
                }
                return m_instance;
            }
        }
        
        private static object lok = new object();
    
        #endregion
    }
}