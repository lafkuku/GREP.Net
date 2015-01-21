using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Parsing;
using CommandLine.Infrastructure;
using CommandLine.Text;
using GTServices.Entities;
using GTServices.Model.CoRoutine;
using System.IO;

namespace GTServices.CommandLine.Client
{
    class ApplicationMain
    {
        static GTServices.Model.GTApplication AppModel { get { return GTServices.Model.GTApplication.Instance; } } 
        static void Main(string[] args)
        {
            Console.WriteLine("");

            Console.WriteLine("");
            Console.WriteLine("Welcome to CS^2 CommandLine tool. Brought to you by Casaba Security LLC.");
            var options = new CommandLineOptions();
           
            var parser = new Parser((x) =>
            {
                x.CaseSensitive = true;
                x.IgnoreUnknownArguments = true;
                x.HelpWriter = Console.Out;
            });
            if (parser.ParseArgumentsStrict(args, options))
            {
                // consume Options instance properties
                if (options.ListOptions != null)
                {
                    HandleListOptions(options);
                }
                else
                {
                    if ((options.Languages != null || options.Issues != null))
                    {
                     
                    }
                }
            }
#if DEBUG
            Console.Out.WriteLine("Hit return to Close"); 
            Console.ReadLine(); 
#endif
        }

        static void HandleListOptions(CommandLineOptions options)
        {
            Console.Out.WriteLine("List Mode - All other options will be ignored." + options.ListOptions);
            Console.Out.WriteLine("This could take a moment. Please wait..");
            Console.Out.WriteLine("");
            switch (options.ListOptions.ToLower())
            {
               
            }
          
        }



       
      
        
    }
}
