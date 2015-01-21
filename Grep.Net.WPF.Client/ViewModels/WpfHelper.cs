using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Grep.Net.WPF.Client.Utilities
{
    public class WpfHelper
    {
        public static DependencyObject GetParent(DependencyObject obj)
        {
            if (obj == null)
                return null;

            ContentElement ce = obj as ContentElement;
            if (ce != null)
            {
                DependencyObject parent = ContentOperations.GetParent(ce);
                if (parent != null)
                    return parent;

                FrameworkContentElement fce = ce as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            FrameworkElement fe = obj as FrameworkElement;
            if (fe != null)
            {
                DependencyObject parent = fe.Parent;
                if (parent != null)
                    return parent;
            }

            return VisualTreeHelper.GetParent(obj);
        }

        /// <summary>
        /// Returns Parent of specified type for the current DependencyObject, or null if no parent of that type exists.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T FindAncestorOrSelf<T>(DependencyObject obj) where T : DependencyObject
        {
            while (obj != null)
            {
                T objTest = obj as T;
                if (objTest != null)
                    return objTest;
                obj = GetParent(obj);
            }
            return null;
        }
    }
}