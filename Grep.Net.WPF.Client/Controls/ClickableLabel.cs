using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Grep.Net.WPF.Client.Controls
{
    public class ClickableLabel : Label
    {
        public static readonly RoutedEvent ClickEvent;

        public static Style TheStyle { get; set; }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(String), typeof(ClickableLabel), new FrameworkPropertyMetadata(TextChanged));

        public String Text
        {
            get
            {
                return (String)GetValue(TextProperty);
            }

            set
            {
                SetText(value);
                SetValue(TextProperty, value);
            }
        }

        private static void TextChanged(
            DependencyObject sender,
            DependencyPropertyChangedEventArgs eventArgs)
        {
            var control = (ClickableLabel)sender;

            var oldString = (String)eventArgs.OldValue;
            var newString = (String)eventArgs.NewValue;

            control.Text = newString;
        }

        static ClickableLabel()
        {
            ClickEvent = ButtonBase.ClickEvent.AddOwner(typeof(ClickableLabel));
        }

        public ClickableLabel()
        {
            this.Initialized += new EventHandler(ClickableLabel_Initialized);
        }

        private void ClickableLabel_Initialized(object sender, EventArgs e)
        {
            SetText(this.Text);
        }

        private void SetText(string text)
        {
            TextDecoration td = new TextDecoration();
            Pen myPen = new Pen(Brushes.Blue, 1);

            td.Pen = myPen;
            td.PenThicknessUnit = new TextDecorationUnit();
            TextDecorationCollection tdc = new TextDecorationCollection();
            tdc.Add(td);

            TextBlock textBlock = new TextBlock();
            textBlock.TextDecorations = tdc;
            textBlock.Text = text;
            textBlock.Foreground = Brushes.Blue;
            this.Content = textBlock;
        }

        public event RoutedEventHandler Click
        {
            add
            {
                AddHandler(ClickEvent, value);
            }
            remove
            {
                RemoveHandler(ClickEvent, value);
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            Mouse.OverrideCursor = null;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            Mouse.OverrideCursor = Cursors.Hand;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            CaptureMouse();
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
                if (IsMouseOver)
                    RaiseEvent(new RoutedEventArgs(ClickEvent, this));
            }
        }
    }
}