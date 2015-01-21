using System;
using System.Collections;
using System.Linq;
using System.Windows.Input;
using Grep.Net.Data;
using Grep.Net.Entities;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Grep.Net.WPF.Client.Commands
{
    public class UtilityCommands
    {
        public static ICommand ImportEntity { get; set; }

        public static ICommand ExportEntity { get; set; }

        public static ICommand GrepSearch { get; set; }

        static UtilityCommands()
        {
            ImportEntity = new DelegateCommand((x) =>
            {
                Type t = null;

                switch (x as String)
                {
                    case "FileTypeDefinition":
                        t = typeof(FileTypeDefinition);
                        break;
                    case "PatternPackage":
                        t = typeof(PatternPackage);
                        break;
                    default:
                        return;
                }
                var dialog = new CommonOpenFileDialog();

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    Object o = SerializationHelper.DeserializeXmlFromFile(t, dialog.FileName);
                    IList list = Grep.Net.Model.GTApplication.Instance.DataModel.GetListFor(o.GetType());
                    if (!list.Contains(o))
                    {
                        list.Add(o);
                    }
                    else
                    {
                        return;
                    }
                }
            });

            ExportEntity = new DelegateCommand((x) =>
            {
                if (x != null)
                {
                    var dialog = new CommonSaveFileDialog();
                   
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        SerializationHelper.SerializeIntoXmlFile(dialog.FileName, x);
                    }
                }
            });

            GrepSearch = new DelegateCommand((x) =>
            {
                
            });
        }
    }
}