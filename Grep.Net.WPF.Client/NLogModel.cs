using System;
using System.Linq;
using Caliburn.Micro;

namespace Grep.Net.WPF.Client
{
    public class NLogModel
    {
        public static BindableCollection<String> Messages { get; set; }

        static NLogModel()
        {
            Messages = new BindableCollection<String>();
        }

        public static void LogMessage(String level, String sourceMethod, String message)
        {
            String fmtMessage = string.Format("[{0}] - {1}: {2}", level, sourceMethod, message);

            Messages.Add(fmtMessage + "\r\n");
        }
    }
}