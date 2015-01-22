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

            HighlightSelectedSameWords transformer = new HighlightSelectedSameWords(this);
            this.TextArea.TextView.LineTransformers.Add(transformer);

            this.TextArea.SelectionChanged += (x, y) => this.TextArea.TextView.Redraw();
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
    }
}