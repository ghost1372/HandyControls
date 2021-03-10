using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace HandyControl.Tools.Extension
{
    public static class EnumerableExtensions
    {
        public static void ForEach<TElement>(this IEnumerable<TElement> @this, Action<TElement> action)
        {
            foreach (var element in @this)
                action(element);
        }

        public static void ReverseForEach<TElement>(this IEnumerable<TElement> @this, Action<TElement> action)
        {
            var list = @this as IList<TElement>;
            if (list is null)
                list = @this.ToList();

            for (var index = list.Count - 1; index >= 0; index--)
                action(list[index]);
        }

        /// <summary>
        /// This Extension Help you to Easily implement search, sort, and group operations
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static CollectionViewExtension<TSource> ShapeView<TSource>(this IEnumerable<TSource> source)
        {
            var view = CollectionViewSource.GetDefaultView(source);
            return new CollectionViewExtension<TSource>(view);
        }
    }
}
