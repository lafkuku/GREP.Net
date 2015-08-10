using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Grep.Net.Entities;
using Grep.Net.WPF.Client.CoRoutines;
using Grep.Net.WPF.Client.Controls;
using ICSharpCode.AvalonEdit.Search;
using NLog;
using Grep.Net.WPF.Client.Commands;
using System.Windows.Threading;

namespace Grep.Net.WPF.Client.ViewModels
{
    public class MatchInfoEditorViewModel : Screen
    {
        public static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private MatchInfo _matchInfo;

        public MatchInfo MatchInfo
        {
            get
            {
                return _matchInfo;
            }
            set
            {
                _matchInfo = value;
                UpdateEditor();
                if (_matchInfo != null)
                {
                    Name = MatchInfo.FileInfo.Name;
                }
               
                NotifyOfPropertyChange(() => MatchInfo);
            }
        }

        private String _searchText;

        public String SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                _searchText = value;
                NotifyOfPropertyChange(() => SearchText);
            }
        }

        public AvalonEditControl Editor { get; set; }

        public SearchPanel SearchPanel { get; set; }

        private bool _closeable;

        public bool Closeable
        {
            get
            {
                return _closeable;
            }
            set
            {
                _closeable = value;
                NotifyOfPropertyChange(() => Closeable); 
            }
        }

        private String _name;

        public String Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        RootViewModel RootViewMdoel { get; set;  }

        public MatchInfoEditorViewModel(RootViewModel rvm)
        {
            this.RootViewMdoel = rvm;
            this.Closeable = true;
            this.Editor = new AvalonEditControl();
            this.Editor.MaxFileSize = 100000; 
           
            
            SearchPanel = SearchPanel.Install(this.Editor.TextArea);
 
            DisplayName = "MatchInfo Details";
        }

        public void UpdateEditor()
        {
            if (_matchInfo != null)
            {
                if (Editor.FilePath != _matchInfo.FileInfo.FullName)
                {
                    Editor.FilePath = _matchInfo.FileInfo.FullName;
                }
                try
                {

                    //Well this is ugly, so i best explain or i'll forget. 

                    //The editor will load the document in an async fashion, as such, we need to also tell it update the scroll posistion. 
                    //So we dispatch an action when the async load document completes, but wait that's not all
                    //We also need to do the scrolling from the UI thread, so our action then queues an action for the UI thread to do the scroll. Whew. 
                    //Technically a race condition still exits but would be rare
                    var scrollTo = new System.Action(()=>{
                        //Verify that we can actually scroll to the line. 
                        if (this.Editor.Document.LineCount >= _matchInfo.LineNumber)
                        {
                            var ln = this.Editor.Document.GetLineByNumber(_matchInfo.LineNumber);
                            if (ln != null)
                            {
                                //Editor.UpdateLayout();   
                                Editor.ScrollTo(ln.LineNumber, 0);
                                Editor.Select(ln.Offset, ln.Length);
                            }
                        }
                      
                    });
                    if (Editor.TextLoadingTask == null || Editor.TextLoadingTask.IsCompleted)
                    {
                        scrollTo();
                    }
                    else
                    {
                        Editor.TextLoadingTask.ContinueWith((x) =>
                        {
                            Editor.Dispatcher.BeginInvoke(new System.Action(delegate
                            {
                                scrollTo();
                            }), DispatcherPriority.Normal);
                          
                        });
                    }
                   
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
            }
        }

        private AvalonEditControl GetTextEditor()
        {
            var view = this.GetView() as System.Windows.Controls.UserControl;
            if (view == null)
                return null;

            var t = GetFrameworkElementByType(view.Content.GetType(), typeof(AvalonEditControl));

            return t.GetValue(view.Content) as AvalonEditControl;
        }

        private FieldInfo GetFrameworkElementByType(Type fromType, Type toFetch)
        {
            List<FieldInfo> result;

            result = fromType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(i => typeof(FrameworkElement).IsAssignableFrom(i.FieldType)).ToList();

            var t = result.FirstOrDefault((x) => x.FieldType.FullName == toFetch.FullName);

            return t;
        }

        public void CopyFilePath()
        {
            if (MatchInfo != null && MatchInfo.FileInfo != null)
            {
                try
                {
                    Clipboard.SetText(MatchInfo.FileInfo.FullName);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        public void Search()
        {
            String selectedText = this.Editor.SelectedText;
            if (!string.IsNullOrEmpty(selectedText))
            {
                //TODO: This should really be a utility command. 
                this.RootViewMdoel.Search(selectedText);
            }
        }

        public void Copy()
        {
            this.Editor.Copy(); 
        }
       
    }
}