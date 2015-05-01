using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interactivity;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Grep.Net.WPF.Client.Behaviours
{
    public class ContextMenuOnLeftClickBehavior : Behavior<ButtonBase>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Click += OnClick;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Click -= OnClick;
            base.OnDetaching();
        }

        private void OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var contextMenu = AssociatedObject.ContextMenu;
            if (contextMenu == null)
            {
                return;
            }

            contextMenu.PlacementTarget = AssociatedObject;
            contextMenu.Placement = PlacementMode.Bottom;
            contextMenu.IsOpen = true;
        }
    }
}
