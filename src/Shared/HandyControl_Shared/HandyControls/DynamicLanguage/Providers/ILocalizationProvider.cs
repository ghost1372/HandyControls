#region Copyright information
// <copyright file="ILocalizationProvider.cs">
//     Licensed under Microsoft Public License (Ms-PL)
//     https://github.com/XAMLMarkupExtensions/WPFLocalizationExtension/blob/master/LICENSE
// </copyright>
// <author>Uwe Mayer</author>
#endregion

namespace HandyControl.Tools.DynamicLanguage
{
    #region Usings
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Windows;
    #endregion

    /// <summary>
    /// An interface describing classes that provide localized values based on a source/dictionary/key combination.
    /// </summary>
    public interface ILocalizationProvider
    {
        /// <summary>
        /// Uses the key and target to build a fully qualified resource key (Assembly, Dictionary, Key)
        /// </summary>
        /// <param name="key">Key used as a base to find the full key</param>
        /// <param name="target">Target used to help determine key information</param>
        /// <returns>Returns an object with all possible pieces of the given key (Assembly, Dictionary, Key)</returns>
        FullyQualifiedResourceKeyBase GetFullyQualifiedResourceKey(string key, DependencyObject target);

        /// <summary>
        /// Get the localized object.
        /// </summary>
        /// <param name="key">The key to the value.</param>
        /// <param name="target">The target <see cref="DependencyObject"/>.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The value corresponding to the source/dictionary/key path for the given culture (otherwise NULL).</returns>
        object GetLocalizedObject(string key, DependencyObject target, CultureInfo culture);

        /// <summary>
        /// An observable list of available cultures.
        /// </summary>
        ObservableCollection<CultureInfo> AvailableCultures { get; }

        /// <summary>
        /// An event that is fired when the provider changed.
        /// </summary>
        event ProviderChangedEventHandler ProviderChanged;

        /// <summary>
        /// An event that is fired when an error occurred.
        /// </summary>
        event ProviderErrorEventHandler ProviderError;

        /// <summary>
        /// An event that is fired when a value changed.
        /// </summary>
        event ValueChangedEventHandler ValueChanged;
    }
}
