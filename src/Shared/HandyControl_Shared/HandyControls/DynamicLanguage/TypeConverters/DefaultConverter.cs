#region Copyright information
// <copyright file="StandardLocConverter.cs">
//     Licensed under Microsoft Public License (Ms-PL)
//     https://github.com/XAMLMarkupExtensions/WPFLocalizationExtension/blob/master/LICENSE
// </copyright>
// <author>Uwe Mayer</author>
#endregion

namespace HandyControl.Tools.DynamicLanguage
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows.Data;
    #endregion

    /// <summary>
    /// Implements a standard converter that calls itself all known type converters.
    /// </summary>
    public class DefaultConverter : IValueConverter
    {
        private static readonly Dictionary<Type, TypeConverter> TypeConverters = new Dictionary<Type, TypeConverter>();

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="Type"/> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            object result;
            var resourceType = value.GetType();

            // Simplest cases: The target type is object or same as the input.
            if (targetType == typeof(object) || resourceType == targetType)
                return value;

            // Is the type already known?
            if (!TypeConverters.ContainsKey(targetType))
            {
                var c = TypeDescriptor.GetConverter(targetType);

                // Get the type converter and store it in the dictionary (even if it is NULL).
                TypeConverters.Add(targetType, c);
            }

            // Get the converter.
            var conv = TypeConverters[targetType];

            // No converter or not convertable?
            if (conv == null || !conv.CanConvertFrom(resourceType))
                return null;

            // Finally, try to convert the value.
            try
            {
                result = conv.ConvertFrom(value);
            }
            catch
            {
                result = null;
            }

            return result;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="Type"/> of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the source object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
