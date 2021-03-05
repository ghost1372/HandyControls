// https://github.com/SeppPenner/CollectionExtensions

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
namespace HandyControl.Tools.Extension
{
    public static class CollectionExtension
    {
        /// <summary>
        /// Clones the observable collection.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns>The new collection.</returns>
        public static ObservableCollection<T> Clone<T>(this ObservableCollection<T> collection)
        {
            var collectionToReturn = new ObservableCollection<T>();

            foreach (var val in collection)
            {
                collectionToReturn.Add(val);
            }

            return collectionToReturn;
        }

        /// <summary>
        /// Adds a value if it doesn't exist yet.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="value">The value.</param>
        public static void AddIfNotExists<T>(this ObservableCollection<T> collection, T value)
        {
            CheckObservableCollectionIsNull(collection);

            if (!collection.Contains(value))
            {
                collection.Add(value);
            }
        }

        /// <summary>
        /// Updates a value.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="value">The value.</param>
        /// <param name="newValue">The new value.</param>
        public static void UpdateValue<T>(this ObservableCollection<T> collection, T value, T newValue)
        {
            CheckObservableCollectionAndValueIsNull(collection, value);
            CheckValueIsNull(newValue);
            var index = collection.IndexOf(value);
            collection[index] = newValue;
        }

        /// <summary>
        /// Deletes a value if it exists.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="value">The value.</param>
        public static void DeleteIfExists<T>(this ObservableCollection<T> collection, T value)
        {
            CheckObservableCollectionAndValueIsNull(collection, value);

            if (collection.Contains(value))
            {
                collection.Remove(value);
            }
        }

        /// <summary>
        /// Checks whether all values are <c>null</c> or not.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns><c>true</c> if all values are <c>null</c>.</returns>
        public static bool AreValuesNull<T>(this ObservableCollection<T> collection)
        {
            CheckObservableCollectionIsNull(collection);
            return collection.All(x => x == null);
        }

        /// <summary>
        /// Checks whether the collection and the values are <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="value">The value.</param>
        private static void CheckObservableCollectionAndValueIsNull<T>(this ObservableCollection<T> collection, T value)
        {
            CheckObservableCollectionIsNull(collection);
            CheckValueIsNull(value);
        }

        /// <summary>
        /// Checks whether a value is <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="value">The value.</param>
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private static void CheckValueIsNull<T>(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Checks whether the collection is <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="collection">The collection.</param>
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private static void CheckObservableCollectionIsNull<T>(this ObservableCollection<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
        }

        /// <summary>
        /// This Extension Help you to Easily implement search, sort, and group operations
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="view"></param>
        /// <returns></returns>
        public static CollectionViewExtension<TSource> Shape<TSource>(this ICollectionView view)
        {
            return new CollectionViewExtension<TSource>(view);
        }

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
