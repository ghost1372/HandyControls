#region Copyright information
// <copyright file="NestedMarkupExtension.cs">
//     Licensed under Microsoft Public License (Ms-PL)
//     https://github.com/XAMLMarkupExtensions/XAMLMarkupExtensions/blob/master/LICENSE
// </copyright>
// <author>Uwe Mayer</author>
#endregion

namespace HandyControl.Tools.DynamicLanguage
{
    #region Uses
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;
    using System.Xaml;
    #endregion

    /// <summary>
    /// This class walks up the tree of markup extensions to support nesting.
    /// Based on <see href="https://github.com/SeriousM/WPFLocalizationExtension"/>
    /// </summary>
    [MarkupExtensionReturnType(typeof(object))]
    public abstract class NestedMarkupExtension : MarkupExtension, INestedMarkupExtension, IDisposable
    {
        /// <summary>
        /// Holds the collection of assigned dependency objects
        /// Instead of a single reference, a list is used, if this extension is applied to multiple instances.
        /// </summary>
        private readonly TargetObjectsList targetObjects = new TargetObjectsList();

        /// <summary>
        /// Holds the markup extensions root object hash code.
        /// </summary>
        private int rootObjectHashCode;

        /// <summary>
        /// Get the target objects and properties.
        /// </summary>
        /// <returns>A list of target objects.</returns>
        private List<TargetInfo> GetTargetObjectsAndProperties()
        {
            var result = targetObjects.GetTargetInfos().ToList();
            targetObjects.ClearDeadReferences();

            return result;
        }

        /// <summary>
        /// Get the paths to all target properties through the nesting hierarchy.
        /// </summary>
        /// <returns>A list of paths to the properties.</returns>
        public List<TargetPath> GetTargetPropertyPaths()
        {
            var list = new List<TargetPath>();
            var objList = GetTargetObjectsAndProperties();

            foreach (var info in objList)
            {
                if (info.IsEndpoint)
                {
                    TargetPath path = new TargetPath(info);
                    list.Add(path);
                }
                else
                {
                    foreach (var path in ((INestedMarkupExtension)info.TargetObject).GetTargetPropertyPaths())
                    {
                        // Push the ITargetMarkupExtension
                        path.AddStep(info);
                        // Add the tuple to the list
                        list.Add(path);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// An action that is called when the first target is bound.
        /// </summary>
        protected Action OnFirstTarget;

        /// <summary>
        /// This function must be implemented by all child classes.
        /// It shall return the properly prepared output of the markup extension.
        /// </summary>
        /// <param name="info">Information about the target.</param>
        /// <param name="endPoint">Information about the endpoint.</param>
        public abstract object FormatOutput(TargetInfo endPoint, TargetInfo info);

        /// <summary>
        /// Check, if the given target is connected to this markup extension.
        /// </summary>
        /// <param name="info">Information about the target.</param>
        /// <returns>True, if a connection exits.</returns>
        public bool IsConnected(TargetInfo info)
        {
            return targetObjects.IsConnected(info);
        }

        /// <summary>
        /// Override this function, if (and only if) additional information is needed from the <see cref="IServiceProvider"/> instance that is passed to <see cref="NestedMarkupExtension.ProvideValue"/>.
        /// </summary>
        /// <param name="serviceProvider">A service provider.</param>
        protected virtual void OnServiceProviderChanged(IServiceProvider serviceProvider)
        {
            // Do nothing in the base class.
        }

        /// <summary>
        /// The ProvideValue method of the <see cref="MarkupExtension"/> base class.
        /// </summary>
        /// <param name="serviceProvider">A service provider.</param>
        /// <returns>The value of the extension, or this if something gone wrong (needed for Templates).</returns>
        public sealed override object ProvideValue(IServiceProvider serviceProvider)
        {
            // If the service provider is null, return this
            if (serviceProvider == null)
                return this;

            OnServiceProviderChanged(serviceProvider);

            // Try to cast the passed serviceProvider to a IProvideValueTarget
            // If the cast fails, return this
            if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget service))
                return this;

            // Try to cast the passed serviceProvider to a IRootObjectProvider and if the cast fails return null
            if (!(serviceProvider.GetService(typeof(IRootObjectProvider)) is IRootObjectProvider rootObject))
            {
                rootObjectHashCode = 0;
            }
            else
            {
                rootObjectHashCode = rootObject.RootObject.GetHashCode();

                // We only sign up once to the Window Closed event to clear the listeners list of root object.
                if (rootObject.RootObject != null && !EndpointReachedEvent.ContainsRootObjectHash(rootObjectHashCode))
                {
                    if (rootObject.RootObject is Window window)
                    {
                        window.Closed += delegate (object sender, EventArgs args) { EndpointReachedEvent.ClearListenersForRootObject(rootObjectHashCode); };
                    }
                    else if (rootObject.RootObject is FrameworkElement frameworkElement)
                    {
                        void frameworkElementUnloadedHandler(object sender, RoutedEventArgs args)
                        {
                            frameworkElement.Unloaded -= frameworkElementUnloadedHandler;
                            EndpointReachedEvent.ClearListenersForRootObject(rootObjectHashCode);
                        }

                        frameworkElement.Unloaded += frameworkElementUnloadedHandler;
                    }
                }
            }

            // Declare a target object and property
            TargetInfo endPoint = null;
            object targetObject = service.TargetObject;
            object targetProperty = service.TargetProperty;
            int targetPropertyIndex = -1;
            Type targetPropertyType = null;
            object overriddenResult = null;

            // If target object is a Binding, extension set at Value.
            // Return Binding which work with BindingValueProvider.
            if (targetObject is Setter setter)
            {
                targetObject = new BindingValueProvider();
                targetProperty = BindingValueProvider.ValueProperty;
                targetPropertyType = setter.Property.PropertyType;

                overriddenResult = new Binding(nameof(BindingValueProvider.Value))
                {
                    Source = targetObject,
                    Mode = BindingMode.TwoWay
                };
            }
            // If target object is a Binding, extension set at Source.
            // Reconfigure existing binding and return BindingValueProvider.
            else if (targetObject is Binding binding)
            {
                binding.Path = new PropertyPath(nameof(BindingValueProvider.Value));
                binding.Mode = BindingMode.TwoWay;

                targetObject = new BindingValueProvider();
                targetProperty = BindingValueProvider.ValueProperty;
                overriddenResult = targetObject;
            }

            // First, check if the service provider is of type SimpleProvideValueServiceProvider
            //      -> If yes, get the target property type and index.
            // Check if the service.TargetProperty is a DependencyProperty or a PropertyInfo and set the type info
            if (serviceProvider is SimpleProvideValueServiceProvider spvServiceProvider)
            {
                targetPropertyType = spvServiceProvider.TargetPropertyType;
                targetPropertyIndex = spvServiceProvider.TargetPropertyIndex;
                endPoint = spvServiceProvider.EndPoint;
            }
            else if (targetPropertyType == null)
            {
                if (targetProperty is PropertyInfo pi)
                {
                    targetPropertyType = pi.PropertyType;

                    // Kick out indexers.
                    if (pi.GetIndexParameters().Any())
                        throw new InvalidOperationException("Indexers are not supported!");
                }
                else if (targetProperty is DependencyProperty dp)
                {
                    targetPropertyType = dp.PropertyType;
                }
                else
                    return this;
            }

            // If the service.TargetObject is System.Windows.SharedDp (= not a DependencyObject and not a PropertyInfo), we return "this".
            // The SharedDp will call this instance later again.
            if (!(targetObject is DependencyObject) && !(targetProperty is PropertyInfo))
                return this;

            // If the target object is a DictionaryEntry we presumably are facing a resource scenario.
            // We will be called again later with the proper target.
            if (targetObject is DictionaryEntry)
                return null;

            // Search for the target in the target object list
            WeakReference wr = targetObjects.TryFindKey(targetObject);
            if (wr == null)
            {
                // If it's the first object, call the appropriate action
                if (targetObjects.Count == 0)
                {
                    OnFirstTarget?.Invoke();
                }

                // Add to the target object list
                wr = targetObjects.AddTargetObject(targetObject);

                // Add this extension to the ObjectDependencyManager to ensure the lifetime along with the target object
                ObjectDependencyManager.AddObjectDependency(wr, this);
            }

            // Finally, add the target prop and info to the list of this WeakReference
            targetObjects.AddTargetObjectProperty(wr, targetProperty, targetPropertyType, targetPropertyIndex);

            // Sign up to the EndpointReachedEvent only if the markup extension wants to do so.
            EndpointReachedEvent.AddListener(rootObjectHashCode, this);

            // Create the target info
            TargetInfo info = new TargetInfo(targetObject, targetProperty, targetPropertyType, targetPropertyIndex);

            // Return the result of FormatOutput
            object result = null;

            if (info.IsEndpoint)
            {
                var args = new EndpointReachedEventArgs(info);
                EndpointReachedEvent.Invoke(rootObjectHashCode, this, args);
                result = args.EndpointValue;
            }
            else
                result = FormatOutput(endPoint, info);

            if (overriddenResult != null)
                return overriddenResult;

            // Check type
            if (typeof(IList).IsAssignableFrom(targetPropertyType))
                return result;
            else if (result != null && targetPropertyType.IsInstanceOfType(result))
                return result;

            // Finally, if nothing was there, return null or default
            if (targetPropertyType.IsValueType)
                return Activator.CreateInstance(targetPropertyType);
            else
                return null;
        }

        /// <summary>
        /// Set the new value for all targets.
        /// </summary>
        protected void UpdateNewValue()
        {
            UpdateNewValue(null);
        }

        /// <summary>
        /// Trigger the update of the target(s).
        /// </summary>
        /// <param name="targetPath">A specific path to follow or null for all targets.</param>
        /// <returns>The output of the path at the endpoint.</returns>
        public object UpdateNewValue(TargetPath targetPath)
        {
            if (targetPath == null)
            {
                // No path supplied - send it to all targets.
                foreach (var path in GetTargetPropertyPaths())
                {
                    // Call yourself and supply the path to follow.
                    UpdateNewValue(path);
                }
            }
            else
            {
                // Get the info of the next step.
                TargetInfo info = targetPath.GetNextStep();

                // Get the own formatted output.
                object output = FormatOutput(targetPath.EndPoint, info);

                // Set the property of the target to the new value.
                SetPropertyValue(output, info, false);

                // Have we reached the endpoint?
                // If not, call the UpdateNewValue function of the next ITargetMarkupExtension
                if (info.IsEndpoint)
                    return output;
                else
                    return ((INestedMarkupExtension)info.TargetObject).UpdateNewValue(targetPath);
            }

            return null;
        }

        /// <summary>
        /// Sets the value of a property of type PropertyInfo or DependencyProperty.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <param name="info">The target information.</param>
        /// <param name="forceNull">Determines, whether null values should be written.</param>
        public static void SetPropertyValue(object value, TargetInfo info, bool forceNull)
        {
            if ((value == null) && !forceNull)
                return;

            if (info.TargetObject is DependencyObject depObject)
            {
                if (depObject.IsSealed)
                    return;
            }

            // Anyway, a value type cannot receive null values...
            if (info.TargetPropertyType.IsValueType && (value == null))
                value = Activator.CreateInstance(info.TargetPropertyType);

            // Set the value.
            if (info.TargetProperty is DependencyProperty dp)
                ((DependencyObject)info.TargetObject).SetValueSync(dp, value);
            else
            {
                PropertyInfo pi = (PropertyInfo)info.TargetProperty;

                if (typeof(IList).IsAssignableFrom(info.TargetPropertyType) && (value != null) && !info.TargetPropertyType.IsAssignableFrom(value.GetType()))
                {
                    // A list, a list - get it and set the value directly via its index.
                    if (info.TargetPropertyIndex >= 0)
                    {
                        IList list = (IList)pi.GetValue(info.TargetObject, null);
                        if (list.Count > info.TargetPropertyIndex)
                            list[info.TargetPropertyIndex] = value;
                    }
                    return;
                }

                pi.SetValue(info.TargetObject, value, null);
            }
        }

        /// <summary>
        /// Gets the value of a property of type PropertyInfo or DependencyProperty.
        /// </summary>
        /// <param name="info">The target information.</param>
        /// <returns>The value.</returns>
        public static object GetPropertyValue(TargetInfo info)
        {
            if (info.TargetProperty is DependencyProperty tP)
            {
                if (info.TargetObject is DependencyObject tO)
                    return tO.GetValueSync<object>(tP);
                else
                    return null;
            }
            else if (info.TargetProperty is PropertyInfo pi)
            {
                if (info.TargetPropertyIndex >= 0)
                {
                    if (typeof(IList).IsAssignableFrom(info.TargetPropertyType))
                    {
                        IList list = (IList)pi.GetValue(info.TargetObject, null);
                        if (list.Count > info.TargetPropertyIndex)
                            return list[info.TargetPropertyIndex];
                    }
                }

                return pi.GetValue(info.TargetObject, null);
            }

            return null;
        }

        /// <summary>
        /// Safely get the value of a property that might be set by a further MarkupExtension.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="value">The value supplied by the set accessor of the property.</param>
        /// <param name="property">The property information.</param>
        /// <param name="index">The index of the indexed property, if applicable.</param>
        /// <param name="endPoint">An optional endpoint information.</param>
        /// <param name="service">An optional serviceProvider information.</param>
        /// <returns>The value or default.</returns>
        protected T GetValue<T>(object value, PropertyInfo property, int index, TargetInfo endPoint = null, IServiceProvider service= null)
        {
            // Simple case: value is of same type
            if (value is T t && !(value is MarkupExtension))
                return t;

            // No property supplied
            if (property == null)
                return default;

            // Is value of type MarkupExtension?
            if (value is MarkupExtension me)
            {
                object result = me.ProvideValue(new SimpleProvideValueServiceProvider(this, property, property.PropertyType, index, endPoint, service));
                if (result != null)
                    return (T)result;
            }

            // Default return path.
            return default;
        }

        /// <summary>
        /// This method must return true, if an update shall be executed when the given endpoint is reached.
        /// This method is called each time an endpoint is reached.
        /// </summary>
        /// <param name="endpoint">Information on the specific endpoint.</param>
        /// <returns>True, if an update of the path to this endpoint shall be performed.</returns>
        protected abstract bool UpdateOnEndpoint(TargetInfo endpoint);

        /// <summary>
        /// Get the path to a specific endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint info.</param>
        /// <returns>The path to the endpoint.</returns>
        protected TargetPath GetPathToEndpoint(TargetInfo endpoint)
        {
            // If endpoint is connected - return empty path.
            if (IsConnected(endpoint))
                return new TargetPath(endpoint);

            // Else try find endpoint in nested targets.
            foreach (var nestedTargetInfo in targetObjects.GetNestedTargetInfos())
            {
                // If nested target inherit NestedMarkupExtension - we can fast get path of endpoint using current method.
                // Otherwise use slow search by getting all paths.
                var interfaceInheritor = (INestedMarkupExtension) nestedTargetInfo.TargetObject;
                var path = nestedTargetInfo.TargetObject is NestedMarkupExtension classInheritor
                    ? classInheritor.GetPathToEndpoint(endpoint)
                    : interfaceInheritor.GetTargetPropertyPaths().FirstOrDefault(pp => pp.EndPoint.TargetObject == endpoint.TargetObject);
                if (path != null)
                {
                    targetObjects.ClearDeadReferences();

                    path.AddStep(nestedTargetInfo);
                    return path;
                }
            }

            targetObjects.ClearDeadReferences();
            return null;
        }

        /// <summary>
        /// Checks the existance of the given object in the target endpoint list.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>True, if the extension nesting tree reaches the given object.</returns>
        protected bool IsEndpointObject(object obj)
        {
            // Check if object contains in current targets.
            if (targetObjects.TryFindKey(obj) != null)
                return true;

            // Else try find object in nested targets.
            foreach (var nestedTargetInfo in targetObjects.GetNestedTargetInfos())
            {
                // If nested target inherit NestedMarkupExtension - we can fast get path of endpoint using current method.
                // Otherwise use slow search by getting all paths.
                var interfaceInheritor = (INestedMarkupExtension) nestedTargetInfo.TargetObject;
                var isEndpoint = nestedTargetInfo.TargetObject is NestedMarkupExtension classInheritor
                    ? classInheritor.IsEndpointObject(obj)
                    : interfaceInheritor.GetTargetPropertyPaths().Any(tpp => tpp.EndPoint.TargetObject == obj);
                if (isEndpoint)
                {
                    targetObjects.ClearDeadReferences();
                    return true;
                }
            }

            targetObjects.ClearDeadReferences();
            return false;
        }

        /// <summary>
        /// An event handler that is called from the static <see cref="EndpointReachedEvent"/> class.
        /// </summary>
        /// <param name="sender">The markup extension that reached an enpoint.</param>
        /// <param name="args">The event args containing the endpoint information.</param>
        private void OnEndpointReached(NestedMarkupExtension sender, EndpointReachedEventArgs args)
        {
            if (args.Handled)
                return;

            if ((this != sender) && !UpdateOnEndpoint(args.Endpoint))
                return;

            var path = GetPathToEndpoint(args.Endpoint);
            if (path == null)
                return;

            args.EndpointValue = UpdateNewValue(path);

            // Removed, because of no use:
            // args.Handled = true;
        }

        /// <summary>
        /// Implements the IDisposable.Dispose function.
        /// </summary>
        public void Dispose()
        {
            EndpointReachedEvent.RemoveListener(rootObjectHashCode, this);
            targetObjects.Dispose();
        }

        #region EndpointReachedEvent
        /// <summary>
        /// A static proxy class that handles endpoint reached events for a list of weak references of <see cref="NestedMarkupExtension"/>.
        /// This circumvents the usage of a WeakEventManager while providing a static instance that is capable of firing the event.
        /// </summary>
        internal static class EndpointReachedEvent
        {
            /// <summary>
            /// A dicitonary which contains a list of listeners per unique rootObject hash.
            /// </summary>
            private static readonly Dictionary<int, List<WeakReference>> listeners;
            private static readonly object listenersLock;

            /// <summary>
            /// Fire the event.
            /// </summary>
            /// <param name="rootObjectHashCode"><paramref name="sender"/>s root object hash code.</param>
            /// <param name="sender">The markup extension that reached an end point.</param>
            /// <param name="args">The event args containing the endpoint information.</param>
            internal static void Invoke(int rootObjectHashCode, NestedMarkupExtension sender, EndpointReachedEventArgs args)
            {
                lock (listenersLock)
                {
                    // Do nothing if we don't have this root object hash.
                    if (!listeners.ContainsKey(rootObjectHashCode))
                        return;

                    foreach (var wr in listeners[rootObjectHashCode].ToList())
                    {
                        var targetReference = wr.Target;
                        if (targetReference is NestedMarkupExtension)
                            ((NestedMarkupExtension)targetReference).OnEndpointReached(sender, args);
                        else
                            listeners[rootObjectHashCode].Remove(wr);
                    }
                }
            }

            /// <summary>
            /// Adds a listener to the inner list of listeners.
            /// </summary>
            /// <param name="rootObjectHashCode"><paramref name="listener"/>s root object hash code.</param>
            /// <param name="listener">The listener to add.</param>
            internal static void AddListener(int rootObjectHashCode, NestedMarkupExtension listener)
            {
                if (listener == null)
                    return;

                lock (listenersLock)
                {
                    // Do we have a listeners list for this root object yet, if not add it.
                    if (!listeners.ContainsKey(rootObjectHashCode))
                    {
                        listeners[rootObjectHashCode] = new List<WeakReference>();
                    }

                    // Check, if this listener already was added.
                    foreach (var wr in listeners[rootObjectHashCode].ToList())
                    {
                        var targetReference = wr.Target;
                        if (targetReference == null)
                            listeners[rootObjectHashCode].Remove(wr);
                        else if (targetReference == listener)
                            return;
                        else
                        {
                            var existing = (NestedMarkupExtension)targetReference;
                            var targets = existing.GetTargetObjectsAndProperties();

                            foreach (var target in targets)
                            {
                                if (listener.IsConnected(target))
                                {
                                    listeners[rootObjectHashCode].Remove(wr);
                                    break;
                                }
                            }
                        }
                    }

                    // Add it now.
                    listeners[rootObjectHashCode].Add(new WeakReference(listener));
                }
            }

            /// <summary>
            /// Clears the listeners list for the given root object hash code <paramref name="rootObjectHashCode"/>.
            /// </summary>
            /// <param name="rootObjectHashCode"></param>
            internal static void ClearListenersForRootObject(int rootObjectHashCode)
            {
                lock (listenersLock)
                {
                    if (!listeners.ContainsKey(rootObjectHashCode))
                        return;

                    listeners[rootObjectHashCode].Clear();
                    listeners.Remove(rootObjectHashCode);
                }
            }

            /// <summary>
            /// Returns true if the given <paramref name="rootObjectHashCode"/> is already added, false otherwise.
            /// </summary>
            /// <param name="rootObjectHashCode">Root object hash code to check.</param>
            /// <returns>Returns true if the given <paramref name="rootObjectHashCode"/> is already added, false otherwise.</returns>
            internal static bool ContainsRootObjectHash(int rootObjectHashCode)
            {
                return listeners.ContainsKey(rootObjectHashCode);
            }

            /// <summary>
            /// Removes a listener from the inner list of listeners.
            /// </summary>
            /// <param name="rootObjectHashCode"><paramref name="listener"/>s root object hash code.</param>
            /// <param name="listener">The listener to remove.</param>
            internal static void RemoveListener(int rootObjectHashCode, NestedMarkupExtension listener)
            {
                if (listener == null)
                    return;

                lock (listenersLock)
                {
                    if (!listeners.ContainsKey(rootObjectHashCode))
                        return;

                    foreach (var wr in listeners[rootObjectHashCode].ToList())
                    {
                        var targetReference = wr.Target;
                        if (targetReference == null)
                            listeners[rootObjectHashCode].Remove(wr);
                        else if ((NestedMarkupExtension)targetReference == listener)
                            listeners[rootObjectHashCode].Remove(wr);
                    }

                    if (listeners[rootObjectHashCode].Count == 0)
                        listeners.Remove(rootObjectHashCode);
                }
            }

            /// <summary>
            /// An empty static constructor to prevent the class from being marked as beforefieldinit.
            /// </summary>
            static EndpointReachedEvent()
            {
                listeners = new Dictionary<int, List<WeakReference>>();
                listenersLock = new object();
            }
        }
        #endregion
    }
}
