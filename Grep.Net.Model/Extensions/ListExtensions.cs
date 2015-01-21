using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Grep.Net.Model.Extensions
{
    public static class ListExtensions
    {
        public static void ForEach<T>(this IList<T> list, Action<T> function)
        {
            foreach (T item in list)
            {
                function(item);
            }
        }

        public static void ForEach(this IList list, Action<Object> function)
        {
            foreach (Object item in list)
            {
                function(item);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> list, Action<T> function)
        {
            foreach (T item in list)
            {
                function(item);
            }
        }
    }
}