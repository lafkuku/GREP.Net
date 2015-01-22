using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.IO;

namespace Grep.Net.Powershell.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            InitialSessionState initial = InitialSessionState.CreateDefault();
            string path = Directory.GetCurrentDirectory() + @"\Grep.Net.Powershell.dll";
            if (System.IO.File.Exists(path))
                initial.ImportPSModule(new string[] {path});
            else
                return;
            Runspace runspace = RunspaceFactory.CreateRunspace(initial);
            runspace.Open();

            RunspaceInvoke invoker = new RunspaceInvoke(runspace);
            Collection<PSObject> results = invoker.Invoke(@"Invoke-Grep 'C:\Users\Kuku\Documents' Secrets Config");

        }
    }
}
