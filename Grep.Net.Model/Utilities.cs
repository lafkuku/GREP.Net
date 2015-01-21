using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Grep.Net.Model
{
    public class Utilities
    {
        public static string GetWorkingDirectory()
        {
            String path = Assembly.GetCallingAssembly().Location;

            return Path.GetDirectoryName(path); 
        }

    }
}