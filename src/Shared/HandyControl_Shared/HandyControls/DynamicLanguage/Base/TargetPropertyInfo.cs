using System;

namespace HandyControl.Tools.DynamicLanguage
{
    /// <summary>
    /// Defines information about target object property.
    /// </summary>
    internal class TargetPropertyInfo
    {
        /// <summary>
        /// Gets the target property.
        /// </summary>
        public object TargetProperty { get; }

        /// <summary>
        /// Gets the target property type.
        /// </summary>
        public Type TargetPropertyType { get; }

        /// <summary>
        /// Gets the target property index.
        /// </summary>
        public int TargetPropertyIndex { get; }

        /// <summary>
        /// Create new <see cref="TargetPropertyInfo" /> instance.
        /// </summary>
        /// <param name="targetProperty">The target property.</param>
        /// <param name="targetPropertyType">The type of property.</param>
        /// <param name="targetPropertyIndex">The target property index</param>
        public TargetPropertyInfo(object targetProperty, Type targetPropertyType, int targetPropertyIndex)
        {
            TargetProperty = targetProperty;
            TargetPropertyType = targetPropertyType;
            TargetPropertyIndex = targetPropertyIndex;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TargetPropertyInfo) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (TargetProperty != null ? TargetProperty.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ TargetPropertyIndex;
                return hashCode;
            }
        }

        /// <summary>
        /// Compare to another object.
        /// </summary>
        /// <param name="other">Other object for comparing.</param>
        /// <returns>
        /// <see langwrod="true" /> if objects are equal, <see langword="false" /> otherwise.
        /// </returns>
        protected bool Equals(TargetPropertyInfo other)
        {
            return Equals(TargetProperty, other.TargetProperty)
                   && TargetPropertyIndex == other.TargetPropertyIndex;
        }
    }
}
