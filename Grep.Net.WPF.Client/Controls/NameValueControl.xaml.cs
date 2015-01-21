using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Grep.Net.WPF.Client.Controls
{
    /// <summary>
    /// Interaction logic for NameValueControl.xaml
    /// </summary>
    public partial class NameValueControl : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(String), typeof(NameValueControl), new FrameworkPropertyMetadata(TextChanged));

        public String Text
        {
            get
            {
                return (String)GetValue(TextProperty);
            }

            set
            {
                this._lblName.Content = value;
                SetValue(TextProperty, value);
            }
        }

        private static void TextChanged(
            DependencyObject sender,
            DependencyPropertyChangedEventArgs eventArgs)
        {
            var control = (NameValueControl)sender;

            var oldString = (String)eventArgs.OldValue;
            var newString = (String)eventArgs.NewValue;

            control._lblName.Content = newString;
        }

        public static new readonly DependencyProperty ToolTipProperty = DependencyProperty.Register("ToolTip", typeof(String), typeof(NameValueControl), new FrameworkPropertyMetadata(ToolTipChanged));

        public new String ToolTip
        {
            get
            {
                return (String)GetValue(TextProperty);
            }

            set
            {
                SetValue(TextProperty, value);
            }
        }

        private static void ToolTipChanged(
            DependencyObject sender,
            DependencyPropertyChangedEventArgs eventArgs)
        {
            var control = (NameValueControl)sender;

            var oldString = (String)eventArgs.OldValue;
            var newString = (String)eventArgs.NewValue;

            control._lblName.ToolTip = newString;
        }

        public static new readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(Object), typeof(NameValueControl), new FrameworkPropertyMetadata(ContentChanged));

        public new object Content
        {
            get
            {
                return (ContentControl)GetValue(ContentProperty);
            }

            set
            {
                this._cntPresneter.Content = value;
                SetValue(TextProperty, value);
            }
        }

        private static void ContentChanged(
            DependencyObject sender,
            DependencyPropertyChangedEventArgs eventArgs)
        {
            var control = (NameValueControl)sender;
            control._cntPresneter.Content = eventArgs.NewValue ;
        }

        public NameValueControl()
        {
            InitializeComponent();
        }
    }
}