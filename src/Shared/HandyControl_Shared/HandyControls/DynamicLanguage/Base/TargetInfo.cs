#region Copyright information
// <copyright file="NestedMarkupExtension.cs">
//     Licensed under Microsoft Public License (Ms-PL)
//     https://github.com/XAMLMarkupExtensions/XAMLMarkupExtensions/blob/master/LICENSE
// </copyright>
// <author>Uwe Mayer</author>
#endregion

namespace HandyControl.Tools.DynamicLanguage
{
    #region Usings
    using System;
    using System.Windows;
    #endregion

    /// <summary>
    /// This class stores information about a markup extension target.
    /// </summary>
    public class TargetInfo
    {
        /// <summary>
        /// Gets the target object.
        /// </summary>
        public object TargetObject { get; private set; }

        /// <summary>
        /// Gets the target property.
        /// </summary>
        public object TargetProperty { get; private set; }

        /// <summary>
        /// Gets the target property type.
        /// </summary>
        public Type TargetPropertyType { get; private set; }

        /// <summary>
        /// Gets the target property index.
        /// </summary>
        public int TargetPropertyIndex { get; private set; }

        /// <summary>
        /// True, if the target object is a DependencyObject.
        /// </summary>
        public bool IsDependencyObject { get { return TargetObject is DependencyObject; } }

        /// <summary>
        /// True, if the target object is an endpoint (not another nested markup extension).
        /// </summary>
        public bool IsEndpoint { get { return !(TargetObject is INestedMarkupExtension); } }

        /// <summary>
        /// Determines, whether both objects are equal.
        /// </summary>
        /// <param name="obj">An object of type TargetInfo.</param>
        /// <returns>True, if both are equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj is TargetInfo ti)
            {
                if (ti.TargetObject != this.TargetObject)
                    return false;
                if (ti.TargetProperty != this.TargetProperty)
                    return false;
                if (ti.TargetPropertyIndex != this.TargetPropertyIndex)
                    return false;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Serves as a hash function.
        /// </summary>
        /// <returns>The hash value.</returns>
        public override int GetHashCode()
        {
            // As this class is similar to a Tuple<T1, T2, T3> (the property type is redundant),
            // we take this as a template for the hash generation.
            return Tuple.Create<object, object, int>(this.TargetObject, this.TargetProperty, this.TargetPropertyIndex).GetHashCode();
        }

        /// <summary>
        /// Creates a new TargetInfo instance.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="targetProperty">The target property.</param>
        /// <param name="targetPropertyType">The target property type.</param>
        /// <param name="targetPropertyIndex">The target property index.</param>
        public TargetInfo(object targetObject, object targetProperty, Type targetPropertyType, int targetPropertyIndex)
        {
            this.TargetObject = targetObject;
            this.TargetProperty = targetProperty;
            this.TargetPropertyType = targetPropertyType;
            this.TargetPropertyIndex = targetPropertyIndex;
        }
    }
}
