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

        public static bool DirectoryExists(string path, int timeout = 3000)
        {
            bool exists = false;

            var toAction = new Action(delegate
            {
                exists = Directory.Exists(path);
            });
            IAsyncResult result = toAction.BeginInvoke(null, null);
            try
            {
                //We have an issue when updating text in AvalonEdit. Complex binary files are causing the UI thread to hang. I needed a way to timeout. 
                //This seems to work but is a huge hack. 
                result.AsyncWaitHandle.WaitOne(timeout); //This is such a hack, but works atm. Need to investigate a better fix. 
            }
            catch { }
            return exists;
        }

    }
}