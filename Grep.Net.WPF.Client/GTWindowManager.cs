using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using Grep.Net.WPF.Client.ViewModels;

namespace Grep.Net.WPF.Client
{
    public class GTWindowManager : WindowManager
    {
        #region Singleton
        
        private static GTWindowManager _instance;
        
        public static GTWindowManager Instance
        {
            get
            {
                lock (_lok)
                {
                    if (_instance == null)
                    {
                        _instance = new GTWindowManager();
                    }
                }
                return _instance;
            }
        }
        
        private static object _lok = new object();
        
        #endregion
        
        private GTWindowManager() : base()
        {
        }
        
        public bool? ShowOkCanelDialog(PropertyChangedBase rootModel, int height, int width, string okText = "Ok", string cancelText = "Cancel", object context = null, IDictionary<string, object> settings = null)
        {
            if (settings == null)
            {
                settings = new Dictionary<String, Object>();
            }
            
            if (!settings.Keys.Contains("Width"))
            {
                settings.Add("Width", width);
            }
            if (!settings.Keys.Contains("Height"))
            {
                settings.Add("Height", height);
            }
            
            OkCancelDialogViewModel vm = new OkCancelDialogViewModel()
            {
                ViewModel = rootModel,
                OkText = okText,
                CancelText = cancelText
            }; 
            
            return base.ShowDialog(vm, context, settings);
        }
        
        public bool? ShowDialog(object rootModel, int height, int width, object context = null, IDictionary<string, object> settings = null)
        {
            if (settings == null)
            {
                settings = new Dictionary<String, Object>();
            }
            
            if (!settings.Keys.Contains("Width"))
            {
                settings.Add("Width", width);
            }
            if (!settings.Keys.Contains("Height"))
            {
                settings.Add("Height", height);
            }
            return base.ShowDialog(rootModel, context, settings);
        }
        
        public void ShowWindow(object rootModel, int height, int width, object context = null, IDictionary<string, object> settings = null)
        {
            if (settings == null)
            {
                settings = new Dictionary<String, Object>(); 
            }
            
            if (!settings.Keys.Contains("Width"))
            {
                settings.Add("Width", width); 
            }
            if (!settings.Keys.Contains("Height"))
            {
                settings.Add("Height", height);
            }
            base.ShowWindow(rootModel, context, settings);
        }
        
        protected override Window InferOwnerOf(Window window)
        {
            if (Application.Current != null)
            {
                Window window2 = (from x in Application.Current.Windows.OfType<Window>()
                                  where x.IsActive
                                  select x).FirstOrDefault<Window>() ?? Application.Current.MainWindow;
                if (window2 != window)
                {
                    return window2;
                }
            }
            return null;
        }
        
        /// <summary>
        /// Makes sure the view is a window and is wrapped by one.
        /// </summary>
        /// <param name="model">The view model.</param>
        /// <param name="view">The view.</param>
        /// <param name="isDialog">Whethor or not the window is being shown as a dialog.</param>
        /// <returns>The window.</returns>
        protected override Window EnsureWindow(object model, object view, bool isDialog)
        {
            var window = view as Window;
            
            if (view is Window)
            {
                var owner = InferOwnerOf(window);
                if (owner != null && isDialog)
                    window.Owner = owner;
            }
            else if (view is UserControl)
            {
                window = new Window();
                
                window.SetValue(View.IsGeneratedProperty, true);             
                window.Content = view;
                
                var owner = InferOwnerOf(window);
                if (owner != null && isDialog)
                {
                    window.Owner = owner;
                    window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                }
                else
                {
                    window.Owner = owner;
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    window.Focus();
                }
            }
            
            return window;
        }
    }
}