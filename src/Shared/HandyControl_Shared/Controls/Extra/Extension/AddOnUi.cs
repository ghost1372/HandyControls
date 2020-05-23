using System;
using System.Collections.Generic;
using System.Windows;
namespace HandyControl.Tools.Extension
{
    public static class AddOnUi
    {
        /// <summary>
        /// This Extension Method Help you to Add Items into ObservableCollection from Another Thread
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        public static void AddOnUI<T>(this ICollection<T> collection, T item)
        {
            Action<T> addMethod = collection.Add;
            Application.Current.Dispatcher.BeginInvoke(addMethod, item);
        }
    }
}
