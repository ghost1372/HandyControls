#region Copyright information
// <copyright file="SafeTargetInfo.cs">
//     Licensed under Microsoft Public License (Ms-PL)
//     https://github.com/XAMLMarkupExtensions/WPFLocalizationExtension/blob/master/LICENSE
// </copyright>
// <author>Uwe Mayer</author>
#endregion

namespace HandyControl.Tools.DynamicLanguage
{
    #region Usings
    using System;
    #endregion

    /// <summary>
    /// An extension to the <see cref="T:XAMLMarkupExtensions.Base.TargetInfo" /> class with WeakReference instead of direct object linking.
    /// </summary>
    public class SafeTargetInfo : TargetInfo
    {
        /// <summary>
        /// Gets the target object reference.
        /// </summary>
        public WeakReference TargetObjectReference { get; }

        /// <summary>
        /// Creates a new TargetInfo instance.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="targetProperty">The target property.</param>
        /// <param name="targetPropertyType">The target property type.</param>
        /// <param name="targetPropertyIndex">The target property index.</param>
        public SafeTargetInfo(object targetObject, object targetProperty, Type targetPropertyType, int targetPropertyIndex)
            : base(null, targetProperty, targetPropertyType, targetPropertyIndex)
        {
            TargetObjectReference = new WeakReference(targetObject);
        }

        /// <summary>
        /// Creates a new <see cref="SafeTargetInfo"/> based on a <see cref="XAMLMarkupExtensions.Base.TargetInfo"/> template.
        /// </summary>
        /// <param name="targetInfo">The target information.</param>
        /// <returns>A new instance with safe references.</returns>
        public static SafeTargetInfo FromTargetInfo(TargetInfo targetInfo)
        {
            return new SafeTargetInfo(targetInfo.TargetObject, targetInfo.TargetProperty, targetInfo.TargetPropertyType, targetInfo.TargetPropertyIndex);
        }
    }
}
