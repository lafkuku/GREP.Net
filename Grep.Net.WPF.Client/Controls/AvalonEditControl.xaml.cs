using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using NLog;
using System.Globalization;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;

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

        public Task TextLoadingTask { get; set; }
        public CancellationTokenSource TextLoadingToken { get; set; }
        public int MaxFileSize { get; set; }

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
                        bool isTooLarge = (info.Length > control.MaxFileSize);

                        if(isTooLarge){
                            MessageBoxResult result = MessageBox.Show("File size too large, display anyways?", "File too large", MessageBoxButton.YesNo);

                            if (result == MessageBoxResult.Yes)
                            {
                                isTooLarge = false;
                            }
                        }


                        if (control.TextLoadingTask != null && !control.TextLoadingTask.IsCompleted)
                        {
                            control.TextLoadingToken.Cancel();
                        }

                        if(!isTooLarge){

                            control.Document.Text = "Loading...";
                            var highLighter = HighlightingManager.Instance.GetDefinitionByExtension(info.Extension);
                            control.SyntaxHighlighting = highLighter;
                            
                            //Create a loading task. 
                            control.TextLoadingTask = Task.Factory.StartNew(async ()=>{
                                TextDocument td = new TextDocument(); 
                               
                                System.IO.StreamReader sr = new System.IO.StreamReader(info.FullName);
                                String s = sr.ReadToEnd();
                               
                                //TODO: Add code to editor. 
                                td.Text = s;

                                td.SetOwnerThread(control.Dispatcher.Thread);
                              
                                var operation = control.Dispatcher.BeginInvoke(new Action(delegate
                                {
                                    control.Document = td;
                                   
                                }), DispatcherPriority.Normal);
                                operation.Wait(new TimeSpan(0,0,5));
                                if (operation.Status != DispatcherOperationStatus.Completed){
                                    operation.Abort();
                                    control.Dispatcher.Invoke(new Action(delegate
                                    {
                                        TextDocument timedout = new TextDocument();
                                        timedout.Text = "Timedout loading document";
                                        control.Document = timedout;
                                    }), DispatcherPriority.Normal);
                                }
                               
                            }, control.TextLoadingToken.Token);
                        }
                        else
                        {
                            control.Document.Text = "File too large";
                        }
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
            this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            
            HighlightSelectedSameWords transformer = new HighlightSelectedSameWords(this);
            this.TextArea.TextView.LineTransformers.Add(transformer);
            
            this.TextArea.SelectionChanged += (x, y) => this.TextArea.TextView.Redraw();
            this.TextArea.SelectionBrush = Brushes.SlateGray;
            this.TextLoadingToken = new CancellationTokenSource(); 
        }
    }

    /// <summary>
    /// This class will focus on other words the same as the selected text. 
    /// </summary>
    public class HighlightSelectedSameWords : DocumentColorizingTransformer
    {
        TextEditor TextEditor { get; set; }

        public HighlightSelectedSameWords(TextEditor te)
        {
            TextEditor = te;
        }
        protected override void ColorizeLine(DocumentLine line)
        {
            
            int lineStartOffset = line.Offset;
            string text = CurrentContext.Document.GetText(line);
            int start = 0;
            int index;
            if (this.TextEditor == null || String.IsNullOrEmpty(this.TextEditor.SelectedText))
            {
                return;
            }
            try
            {
                while ((index = text.IndexOf(this.TextEditor.SelectedText, start)) >= 0)
                {
                    //Dont screw with the selected text

                    if (!(lineStartOffset + index == this.TextEditor.SelectionStart))
                    {

                        base.ChangeLinePart(
                            lineStartOffset + index, // startOffset
                            lineStartOffset + index + this.TextEditor.SelectedText.Length, // endOffset
                            (VisualLineElement element) =>
                            {
                                // This lambda gets called once for every VisualLineElement
                                // between the specified offsets.
                                Typeface tf = element.TextRunProperties.Typeface;

                                // Replace the typeface with a modified version of
                                // the same typeface
                                element.TextRunProperties.SetTypeface(new Typeface(
                                    tf.FontFamily,
                                    FontStyles.Italic,
                                    FontWeights.Bold,
                                    tf.Stretch
                                ));

                                element.TextRunProperties.SetBackgroundBrush(Brushes.Blue);
                                element.TextRunProperties.SetForegroundBrush(Brushes.Ivory);
                                //element.TextRunProperties.SetFontRenderingEmSize(element.TextRunProperties.FontRenderingEmSize * 1.5);
                                element.TextRunProperties.SetFontHintingEmSize(element.TextRunProperties.FontRenderingEmSize * 1.5);
                            });
                    }



                    start = index + 1; // search for next occurrence
                }
            }
            catch (Exception e)
            {
                
            }
            
        }
    }
}