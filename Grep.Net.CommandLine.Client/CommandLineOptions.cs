using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Infrastructure;
using CommandLine.Parsing;

namespace GTServices.CommandLine.Client
{
    public class CommandLineOptions
    {
  
        [OptionArray('l', "Languages", DefaultValue = null, HelpText = "Example Usage: -l All")]
        public string[] Languages { get; set; }

        [OptionArray('c', "Categories", DefaultValue = null, HelpText = "Example Usage: -l All")]
        public string[] Issues { get; set; }

        [Option('L', "List", DefaultValue = null, HelpText = "List Mode.\n You Can list: patterns, categories or languages")]
        public string ListOptions { get; set; }

        [Option('o', "output", DefaultValue = null, HelpText = "Output file for results.")]
        public string OutFile { get; set; }

        [Option('d', "directory", DefaultValue = null, HelpText = "RootSearch Directory")]
        public string RootSearchDirectory { get; set; }

        [HelpOption('h')]
        public string GetUsage()
        {
            // this without using CommandLine.Text
            var usage = new StringBuilder();
            usage.AppendLine("");
            usage.AppendLine("CS^2 CommandLine Application 0.4.0");
            usage.AppendLine("Usage: CS2.exe options..");
            usage.AppendLine("");
            
            usage.AppendLine("***********Query Modes***********");
            usage.AppendLine("");
            usage.AppendLine("List Mode: -L {categories, languages}");
            usage.AppendLine("");
            usage.AppendLine("Examples:");
            usage.AppendLine("");
            usage.AppendLine("Query for all Categories - cs2 -L categories");
            usage.AppendLine("Query for all Languages  - cs2 -L languages");
            usage.AppendLine("");

            usage.AppendLine("***********Searching (Grepping)***********");
            usage.AppendLine("");

            usage.AppendLine("Langauges      : -l python ruby c#");
            usage.AppendLine("Categories     : -c xss_sinks xss_sources secrets");
            usage.AppendLine("OutputFile     : -o outputFile");
            usage.AppendLine("Start Directory: -d \"c:\\starthere\"");
            usage.AppendLine("");
            usage.AppendLine("Examples:");
            usage.AppendLine("");
            usage.AppendLine("cs2 -c XSS_Sinks XSS_Sources -d \"c:\\dev\\project\"");
            usage.AppendLine("cs2 -c xss_sinks xss_sources -l C#");
            usage.AppendLine("cs2 -l C# -o results.xml");
            usage.AppendLine("cs2 -l Language [Soon to be file type container] -o OutFile.xml");
            usage.AppendLine("");
            usage.AppendLine("");
            return usage.ToString();
        }

    }
}
