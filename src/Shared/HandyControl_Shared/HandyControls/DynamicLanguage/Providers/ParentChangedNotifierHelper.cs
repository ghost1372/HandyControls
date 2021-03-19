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

    internal static class ParentChangedNotifierHelper
    {
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

        public static T GetValueOrRegisterParentNotifier<T>(
            this DependencyObject target,
            DependencyProperty property,
            Action<DependencyObject> parentChangedAction,
            ParentNotifiers parentNotifiers)
        {
            return target.GetValueOrRegisterParentNotifier(depObj => depObj.GetValueSync<T>(property), parentChangedAction, parentNotifiers);
        }

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
