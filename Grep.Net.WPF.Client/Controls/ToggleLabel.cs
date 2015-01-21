using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Grep.Net.WPF.Client.Controls
{
    public class ToggleLabel : Label
    {
        public static readonly RoutedEvent ClickEvent;

        public static Style TheStyle { get; set; }

        public new static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(Object), typeof(ToggleLabel), new FrameworkPropertyMetadata(null));

        public new Object Content
        {
            get
            {
                return GetValue(ContentProperty);
            }

            set
            {
                SetValue(ContentProperty, value);
            }
        }

        public static readonly DependencyProperty AltContentProperty = DependencyProperty.Register("AltContent", typeof(Object), typeof(ToggleLabel), new FrameworkPropertyMetadata(null));

        public Object AltContent
        {
            get
            {
                return GetValue(AltContentProperty);
            }

            set
            {
                SetValue(AltContentProperty, value);
            }
        }

        public static readonly DependencyProperty ToggledProperty = DependencyProperty.Register("Toggled", typeof(bool), typeof(ToggleLabel), new FrameworkPropertyMetadata(null));

        public bool Toggled
        {
            get
            {
                return (bool)GetValue(ToggledProperty); 
            }

            set
            {
                SetValue(ToggledProperty, value); 
            }
        }

        static ToggleLabel()
        {
            ClickEvent = ButtonBase.ClickEvent.AddOwner(typeof(ToggleLabel));
            TheStyle = new Style(typeof(ToggleLabel));
            TheStyle.Setters.Add(new Setter(Label.ForegroundProperty, new SolidColorBrush(Color.FromRgb(0, 0, 255))));
        }

        public ToggleLabel()
        {
            this.Initialized += new EventHandler(ToggleLabel_Initialized);
        }

        private void ToggleLabel_Initialized(object sender, EventArgs e)
        {
            this.Style = TheStyle;
            object cp = GetValue(ContentProperty);

            if (cp != null)
            {
                base.Content = cp;
            }
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
            Toggled = !Toggled;

            if (Toggled)
                base.Content = AltContent;
            else
                base.Content = Content;

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