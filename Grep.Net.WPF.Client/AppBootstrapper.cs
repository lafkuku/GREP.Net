namespace Grep.Net.WPF.Client
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;
    using System.Xml;
    using Caliburn.Micro;
    using Grep.Net.WPF.Client.ViewModels;
    using ICSharpCode.AvalonEdit.Highlighting;
    using ICSharpCode.AvalonEdit.Highlighting.Xshd;
    using System.ComponentModel.Composition.Primitives;

    public class AppBootstrapper : BootstrapperBase
    {
        private CompositionContainer container;
        
        private Window _mainWindow;

        /// <summary>
        /// By default, we are configured to use MEF
        /// </summary>
        protected override void Configure()
        {
            var catalog = new AggregateCatalog(
                AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>());

            container = new CompositionContainer(catalog);

            var batch = new CompositionBatch();

            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
     
            batch.AddExportedValue(container);
            batch.AddExportedValue(catalog);

            container.Compose(batch);
           
            var baseLocator = ViewLocator.LocateForModelType;

            ConventionManager.AddElementConvention<MenuItem>(MenuItem.ItemsSourceProperty, "DataContext", "Click");

            ViewLocator.LocateForModelType = (x, y, z) =>
            {
                return baseLocator(x, y, z);
            };
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            DisplayRootViewFor<IRoot>();

            _mainWindow = Application.MainWindow;
   
            //Set the icon
            String path = Grep.Net.Model.Utilities.GetWorkingDirectory();
            /*
            PngBitmapDecoder decoder = new PngBitmapDecoder(System.IO.File.Open(path, System.IO.FileMode.Open), BitmapCreateOptions.None, BitmapCacheOption.None);
            if (decoder != null && decoder.Frames[0] != null)
                _mainWindow.Icon = decoder.Frames[0];
            */
            //Set the title
            _mainWindow.Title = "GREP.Net";
          
            //Load syntax highlights. 

            //TODO: Move this to the model project.. 
            string dir = @".\Highlighting"; // Insert the path to your xshd-files.
            if (Directory.Exists(dir))
            {
                foreach (String file in Directory.GetFiles(dir))
                {
                    if (Path.GetExtension(file).ToLower().Contains("xshd"))
                    {
                        using (StreamReader sr = new StreamReader(File.Open(file, FileMode.Open)))
                        {
                            using (XmlReader reader = XmlReader.Create(sr))
                            {
                                var ret = HighlightingLoader.LoadXshd(reader);
                                var def = HighlightingLoader.Load(ret, HighlightingManager.Instance);

                                HighlightingManager.Instance.RegisterHighlighting(ret.Name, ret.Extensions.ToArray(), def);
                            }
                        }
                    }
                }
            }
            //LoadLayout(RootViewModel.Instance.GetDockingManager()); 
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = container.GetExportedValues<object>(contract);

            if (exports.Count() > 0)
                return exports.First();

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }
        
        protected override void BuildUp(object instance)
        {
            container.SatisfyImportsOnce(instance);
        }

        private void SaveLayout(Xceed.Wpf.AvalonDock.DockingManager dm)
        {
            if (dm == null)
                throw new ArgumentNullException();

            var layoutSerializer = new Xceed.Wpf.AvalonDock.Layout.Serialization.XmlLayoutSerializer(dm);
            StringWriter textWriter = new StringWriter();
            layoutSerializer.Serialize(textWriter);
            Properties.Settings.Default.DockingManagerLayout = textWriter.ToString();
        }

        private void LoadLayout(Xceed.Wpf.AvalonDock.DockingManager dm)
        {
            if (dm == null)
            {
                return;
            }

            var layoutSerialiser = new Xceed.Wpf.AvalonDock.Layout.Serialization.XmlLayoutSerializer(dm);

            string layoutToRestore = Properties.Settings.Default.DockingManagerLayout;
            if (!String.IsNullOrEmpty(layoutToRestore))
            {
                StringReader textReader = new StringReader(layoutToRestore);
                layoutSerialiser.Deserialize(textReader);
            }
        }
        
        protected override void OnExit(object sender, EventArgs e)
        {
            base.OnExit(sender, e);
            //Shut down visuals (So we have time to save the layout
            RootViewModel rvm = (RootViewModel)this.container.GetExport<IRoot>().Value;
            if (rvm != null &&
                Properties.Settings.Default.SaveLayout == true)
            {
                Xceed.Wpf.AvalonDock.DockingManager dm = rvm.GetDockingManager(); 
                if (dm != null)
                {
                    //SaveLayout(dm);
                }
            }

            Properties.Settings.Default.Save();
   
            Grep.Net.Model.GTApplication.Instance.Shutdown(); 
        }

        protected override void OnUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            base.OnUnhandledException(sender, e);
        }
        public AppBootstrapper() : base(true)
        {
            Initialize();
        }
    }
}