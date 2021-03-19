#region Copyright information
// <copyright file="BLoc.cs">
//     Licensed under Microsoft Public License (Ms-PL)
//     https://github.com/XAMLMarkupExtensions/WPFLocalizationExtension/blob/master/LICENSE
// </copyright>
// <author>Bernhard Millauer</author>
// <author>Uwe Mayer</author>
#endregion

namespace HandyControl.Tools.DynamicLanguage
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    
    #endregion

    /// <summary>
    /// A localization extension based on <see cref="Binding"/>.
    /// </summary>
    public class LocalizationBindingExtension : Binding, INotifyPropertyChanged, IDictionaryEventListener, IDisposable
    {
        #region PropertyChanged Logic
        /// <summary>
        /// Informiert über sich ändernde Eigenschaften.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notify that a property has changed
        /// </summary>
        /// <param name="property">
        /// The property that changed
        /// </param>
        internal void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        #endregion

        #region Variables & Properties
        private static readonly object ResourceBufferLock = new object();
        private static Dictionary<string, object> _resourceBuffer = new Dictionary<string, object>();

        private object _value;
        /// <summary>
        /// The value, the internal binding is pointing at.
        /// </summary>
        public object Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    RaisePropertyChanged(nameof(Value));
                }
            }
        }

        private string _key;
        /// <summary>
        /// Gets or sets the Key to a .resx object
        /// </summary>
        public string Key
        {
            get => _key;
            set
            {
                if (_key != value)
                {
                    _key = value;
                    UpdateNewValue();
                    RaisePropertyChanged(nameof(Key));
                }
            }
        }

        /// <summary>
        /// Gets or sets the culture to force a fixed localized object
        /// </summary>
        public string ForceCulture { get; set; }
        #endregion

        #region Resource buffer handling.
        /// <summary>
        /// Clears the common resource buffer.
        /// </summary>
        public static void ClearResourceBuffer()
        {
            lock (ResourceBufferLock)
            {
                _resourceBuffer?.Clear();
                _resourceBuffer = null;
            }
        }

        /// <summary>
        /// Adds an item to the resource buffer (threadsafe).
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        internal static void SafeAddItemToResourceBuffer(string key, object item)
        {
            lock (ResourceBufferLock)
            {
                if (!LocalizeDictionary.Instance.DisableCache && !_resourceBuffer.ContainsKey(key))
                    _resourceBuffer.Add(key, item);
            }
        }

        /// <summary>
        /// Removes an item from the resource buffer (threadsafe).
        /// </summary>
        /// <param name="key">The key.</param>
        internal static void SafeRemoveItemFromResourceBuffer(string key)
        {
            lock (ResourceBufferLock)
            {
                if (_resourceBuffer.ContainsKey(key))
                    _resourceBuffer.Remove(key);
            }
        }
        #endregion

        #region Block some Binding Parameters
        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new PropertyPath Path
        {
            get { return null; }
            set
            {
                throw new Exception(nameof(Path) + " not allowed for BLoc");
            }
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new object Source
        {
            get => null;
            set => throw new Exception(nameof(Source) + " not allowed for BLoc");
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new RelativeSource RelativeSource
        {
            get => null;
            set => throw new Exception(nameof(RelativeSource) + " not allowed for BLoc");
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new string ElementName
        {
            get => null;
            set => throw new Exception(nameof(ElementName) + " not allowed for BLoc");
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new BindingMode Mode
        {
            get => BindingMode.Default;
            set => throw new Exception(nameof(Mode) + " not allowed for BLoc");
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new string XPath
        {
            get => null;
            set => throw new Exception(nameof(XPath) + " not allowed for BLoc");
        }
        #endregion

        #region Constructors & Dispose
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationBindingExtension"/> class.
        /// </summary>
        public LocalizationBindingExtension()
        {
            LocalizeDictionary.DictionaryEvent.AddListener(this);
            base.Path = new PropertyPath("Value");
            base.Source = this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationBindingExtension"/> class.
        /// </summary>
        /// <param name="key">The resource identifier.</param>
        public LocalizationBindingExtension(string key)
            : this()
        {
            Key = key;
        }

        /// <summary>
        /// Removes the listener from the dictionary.
        /// </summary>
        public void Dispose()
        {
            LocalizeDictionary.DictionaryEvent.RemoveListener(this);
        }

        /// <summary>
        /// The finalizer.
        /// </summary>
        ~LocalizationBindingExtension()
        {
            Dispose();
        }
        #endregion

        #region Forced culture handling
        /// <summary>
        /// If Culture property defines a valid <see cref="CultureInfo"/>, a <see cref="CultureInfo"/> instance will get
        /// created and returned, otherwise <see cref="LocalizeDictionary"/>.Culture will get returned.
        /// </summary>
        /// <returns>The <see cref="CultureInfo"/></returns>
        /// <exception cref="System.ArgumentException">
        /// thrown if the parameter Culture don't defines a valid <see cref="CultureInfo"/>
        /// </exception>
        protected CultureInfo GetForcedCultureOrDefault()
        {
            // define a culture info
            CultureInfo cultureInfo;

            // check if the forced culture is not null or empty
            if (!string.IsNullOrEmpty(ForceCulture))
            {
                // try to create a valid cultureinfo, if defined
                try
                {
                    // try to create a specific culture from the forced one
                    // cultureInfo = CultureInfo.CreateSpecificCulture(this.ForceCulture);
                    cultureInfo = new CultureInfo(ForceCulture);
                }
                catch (ArgumentException ex)
                {
                    // on error, check if designmode is on
                    if (LocalizeDictionary.Instance.GetIsInDesignMode())
                    {
                        // cultureInfo will be set to the current specific culture
                        cultureInfo = LocalizeDictionary.Instance.SpecificCulture;
                    }
                    else
                    {
                        // tell the customer, that the forced culture cannot be converted propperly
                        throw new ArgumentException("Cannot create a CultureInfo with '" + ForceCulture + "'", ex);
                    }
                }
            }
            else
            {
                // take the current specific culture
                cultureInfo = LocalizeDictionary.Instance.SpecificCulture;
            }

            // return the evaluated culture info
            return cultureInfo;
        }
        #endregion

        /// <summary>
        /// This method is called when the resource somehow changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        public void ResourceChanged(DependencyObject sender, DictionaryEventArgs e)
        {
            UpdateNewValue();
        }

        private void UpdateNewValue()
        {
            Value = FormatOutput();
        }

        #region Future TargetMarkupExtension implementation
        /// <summary>
        /// This function returns the properly prepared output of the markup extension.
        /// </summary>
        public object FormatOutput()
        {
            object result = null;

            // Try to get the localized input from the resource.
            string resourceKey = LocalizeDictionary.Instance.GetFullyQualifiedResourceKey(Key, null);

            var ci = GetForcedCultureOrDefault();

            var key = ci.Name + ":";

            // Check, if the key is already in our resource buffer.
            lock (ResourceBufferLock)
            {
                if (_resourceBuffer.ContainsKey(key + resourceKey))
                    result = _resourceBuffer[key + resourceKey];
            }

            if (result == null)
            {
                result = LocalizeDictionary.Instance.GetLocalizedObject(resourceKey, null, ci);

                if (result == null)
                {
                    var missingKeyEventResult = LocalizeDictionary.Instance.OnNewMissingKeyEvent(this, resourceKey);

                    if (missingKeyEventResult.Reload)
                        UpdateNewValue();

                    if (LocalizeDictionary.Instance.OutputMissingKeys
                        && !string.IsNullOrEmpty(_key))
                    {
                        if (missingKeyEventResult.MissingKeyResult != null)
                            result = missingKeyEventResult.MissingKeyResult;
                        else
                            result = "Key: " + _key;
                    }
                }
                else
                {
                    key += resourceKey;
                    SafeAddItemToResourceBuffer(key, result);
                }
            }

            return result;
        }
        #endregion
    }
}
