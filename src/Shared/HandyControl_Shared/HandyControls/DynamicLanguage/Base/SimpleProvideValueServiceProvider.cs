#region Copyright information
// <copyright file="SimpleProvideValueServiceProvider.cs">
//     Licensed under Microsoft Public License (Ms-PL)
//     https://github.com/XAMLMarkupExtensions/XAMLMarkupExtensions/blob/master/LICENSE
// </copyright>
// <author>Uwe Mayer</author>
#endregion

namespace HandyControl.Tools.DynamicLanguage
{
    #region Usings
    using System;
    using System.Windows.Markup;
    #endregion

    /// <summary>
    /// This class implements the interfaces IServiceProvider and IProvideValueTarget for ProvideValue calls on markup extensions.
    /// </summary>
    public class SimpleProvideValueServiceProvider : IServiceProvider, IProvideValueTarget
    {
        #region IServiceProvider
        /// <summary>
        /// Return the requested service.
        /// </summary>
        /// <param name="service">The type of the service.</param>
        /// <returns>The service (this, if service ist IProvideValueTarget, otherwise null).</returns>
        public object GetService(Type service)
        {
            // This class only implements the IProvideValueTarget service.
            if (service == typeof(IProvideValueTarget))
                return this;

            return ServiceProvider?.GetService(service);
        }
        #endregion

        #region Properties

        #region IProvideValueTarget
        /// <summary>
        /// The target object.
        /// </summary>
        public object TargetObject { get; private set; }

        /// <summary>
        /// The target property.
        /// </summary>
        public object TargetProperty { get; private set; } 
        #endregion

        /// <summary>
        /// The target property type.
        /// </summary>
        public Type TargetPropertyType { get; private set; }

        /// <summary>
        /// The target property index.
        /// </summary>
        public int TargetPropertyIndex { get; private set; }

        /// <summary>
        /// An optional endpoint information.
        /// </summary>
        public TargetInfo EndPoint { get; private set; }

        /// <summary>
        /// An optional IServiceProvider information.
        /// </summary>
        public IServiceProvider ServiceProvider { get; private set; }
        #endregion

        #region Construtors
        /// <summary>
        /// Create a new instance of a SimpleProvideValueServiceProvider.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="targetProperty">The target property.</param>
        /// <param name="targetPropertyType">The target property type.</param>
        /// <param name="targetPropertyIndex">The target property index.</param>
        /// <param name="endPoint">An optional endpoint information.</param>
        /// <param name="serviceProvider">An optional endpoint information.</param>
        public SimpleProvideValueServiceProvider(object targetObject, object targetProperty, Type targetPropertyType, int targetPropertyIndex, TargetInfo endPoint = null, IServiceProvider serviceProvider = null)
        {
            TargetObject = targetObject;
            TargetProperty = targetProperty;
            TargetPropertyType = targetPropertyType;
            TargetPropertyIndex = targetPropertyIndex;
            EndPoint = endPoint;
            ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Create a new instance of a SimpleProvideValueServiceProvider.
        /// </summary>
        /// <param name="info">Information about the target.</param>
        /// <param name="endPoint">An optional endpoint information.</param>
        /// <param name="serviceProvider">An optional endpoint information.</param>
        public SimpleProvideValueServiceProvider(TargetInfo info, TargetInfo endPoint = null, IServiceProvider serviceProvider = null) :
            this(info.TargetObject, info.TargetProperty, info.TargetPropertyType, info.TargetPropertyIndex, endPoint, serviceProvider)
        {
        } 
        #endregion
    }
}
