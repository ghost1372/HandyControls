#region Copyright information
// <copyright file="ParentChangedNotifierHelper.cs">
//     Licensed under Microsoft Public License (Ms-PL)
//     https://github.com/XAMLMarkupExtensions/WPFLocalizationExtension/blob/master/LICENSE
// </copyright>
// <author>Bernhard Millauer</author>
#endregion

namespace HandyControl.Tools.DynamicLanguage
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    #endregion

    /// <summary>
    /// A memory safe dictionary storage for <see cref="ParentChangedNotifier"/> instances.
    /// </summary>
    internal class ParentNotifiers
    {
        readonly Dictionary<WeakReference<DependencyObject>, WeakReference<ParentChangedNotifier>> _inner =
            new Dictionary<WeakReference<DependencyObject>, WeakReference<ParentChangedNotifier>>();

        /// <summary>
        /// Check, if it contains the key.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <returns>True, if the key exists.</returns>
        public bool ContainsKey(DependencyObject target)
        {
            return _inner.Keys.Any(x => x.TryGetTarget(out var item) && ReferenceEquals(item, target));
        }

        /// <summary>
        /// Removes the entry.
        /// </summary>
        /// <param name="target">The target object.</param>
		public void Remove(DependencyObject target)
        {
            WeakReference<DependencyObject> key = _inner.Keys.SingleOrDefault(x => x.TryGetTarget(out var item) &&  ReferenceEquals(item, target));
            if (key == null)
                return;

            if (_inner[key].TryGetTarget(out var notifier))
            {
                notifier?.Dispose();
            }

            _inner.Remove(key);
        }

        /// <summary>
        /// Adds the key-value-pair.
        /// </summary>
        /// <param name="target">The target key object.</param>
        /// <param name="parentChangedNotifier">The notifier.</param>
		public void Add(DependencyObject target, ParentChangedNotifier parentChangedNotifier)
        {
            _inner.Add(new WeakReference<DependencyObject>(target), new WeakReference<ParentChangedNotifier>(parentChangedNotifier));
        }
    }
}