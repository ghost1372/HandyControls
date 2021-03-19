#region Copyright information
// <copyright file="ParentChangedNotifierHelper.cs">
//     Licensed under Microsoft Public License (Ms-PL)
//     https://github.com/XAMLMarkupExtensions/WPFLocalizationExtension/blob/master/LICENSE
// </copyright>
// <author>Uwe Mayer</author>
#endregion

namespace HandyControl.Tools.DynamicLanguage
{
    #region Usings
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    #endregion

    /// <summary>
    /// Extension methods for <see cref="DependencyObject"/> in conjunction with the <see cref="XAMLMarkupExtensions.Base.ParentChangedNotifier"/>.
    /// </summary>
    internal static class ParentChangedNotifierHelper
    {
        /// <summary>
        /// Tries to get a value that is stored somewhere in the visual tree above this <see cref="DependencyObject"/>.
        /// <para>If this is not available, it will register a <see cref="XAMLMarkupExtensions.Base.ParentChangedNotifier"/> on the last element.</para>
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="target">The <see cref="DependencyObject"/>.</param>
        /// <param name="getFunction">The function that gets the value from a <see cref="DependencyObject"/>.</param>
        /// <param name="parentChangedAction">The notification action on the change event of the Parent property.</param>
        /// <param name="parentNotifiers">A dictionary of already registered notifiers.</param>
        /// <returns>The value, if possible.</returns>
        public static T GetValueOrRegisterParentNotifier<T>(
            this DependencyObject target,
            Func<DependencyObject, T> getFunction,
            Action<DependencyObject> parentChangedAction,
            ParentNotifiers parentNotifiers)
        {
            var ret = default(T);

            if (target == null) return ret;

            var depObj = target;
            var weakTarget = new WeakReference(target);

            while (ret == null)
            {
                // Try to get the value using the provided GetFunction.
                ret = getFunction(depObj);

                if (ret != null)
                    parentNotifiers.Remove(target);

                // Try to get the parent using the visual tree helper. This may fail on some occations.
                if (depObj is System.Windows.Controls.ToolTip)
                    break;

                if (!(depObj is Visual) && !(depObj is Visual3D) && !(depObj is FrameworkContentElement))
                    break;

                if (depObj is Window)
                    break;

                DependencyObject depObjParent;

                if (depObj is FrameworkContentElement element)
                    depObjParent = element.Parent;
                else
                {
                    try
                    {
                        depObjParent = depObj.GetParent(false);
                    }
                    catch
                    {
                        depObjParent = null;
                    }
                }

                if (depObjParent == null)
                {
                    try
                    {
                        depObjParent = depObj.GetParent(true);
                    }
                    catch
                    {
                        break;
                    }
                }

                // If this failed, try again using the Parent property (sometimes this is not covered by the VisualTreeHelper class :-P.
                if (depObjParent == null && depObj is FrameworkElement)
                    depObjParent = ((FrameworkElement)depObj).Parent;

                if (ret == null && depObjParent == null)
                {
                    // Try to establish a notification on changes of the Parent property of dp.
                    if (depObj is FrameworkElement frameworkElement && !parentNotifiers.ContainsKey(target))
                    {
                        var pcn = new ParentChangedNotifier(frameworkElement, () =>
                        {
                            var localTarget = (DependencyObject)weakTarget.Target;
                            if (localTarget == null)
                                return;

                            // Call the action...
                            parentChangedAction(localTarget);
                            // ...and remove the notifier - it will probably not be used again.
                            parentNotifiers.Remove(localTarget);
                        });

                        parentNotifiers.Add(target, pcn);
                    }
                    break;
                }

                // Assign the parent to the current DependencyObject and start the next iteration.
                depObj = depObjParent;
            }

            return ret;
        }

        /// <summary>
        /// Tries to get a value that is stored somewhere in the visual tree above this <see cref="DependencyObject"/>.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="target">The <see cref="DependencyObject"/>.</param>
        /// <param name="getFunction">The function that gets the value from a <see cref="DependencyObject"/>.</param>
        /// <returns>The value, if possible.</returns>
        public static T GetValue<T>(this DependencyObject target, Func<DependencyObject, T> getFunction)
        {
            var ret = default(T);

            if (target != null)
            {
                var depObj = target;

                while (ret == null)
                {
                    // Try to get the value using the provided GetFunction.
                    ret = getFunction(depObj);

                    // Try to get the parent using the visual tree helper. This may fail on some occations.
                    if (!(depObj is Visual) && !(depObj is Visual3D) && !(depObj is FrameworkContentElement))
                        break;

                    DependencyObject depObjParent;

                    if (depObj is FrameworkContentElement element)
                        depObjParent = element.Parent;
                    else
                    {
                        try
                        {
                            depObjParent = depObj.GetParent(true);
                        }
                        catch
                        {
                            break;
                        }
                    }
                    // If this failed, try again using the Parent property (sometimes this is not covered by the VisualTreeHelper class :-P.
                    if (depObjParent == null && depObj is FrameworkElement)
                        depObjParent = ((FrameworkElement)depObj).Parent;

                    if (ret == null && depObjParent == null)
                        break;

                    // Assign the parent to the current DependencyObject and start the next iteration.
                    depObj = depObjParent;
                }
            }

            return ret;
        }

        /// <summary>
        /// Tries to get a value from a <see cref="DependencyProperty"/> that is stored somewhere in the visual tree above this <see cref="DependencyObject"/>.
        /// If this is not available, it will register a <see cref="XAMLMarkupExtensions.Base.ParentChangedNotifier"/> on the last element.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="target">The <see cref="DependencyObject"/>.</param>
        /// <param name="property">A <see cref="DependencyProperty"/> that will be read out.</param>
        /// <param name="parentChangedAction">The notification action on the change event of the Parent property.</param>
        /// <param name="parentNotifiers">A dictionary of already registered notifiers.</param>
        /// <returns>The value, if possible.</returns>
        public static T GetValueOrRegisterParentNotifier<T>(
            this DependencyObject target,
            DependencyProperty property,
            Action<DependencyObject> parentChangedAction,
            ParentNotifiers parentNotifiers)
        {
            return target.GetValueOrRegisterParentNotifier(depObj => depObj.GetValueSync<T>(property), parentChangedAction, parentNotifiers);
        }

        /// <summary>
        /// Gets the parent in the visual or logical tree.
        /// </summary>
        /// <param name="depObj">The dependency object.</param>
        /// <param name="isVisualTree">True for visual tree, false for logical tree.</param>
        /// <returns>The parent, if available.</returns>
        public static DependencyObject GetParent(this DependencyObject depObj, bool isVisualTree)
        {
            if (depObj.CheckAccess())
                return GetParentInternal(depObj, isVisualTree);

            return (DependencyObject)depObj.Dispatcher.Invoke(new Func<DependencyObject>(() => GetParentInternal(depObj, isVisualTree)));
        }

        private static DependencyObject GetParentInternal(DependencyObject depObj, bool isVisualTree)
        {
            if (isVisualTree)
                return VisualTreeHelper.GetParent(depObj);

            return LogicalTreeHelper.GetParent(depObj);
        }
    }
}
