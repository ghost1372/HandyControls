#region Copyright information
// <copyright file="IDictionaryEventListener.cs">
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
    #endregion

    /// <summary>
    /// Interface for listeners on dictionary events of the <see cref="LocalizeDictionary"/> class.
    /// </summary>
    public interface IDictionaryEventListener
    {
        /// <summary>
        /// This method is called when the resource somehow changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        void ResourceChanged(DependencyObject sender, DictionaryEventArgs e);
    }

    /// <summary>
    /// An enumeration of dictionary event types.
    /// </summary>
    public enum DictionaryEventType
    {
        /// <summary>
        /// The separation changed.
        /// </summary>
        SeparationChanged,
        /// <summary>
        /// The provider changed.
        /// </summary>
        ProviderChanged,
        /// <summary>
        /// A provider reports an update.
        /// </summary>
        ProviderUpdated,
        /// <summary>
        /// The culture changed.
        /// </summary>
        CultureChanged,
        /// <summary>
        /// A certain value changed.
        /// </summary>
        ValueChanged,
    }

    /// <summary>
    /// Event argument for dictionary events.
    /// </summary>
    public class DictionaryEventArgs : EventArgs
    {
        /// <summary>
        /// The type of the event.
        /// </summary>
        public DictionaryEventType Type { get; }

        /// <summary>
        /// A corresponding tag.
        /// </summary>
        public object Tag { get; }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="type">The type of the event.</param>
        /// <param name="tag">The corresponding tag.</param>
        public DictionaryEventArgs(DictionaryEventType type, object tag)
        {
            Type = type;
            Tag = tag;
        }

        /// <summary>
        /// Returns the type and tag as a string.
        /// </summary>
        /// <returns>The type and tag as a string.</returns>
        public override string ToString()
        {
            return Type + ": " + Tag;
        }
    }
}
