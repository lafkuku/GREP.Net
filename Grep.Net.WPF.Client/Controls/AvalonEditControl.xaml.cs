using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using NLog;


namespace Grep.Net.WPF.Client.Controls
{
    /// <summary>
    /// Interaction logic for AvalonEditControl.xaml
    /// </summary>
    public partial class AvalonEditControl : TextEditor
    {
        public static DependencyProperty CaretOffsetProperty =
            DependencyProperty.Register("CaretOffset", typeof(int), typeof(AvalonEditControl),
            // binding changed callback: set value of underlying property
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (obj, args) =>
                {
                    AvalonEditControl target = (AvalonEditControl)obj;
                    target.CaretOffset = (int)args.NewValue;
                }));

        public new int CaretOffset
        {
            get
            {
                return base.CaretOffset;
            }
            set
            {
                base.CaretOffset = value;

                if (this.Document != null)
                {
                    var loc = this.Document.GetLocation(value);
                    this.ScrollTo(loc.Line, loc.Column);
                }
            }
        }

        public static void OnFileInfoChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            var control = (AvalonEditControl)sender;

            var oldFile = (String)eventArgs.OldValue;
            var newFile = (String)eventArgs.NewValue;

            if (oldFile == null || (oldFile != newFile))
            {
                try
                {
                    System.IO.FileInfo info = new System.IO.FileInfo(newFile); 
                    if (info.Exists)
                    {
                        System.IO.StreamReader sr = new System.IO.StreamReader(info.FullName);
                        String s = sr.ReadToEnd();
                        var high = HighlightingManager.Instance.GetDefinitionByExtension(info.Extension);

                        control.SyntaxHighlighting = high;
                        //TODO: Add code to editor. 
                        control.Document.Text = s;

                    }
                    else
                    {
                        //TODO: Log
                        logger.Error("File does not exists at path: " + info.FullName);
                    }
                }
                catch(Exception e)
                {
                    //TODO: Log
                    logger.Error(e);
                    throw;
                }
            }
            control.FilePath = newFile;
        }

        public static readonly DependencyProperty FilePathProperty = DependencyProperty.RegisterAttached("FilePath", typeof(String), typeof(AvalonEditControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnFileInfoChanged)));

        public String FilePath
        {
            get
            {
                return (String)GetValue(FilePathProperty);
            }

            set
            {
                SetValue(FilePathProperty, value);
            }
        }

        public static Logger logger = LogManager.GetCurrentClassLogger();
        public AvalonEditControl() : base()
        {
            this.ShowLineNumbers = true;
            this.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            this.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
           
        }
    }
}