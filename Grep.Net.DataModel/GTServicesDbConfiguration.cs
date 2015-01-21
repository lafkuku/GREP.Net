using System;
using System.Data.Entity;
using System.Linq;

namespace Grep.Net.Data
{
    public class GrepNetDbConfiguration : DbConfiguration
    {
        /// <summary>
        /// This is used by the client application
        /// </summary>
        public static String LocalDBConnectionString = "";

        /// <summary>
        /// This is used by the serverSize application. 
        /// </summary>
        public static String SqlAzureConnectionString = ""; 

        public GrepNetDbConfiguration() : base()
        {
            //Common connection stuffs.
        }
    }
}