// https://github.com/SeppPenner/CollectionExtensions

using System;
using System.Collections.Generic;
using System.Linq;

namespace HandyControl.Tools.Extension
{
    /// <summary>
    /// An extension for the <see cref="List{T}"/> class.
    /// </summary>
    public static class ListExtension
    {
        /// <summary>
        /// Clones the list.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>The new list.</returns>
        public static List<T> Clone<T>(this List<T> list)
        {
            CheckListIsNull(list);
            return list.ToList();
        }

        /// <summary>
        /// Adds a value if it doesn't exist yet.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="value">The value.</param>
        public static void AddIfNotExists<T>(this List<T> list, T value)
        {
            CheckListIsNull(list);

            if (!list.Contains(value))
            {
                list.Add(value);
            }
        }

        /// <summary>
        /// Updates a value.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="value">The value.</param>
        /// <param name="newValue">The new value.</param>
        public static void UpdateValue<T>(this IList<T> list, T value, T newValue)
        {
            CheckListAndValueIsNull(list, value);
            CheckValueIsNull(newValue);
            var index = list.IndexOf(value);
            list[index] = newValue;
        }

        /// <summary>
        /// Deletes a value if it exists.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="value">The value.</param>
        public static void DeleteIfExists<T>(this IList<T> list, T value)
        {
            CheckListAndValueIsNull(list, value);

            if (list.Contains(value))
            {
                list.Remove(value);
            }
        }

        /// <summary>
        /// Checks whether all values are <c>null</c> or not.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns><c>true</c> if all values are <c>null</c>.</returns>
        public static bool AreValuesNull<T>(this IList<T> list)
        {
            CheckListIsNull(list);
            return list.All(x => x == null);
        }

        /// <summary>
        /// Checks whether the list and the values are <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="value">The value.</param>
        private static void CheckListAndValueIsNull<T>(this IList<T> list, T value)
        {
            CheckListIsNull(list);
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
        /// Checks whether the list is <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="list">The list.</param>
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private static void CheckListIsNull<T>(this IList<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }
        }
    }
}
