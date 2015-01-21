using System;
using System.Linq;
using Xceed.Wpf.AvalonDock.Layout;

namespace Grep.Net.WPF.Client.Docking
{
    /// <summary>
    /// This is what is used to actually put the correct viewmodels into the correct AnchorablePanes... Pretty stupid i know.. Oh well! 
    /// </summary>
    public class LayoutStrategy : ILayoutUpdateStrategy
    {
        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
        {
            
            return false;
        }

        public bool InsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
        {
            return false;
        }

        public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
        {
        }

        public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
        {
        }

        public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
        {
            string destPaneName = string.Empty;

            destPaneName = "Document";

            var toolsPane = layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (toolsPane != null && destPaneName == "Document")
            {
                toolsPane.Children.Add(anchorableToShow);
                return true;
            }
            return false;
        }
    }
}