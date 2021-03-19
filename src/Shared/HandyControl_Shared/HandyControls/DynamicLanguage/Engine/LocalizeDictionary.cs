#region Copyright information
// <copyright file="LocalizeDictionary.cs">
//     Licensed under Microsoft Public License (Ms-PL)
//     https://github.com/XAMLMarkupExtensions/WPFLocalizationExtension/blob/master/LICENSE
// </copyright>
// <author>Bernhard Millauer</author>
// <author>Uwe Mayer</author>
#endregion

namespace HandyControl.Tools.DynamicLanguage
{
    using HandyControl.Data;
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    #endregion

    /// <summary>
    /// Represents the culture interface for localization
    /// </summary>
    public sealed class LocalizeDictionary : DependencyObject, INotifyPropertyChanged
    {
        public event EventHandler<FunctionEventArgs<CultureInfo>> CultureChanged;
        internal void RaiseCultureChanged(CultureInfo culture)
        {
            CultureChanged?.Invoke(this, new FunctionEventArgs<CultureInfo>(culture));
        }

        #region INotifyPropertyChanged Implementation
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

        #region Dependency Properties
        /// <summary>
        /// <see cref="DependencyProperty"/> DefaultProvider to set the default ILocalizationProvider.
        /// </summary>
        public static readonly DependencyProperty DefaultProviderProperty =
            DependencyProperty.RegisterAttached(
                "DefaultProvider",
                typeof(ILocalizationProvider),
                typeof(LocalizeDictionary),
                new PropertyMetadata(null, SetDefaultProviderFromDependencyProperty));

        /// <summary>
        /// <see cref="DependencyProperty"/> Provider to set the ILocalizationProvider.
        /// </summary>
        public static readonly DependencyProperty ProviderProperty =
            DependencyProperty.RegisterAttached(
                "Provider",
                typeof(ILocalizationProvider),
                typeof(LocalizeDictionary),
                new PropertyMetadata(null, SetProviderFromDependencyProperty));

        /// <summary>
        /// <see cref="DependencyProperty"/> DesignCulture to set the Culture.
        /// Only supported at DesignTime.
        /// </summary>
        [DesignOnly(true)]
        public static readonly DependencyProperty DesignCultureProperty =
            DependencyProperty.RegisterAttached(
                "DesignCulture",
                typeof(CultureInfo),
                typeof(LocalizeDictionary),
                new PropertyMetadata(SetCultureFromDependencyProperty));

        /// <summary>
        /// <see cref="DependencyProperty"/> Separation to set the separation character/string for resource name patterns.
        /// </summary>
        public static readonly DependencyProperty SeparationProperty =
            DependencyProperty.RegisterAttached(
                "Separation",
                typeof(string),
                typeof(LocalizeDictionary),
                new PropertyMetadata(DefaultSeparation, SetSeparationFromDependencyProperty));

        /// <summary>
        /// A flag indicating that the invariant culture should be included.
        /// </summary>
        public static readonly DependencyProperty IncludeInvariantCultureProperty =
            DependencyProperty.RegisterAttached(
                "IncludeInvariantCulture",
                typeof(bool),
                typeof(LocalizeDictionary),
                new PropertyMetadata(true, SetIncludeInvariantCultureFromDependencyProperty));

        /// <summary>
        /// A flag indicating that the cache is disabled.
        /// </summary>
        public static readonly DependencyProperty DisableCacheProperty =
            DependencyProperty.RegisterAttached(
                "DisableCache",
                typeof(bool),
                typeof(LocalizeDictionary),
                new PropertyMetadata(false, SetDisableCacheFromDependencyProperty));

        /// <summary>
        /// A flag indicating that missing keys should be output.
        /// </summary>
        public static readonly DependencyProperty OutputMissingKeysProperty =
            DependencyProperty.RegisterAttached(
                "OutputMissingKeys",
                typeof(bool),
                typeof(LocalizeDictionary),
                new PropertyMetadata(true, SetOutputMissingKeysFromDependencyProperty));
        #endregion

        #region Dependency Property Callbacks
        /// <summary>
        /// Callback function. Used to set the <see cref="LocalizeDictionary"/>.Culture if set in Xaml.
        /// Only supported at DesignTime.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="args">The event argument.</param>
        [DesignOnly(true)]
        private static void SetCultureFromDependencyProperty(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (!Instance.GetIsInDesignMode())
                return;

            CultureInfo culture;

            try
            {
                culture = (CultureInfo)args.NewValue;
            }
            catch
            {
                if (Instance.GetIsInDesignMode())
                    culture = DefaultCultureInfo;
                else
                    throw;
            }

            if (culture != null)
                Instance.Culture = culture;
        }

        /// <summary>
        /// Callback function. Used to set the <see cref="LocalizeDictionary"/>.Provider if set in Xaml.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="args">The event argument.</param>
        private static void SetProviderFromDependencyProperty(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DictionaryEvent.Invoke(obj, new DictionaryEventArgs(DictionaryEventType.ProviderChanged, args.NewValue));

            if (args.OldValue is ILocalizationProvider oldProvider)
            {
                oldProvider.ProviderChanged -= ProviderUpdated;
                oldProvider.ValueChanged -= ValueChanged;
                oldProvider.AvailableCultures.CollectionChanged -= Instance.AvailableCulturesCollectionChanged;
            }

            if (args.NewValue is ILocalizationProvider provider)
            {
                provider.ProviderChanged += ProviderUpdated;
                provider.ValueChanged += ValueChanged;
                provider.AvailableCultures.CollectionChanged += Instance.AvailableCulturesCollectionChanged;

                foreach (var c in provider.AvailableCultures)
                    if (!Instance.MergedAvailableCultures.Contains(c))
                        Instance.MergedAvailableCultures.Add(c);
            }
        }

        private static void ProviderUpdated(object sender, ProviderChangedEventArgs args)
        {
            DictionaryEvent.Invoke(args.Object, new DictionaryEventArgs(DictionaryEventType.ProviderUpdated, sender));
        }

        private static void ValueChanged(object sender, ValueChangedEventArgs args)
        {
            DictionaryEvent.Invoke(null, new DictionaryEventArgs(DictionaryEventType.ValueChanged, args));
        }

        /// <summary>
        /// Callback function. Used to set the <see cref="LocalizeDictionary"/>.DefaultProvider if set in Xaml.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="args">The event argument.</param>
        private static void SetDefaultProviderFromDependencyProperty(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue is ILocalizationProvider provider)
                Instance.DefaultProvider = provider;
        }

        /// <summary>
        /// Callback function. Used to set the <see cref="LocalizeDictionary"/>.Separation if set in Xaml.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="args">The event argument.</param>
        private static void SetSeparationFromDependencyProperty(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
        }

        /// <summary>
        /// Callback function. Used to set the <see cref="LocalizeDictionary"/>.IncludeInvariantCulture if set in Xaml.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="args">The event argument.</param>
        private static void SetIncludeInvariantCultureFromDependencyProperty(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue is bool b)
                Instance.IncludeInvariantCulture = b;
        }

        /// <summary>
        /// Callback function. Used to set the <see cref="LocalizeDictionary"/>.DisableCache if set in Xaml.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="args">The event argument.</param>
        private static void SetDisableCacheFromDependencyProperty(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue is bool b)
                Instance.DisableCache = b;
        }

        /// <summary>
        /// Callback function. Used to set the <see cref="LocalizeDictionary"/>.OutputMissingKeys if set in Xaml.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="args">The event argument.</param>
        private static void SetOutputMissingKeysFromDependencyProperty(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue is bool b)
                Instance.OutputMissingKeys = b;
        }
        #endregion

        #region Dependency Property Management
        #region Get
        /// <summary>
        /// Getter of <see cref="DependencyProperty"/> Provider.
        /// </summary>
        /// <param name="obj">The dependency object to get the provider from.</param>
        /// <returns>The provider.</returns>
        public static ILocalizationProvider GetProvider(DependencyObject obj)
        {
            return obj.GetValueSync<ILocalizationProvider>(ProviderProperty);
        }

#pragma warning disable IDE0060
        /// <summary>
        /// Getter of <see cref="DependencyProperty"/> DefaultProvider.
        /// </summary>
        /// <param name="obj">The dependency object to get the default provider from.</param>
        /// <returns>The default provider.</returns>
        public static ILocalizationProvider GetDefaultProvider(DependencyObject obj)
        {
            return Instance.DefaultProvider;
        }

        /// <summary>
        /// Tries to get the separation from the given target object or of one of its parents.
        /// </summary>
        /// <param name="target">The target object for context.</param>
        /// <returns>The separation of the given context or the default.</returns>
        public static string GetSeparation(DependencyObject target)
        {
            return Instance.Separation;
        }

        /// <summary>
        /// Tries to get the flag from the given target object or of one of its parents.
        /// </summary>
        /// <param name="target">The target object for context.</param>
        /// <returns>The flag.</returns>
        public static bool GetIncludeInvariantCulture(DependencyObject target)
        {
            return Instance.IncludeInvariantCulture;
        }

        /// <summary>
        /// Tries to get the flag from the given target object or of one of its parents.
        /// </summary>
        /// <param name="target">The target object for context.</param>
        /// <returns>The flag.</returns>
        public static bool GetDisableCache(DependencyObject target)
        {
            return Instance.DisableCache;
        }

        /// <summary>
        /// Tries to get the flag from the given target object or of one of its parents.
        /// </summary>
        /// <param name="target">The target object for context.</param>
        /// <returns>The flag.</returns>
        public static bool GetOutputMissingKeys(DependencyObject target)
        {
            return Instance.OutputMissingKeys;
        }
#pragma warning restore IDE0060

        /// <summary>
        /// Getter of <see cref="DependencyProperty"/> DesignCulture.
        /// Only supported at DesignTime.
        /// If its in Runtime, <see cref="LocalizeDictionary"/>.Culture will be returned.
        /// </summary>
        /// <param name="obj">The dependency object to get the design culture from.</param>
        /// <returns>The design culture at design time or the current culture at runtime.</returns>
        [DesignOnly(true)]
        public static CultureInfo GetDesignCulture(DependencyObject obj)
        {
            if (Instance.GetIsInDesignMode())
                return obj.GetValueSync<CultureInfo>(DesignCultureProperty);

            return Instance.Culture;
        }
        #endregion

        #region Set
        /// <summary>
        /// Setter of <see cref="DependencyProperty"/> Provider.
        /// </summary>
        /// <param name="obj">The dependency object to set the provider to.</param>
        /// <param name="value">The provider.</param>
        public static void SetProvider(DependencyObject obj, ILocalizationProvider value)
        {
            obj.SetValueSync(ProviderProperty, value);
        }

#pragma warning disable IDE0060
        /// <summary>
        /// Setter of <see cref="DependencyProperty"/> DefaultProvider.
        /// </summary>
        /// <param name="obj">The dependency object to set the default provider to.</param>
        /// <param name="value">The default provider.</param>
        public static void SetDefaultProvider(DependencyObject obj, ILocalizationProvider value)
        {
            Instance.DefaultProvider = value;
        }

        /// <summary>
        /// Setter of <see cref="DependencyProperty"/> Separation.
        /// </summary>
        /// <param name="obj">The dependency object to set the separation to.</param>
        /// <param name="value">The separation.</param>
        public static void SetSeparation(DependencyObject obj, string value)
        {
            Instance.Separation = value;
        }

        /// <summary>
        /// Setter of <see cref="DependencyProperty"/> IncludeInvariantCulture.
        /// </summary>
        /// <param name="obj">The dependency object to set the separation to.</param>
        /// <param name="value">The flag.</param>
        public static void SetIncludeInvariantCulture(DependencyObject obj, bool value)
        {
            Instance.IncludeInvariantCulture = value;
        }

        /// <summary>
        /// Setter of <see cref="DependencyProperty"/> DisableCache.
        /// </summary>
        /// <param name="obj">The dependency object to set the separation to.</param>
        /// <param name="value">The flag.</param>
        public static void SetDisableCache(DependencyObject obj, bool value)
        {
            Instance.DisableCache = value;
        }

        /// <summary>
        /// Setter of <see cref="DependencyProperty"/> OutputMissingKeys.
        /// </summary>
        /// <param name="obj">The dependency object to set the separation to.</param>
        /// <param name="value">The flag.</param>
        public static void SetOutputMissingKeys(DependencyObject obj, bool value)
        {
            Instance.OutputMissingKeys = value;
        }
#pragma warning restore IDE0060

        /// <summary>
        /// Setter of <see cref="DependencyProperty"/> DesignCulture.
        /// Only supported at DesignTime.
        /// </summary>
        /// <param name="obj">The dependency object to set the culture to.</param>
        /// <param name="value">The value.</param>
        [DesignOnly(true)]
        public static void SetDesignCulture(DependencyObject obj, CultureInfo value)
        {
            if (Instance.GetIsInDesignMode())
                obj.SetValueSync(DesignCultureProperty, value);
        }
        #endregion
        #endregion

        #region Variables
        /// <summary>
        /// Holds a SyncRoot to be thread safe
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// Holds the instance of singleton
        /// </summary>
        private static LocalizeDictionary _instance;

        /// <summary>
        /// Holds the current chosen <see cref="CultureInfo"/>
        /// </summary>
        private CultureInfo _culture;

        /// <summary>
        /// Holds the separation char/string.
        /// </summary>
        private string _separation = DefaultSeparation;

        /// <summary>
        /// Determines, if the <see cref="MergedAvailableCultures"/> contains the invariant culture.
        /// </summary>
        private bool _includeInvariantCulture = true;

        /// <summary>
        /// Determines, if the cache is disabled.
        /// </summary>
        private bool _disableCache = true;

        /// <summary>
        /// Determines, if missing keys should be output.
        /// </summary>
        private bool _outputMissingKeys = true;

        /// <summary>
        /// A default provider.
        /// </summary>
        private ILocalizationProvider _defaultProvider;

        /// <summary>
        /// Determines, if the CurrentThread culture is set along with the Culture property.
        /// </summary>
        private bool _setCurrentThreadCulture = true;

        /// <summary>
        /// Determines if the code is run in DesignMode or not.
        /// </summary>
        private bool? _isInDesignMode;

        #endregion

        #region Constructor
        /// <summary>
        /// Prevents a default instance of the <see cref="T:WPFLocalizeExtension.Engine.LocalizeDictionary" /> class from being created.
        /// Static Constructor
        /// </summary>
        private LocalizeDictionary()
        {
            DefaultProvider = ResxLocalizationProvider.Instance;
            SetCultureCommand = new CultureInfoDelegateCommand(SetCulture);
        }

        private void AvailableCulturesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action<NotifyCollectionChangedEventArgs>(args =>
            {
                if (args.NewItems != null)
                {
                    foreach (CultureInfo c in args.NewItems)
                    {
                        if (!MergedAvailableCultures.Contains(c))
                            MergedAvailableCultures.Add(c);
                    }
                }

                if (args.OldItems != null)
                {
                    foreach (CultureInfo c in args.OldItems)
                    {
                        if (MergedAvailableCultures.Contains(c))
                            MergedAvailableCultures.Remove(c);
                    }
                }

                if (!_includeInvariantCulture && MergedAvailableCultures.Count > 1 && MergedAvailableCultures.Contains(CultureInfo.InvariantCulture))
                    MergedAvailableCultures.Remove(CultureInfo.InvariantCulture);
            }), e);
        }

        /// <summary>
        /// Destructor code.
        /// </summary>
        ~LocalizeDictionary()
        {
            LocalizationExtension.ClearResourceBuffer();
            LocalizationFrameworkElementExtension.ClearResourceBuffer();
            LocalizationBindingExtension.ClearResourceBuffer();
        }
        #endregion

        #region Static Properties
        /// <summary>
        /// Gets the default <see cref="CultureInfo"/> to initialize the <see cref="LocalizeDictionary"/>.<see cref="CultureInfo"/>
        /// </summary>
        public static CultureInfo DefaultCultureInfo => CultureInfo.InvariantCulture;

        /// <summary>
        /// Gets the default separation char/string.
        /// </summary>
        public static string DefaultSeparation => "_";

        /// <summary>
        /// Gets the <see cref="LocalizeDictionary"/> singleton.
        /// If the underlying instance is null, a instance will be created.
        /// </summary>
        public static LocalizeDictionary Instance
        {
            get
            {
                // check if the underlying instance is null
                if (_instance == null)
                {
                    // if it is null, lock the syncroot.
                    // if another thread is accessing this too,
                    // it have to wait until the syncroot is released
                    lock (SyncRoot)
                    {
                        // check again, if the underlying instance is null
                        if (_instance == null)
                        {
                            // create a new instance
                            _instance = new LocalizeDictionary();
                        }
                    }
                }

                // return the existing/new instance
                return _instance;
            }
        }

        /// <summary>
        /// Gets the culture of the singleton instance.
        /// </summary>
        public static CultureInfo CurrentCulture => Instance.Culture;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the <see cref="CultureInfo"/> for localization.
        /// On set, <see cref="DictionaryEvent"/> is raised.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// You have to set <see cref="LocalizeDictionary"/>.Culture first or
        /// wait until System.Windows.Application.Current.MainWindow is created.
        /// Otherwise you will get an Exception.</exception>
        /// <exception cref="System.ArgumentNullException">thrown if Culture will be set to null</exception>
        public CultureInfo Culture
        {
            get
            {
                if (_culture == null)
                    _culture = DefaultCultureInfo;

                return _culture;
            }

            set
            {
                // the cultureinfo cannot contain a null reference
                if (value == null)
                    value = DefaultCultureInfo;

                // Let's see if we already got this culture
                var newCulture = value;

                if (!GetIsInDesignMode())
                {
                    foreach (var c in MergedAvailableCultures)
                        if (c == CultureInfo.InvariantCulture && !IncludeInvariantCulture)
                            continue;
                        else if (c.Name == value.Name)
                        {
                            newCulture = c;
                            break;
                        }
                        else if (c.Parent.Name == value.Name)
                        {
                            // We found a parent culture, but continue - maybe there is a specific one available too.
                            newCulture = c;
                        }
                        else if (value.Parent.Name == c.Name)
                        {
                            // We found a parent culture, but continue - maybe there is a specific one available too.
                            newCulture = value;
                        }
                }

                if (_culture != newCulture)
                {
                    if (newCulture != null && !MergedAvailableCultures.Contains(newCulture))
                        MergedAvailableCultures.Add(newCulture);

                    _culture = newCulture;

                    // Change the CurrentThread culture if needed.
                    if (_setCurrentThreadCulture && !GetIsInDesignMode())
                    {
                        Thread.CurrentThread.CurrentCulture = _culture;
                        Thread.CurrentThread.CurrentUICulture = _culture;
                    }

                    // Raise the OnLocChanged event
                    DictionaryEvent.Invoke(null, new DictionaryEventArgs(DictionaryEventType.CultureChanged, value));
                    RaiseCultureChanged(_culture);
                    RaisePropertyChanged(nameof(Culture));
                }
            }
        }

        /// <summary>
        /// Gets or sets a flag that determines, if the CurrentThread culture should be changed along with the Culture property.
        /// </summary>
        public bool SetCurrentThreadCulture
        {
            get => _setCurrentThreadCulture;
            set
            {
                if (_setCurrentThreadCulture != value)
                {
                    _setCurrentThreadCulture = value;
                    RaisePropertyChanged(nameof(SetCurrentThreadCulture));
                }
            }
        }

        /// <summary>
        /// Gets or sets the flag indicating if the invariant culture is included in the <see cref="MergedAvailableCultures"/> list.
        /// </summary>
        public bool IncludeInvariantCulture
        {
            get => _includeInvariantCulture;
            set
            {
                if (_includeInvariantCulture != value)
                {
                    _includeInvariantCulture = value;

                    var c = CultureInfo.InvariantCulture;
                    var existing = MergedAvailableCultures.Contains(c);

                    if (_includeInvariantCulture && !existing)
                        MergedAvailableCultures.Insert(0, c);
                    else if (!_includeInvariantCulture && existing && MergedAvailableCultures.Count > 1)
                        MergedAvailableCultures.Remove(c);
                }
            }
        }

        /// <summary>
        /// Gets or sets the flag that disables the cache.
        /// </summary>
        public bool DisableCache
        {
            get => _disableCache;
            set => _disableCache = value;
        }

        /// <summary>
        /// Gets or sets the flag that controls the output of missing keys.
        /// </summary>
        public bool OutputMissingKeys
        {
            get => _outputMissingKeys;
            set => _outputMissingKeys = value;
        }

        /// <summary>
        /// The separation char for automatic key retrieval.
        /// </summary>
        public string Separation
        {
            get => _separation;
            set
            {
                _separation = value;
                DictionaryEvent.Invoke(null, new DictionaryEventArgs(DictionaryEventType.SeparationChanged, value));
            }
        }

        /// <summary>
        /// Gets or sets the default <see cref="ILocalizationProvider"/>.
        /// </summary>
        public ILocalizationProvider DefaultProvider
        {
            get => _defaultProvider;
            set
            {
                if (_defaultProvider != value)
                {
                    if (_defaultProvider != null)
                    {
                        _defaultProvider.ProviderChanged -= ProviderUpdated;
                        _defaultProvider.ValueChanged -= ValueChanged;
                        _defaultProvider.AvailableCultures.CollectionChanged -= AvailableCulturesCollectionChanged;
                    }

                    _defaultProvider = value;

                    if (_defaultProvider != null)
                    {
                        _defaultProvider.ProviderChanged += ProviderUpdated;
                        _defaultProvider.ValueChanged += ValueChanged;
                        _defaultProvider.AvailableCultures.CollectionChanged += AvailableCulturesCollectionChanged;

                        foreach (var c in _defaultProvider.AvailableCultures)
                        {
                            if (!MergedAvailableCultures.Contains(c))
                                MergedAvailableCultures.Add(c);
                        }
                    }

                    RaisePropertyChanged(nameof(DefaultProvider));
                }
            }
        }

        private ObservableCollection<CultureInfo> _mergedAvailableCultures;

        /// <summary>
        /// Gets the merged list of all available cultures.
        /// </summary>
        public ObservableCollection<CultureInfo> MergedAvailableCultures
        {
            get
            {
                if (_mergedAvailableCultures == null)
                {
                    _mergedAvailableCultures = new ObservableCollection<CultureInfo> { CultureInfo.InvariantCulture };
                    _mergedAvailableCultures.CollectionChanged += (s, e) => { Culture = Culture; };
                }

                return _mergedAvailableCultures;
            }
        }

        /// <summary>
        /// A command for culture changes.
        /// </summary>
        public ICommand SetCultureCommand { get; }

        /// <summary>
        /// Gets the specific <see cref="CultureInfo"/> of the current culture.
        /// This can be used for format manners.
        /// If the Culture is an invariant <see cref="CultureInfo"/>,
        /// SpecificCulture will also return an invariant <see cref="CultureInfo"/>.
        /// </summary>
        public CultureInfo SpecificCulture => CultureInfo.CreateSpecificCulture(Culture.ToString());

        #endregion

        #region Localization Core
        /// <summary>
        /// Get the localized object using the built-in ResxLocalizationProvider.
        /// </summary>
        /// <param name="source">The source of the dictionary.</param>
        /// <param name="dictionary">The dictionary with key/value pairs.</param>
        /// <param name="key">The key to the value.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The value corresponding to the source/dictionary/key path for the given culture (otherwise NULL).</returns>
        public object GetLocalizedObject(string source, string dictionary, string key, CultureInfo culture)
        {
            return GetLocalizedObject(source + ":" + dictionary + ":" + key, null, culture, DefaultProvider);
        }

        /// <summary>
        /// Get the localized object using the given target for context information.
        /// </summary>
        /// <param name="key">The key to the value.</param>
        /// <param name="target">The target <see cref="DependencyObject"/>.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The value corresponding to the source/dictionary/key path for the given culture (otherwise NULL).</returns>
        public object GetLocalizedObject(string key, DependencyObject target, CultureInfo culture)
        {
            if (DefaultProvider is IInheritingLocalizationProvider)
                return GetLocalizedObject(key, target, culture, DefaultProvider);

            var provider = target?.GetValue(GetProvider);

            if (provider == null)
                provider = DefaultProvider;

            return GetLocalizedObject(key, target, culture, provider);
        }

        /// <summary>
        /// Get the localized object using the given target and provider.
        /// </summary>
        /// <param name="key">The key to the value.</param>
        /// <param name="target">The target <see cref="DependencyObject"/>.</param>
        /// <param name="culture">The culture to use.</param>
        /// <param name="provider">The provider to use.</param>
        /// <returns>The value corresponding to the source/dictionary/key path for the given culture (otherwise NULL).</returns>
        public object GetLocalizedObject(string key, DependencyObject target, CultureInfo culture, ILocalizationProvider provider)
        {
            if (provider == null)
                throw new InvalidOperationException("No provider found and no default provider given.");

            return provider.GetLocalizedObject(key, target, culture);
        }

        /// <summary>
        /// Uses the key and target to build a fully qualified resource key (Assembly, Dictionary, Key)
        /// </summary>
        /// <param name="key">Key used as a base to find the full key</param>
        /// <param name="target">Target used to help determine key information</param>
        /// <returns>Returns an object with all possible pieces of the given key (Assembly, Dictionary, Key)</returns>
        public FullyQualifiedResourceKeyBase GetFullyQualifiedResourceKey(string key, DependencyObject target)
        {
            if (DefaultProvider is IInheritingLocalizationProvider)
                return GetFullyQualifiedResourceKey(key, target, DefaultProvider);

            var provider = target?.GetValue(GetProvider);

            if (provider == null)
                provider = DefaultProvider;

            return GetFullyQualifiedResourceKey(key, target, provider);
        }

        /// <summary>
        /// Uses the key and target to build a fully qualified resource key (Assembly, Dictionary, Key)
        /// </summary>
        /// <param name="key">Key used as a base to find the full key</param>
        /// <param name="target">Target used to help determine key information</param>
        /// <param name="provider">Provider to use</param>
        /// <returns>Returns an object with all possible pieces of the given key (Assembly, Dictionary, Key)</returns>
        public FullyQualifiedResourceKeyBase GetFullyQualifiedResourceKey(string key, DependencyObject target, ILocalizationProvider provider)
        {
            if (provider == null)
                throw new InvalidOperationException("No provider found and no default provider given.");

            return provider.GetFullyQualifiedResourceKey(key, target);
        }

        /// <summary>
        /// Looks up the ResourceManagers for the searched <paramref name="resourceKey"/>
        /// in the <paramref name="resourceDictionary"/> in the <paramref name="resourceAssembly"/>
        /// with an Invariant Culture.
        /// </summary>
        /// <param name="resourceAssembly">The resource assembly</param>
        /// <param name="resourceDictionary">The dictionary to look up</param>
        /// <param name="resourceKey">The key of the searched entry</param>
        /// <returns>
        /// TRUE if the searched one is found, otherwise FALSE
        /// </returns>
        public bool ResourceKeyExists(string resourceAssembly, string resourceDictionary, string resourceKey)
        {
            return ResourceKeyExists(resourceAssembly, resourceDictionary, resourceKey, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Looks up the ResourceManagers for the searched <paramref name="resourceKey"/>
        /// in the <paramref name="resourceDictionary"/> in the <paramref name="resourceAssembly"/>
        /// with the passed culture. If the searched one does not exists with the passed culture, is will searched
        /// until the invariant culture is used.
        /// </summary>
        /// <param name="resourceAssembly">The resource assembly</param>
        /// <param name="resourceDictionary">The dictionary to look up</param>
        /// <param name="resourceKey">The key of the searched entry</param>
        /// <param name="cultureToUse">The culture to use.</param>
        /// <returns>
        /// TRUE if the searched one is found, otherwise FALSE
        /// </returns>
        public bool ResourceKeyExists(string resourceAssembly, string resourceDictionary, string resourceKey, CultureInfo cultureToUse)
        {
            var provider = ResxLocalizationProvider.Instance;

            return ResourceKeyExists(resourceAssembly + ":" + resourceDictionary + ":" + resourceKey, cultureToUse, provider);
        }

        /// <summary>
        /// Looks up the ResourceManagers for the searched <paramref name="key"/>
        /// with the passed culture. If the searched one does not exists with the passed culture, is will searched
        /// until the invariant culture is used.
        /// </summary>
        /// <param name="key">The key of the searched entry</param>
        /// <param name="cultureToUse">The culture to use.</param>
        /// <param name="provider">The localization provider.</param>
        /// <returns>
        /// TRUE if the searched one is found, otherwise FALSE
        /// </returns>
        public bool ResourceKeyExists(string key, CultureInfo cultureToUse, ILocalizationProvider provider)
        {
            return provider.GetLocalizedObject(key, null, cultureToUse) != null;
        }
        #endregion

        #region Helper Functions
        /// <summary>
        /// Gets the status of the design mode
        /// </summary>
        /// <returns>TRUE if in design mode, else FALSE</returns>
        public bool GetIsInDesignMode()
        {
            lock (SyncRoot)
            {
                if (_isInDesignMode.HasValue)
                    return _isInDesignMode.Value;

                if (Dispatcher?.Thread == null || !Dispatcher.Thread.IsAlive)
                {
                    _isInDesignMode = false;
                    return _isInDesignMode.Value;
                }

                if (!Dispatcher.CheckAccess())
                {
                    try
                    {
                        _isInDesignMode = (bool)Dispatcher.Invoke(DispatcherPriority.Normal, TimeSpan.FromMilliseconds(100), new Func<bool>(GetIsInDesignMode));
                    }
                    catch (Exception)
                    {
                        _isInDesignMode = default(bool);
                    }

                    return _isInDesignMode.Value;
                }
                _isInDesignMode = DesignerProperties.GetIsInDesignMode(this);
                return _isInDesignMode.Value;
            }
        }
        #endregion

        #region MissingKeyEvent (standard event)
        /// <summary>
        /// An event for missing keys.
        /// </summary>
        public event EventHandler<MissingKeyEventArgs> MissingKeyEvent;

        /// <summary>
        /// Triggers a MissingKeyEvent.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="key">The missing key.</param>
        /// <returns>True, if a reload should be performed.</returns>
        internal MissingKeyEventArgs OnNewMissingKeyEvent(object sender, string key)
        {
            var args = new MissingKeyEventArgs(key);
            MissingKeyEvent?.Invoke(sender, args);
            return args;
        }
        #endregion

        #region DictionaryEvent (using weak references)
        internal static class DictionaryEvent
        {
            /// <summary>
            /// The list of listeners
            /// </summary>
            private static readonly List<WeakReference> Listeners = new List<WeakReference>();
            private static readonly object ListenersLock = new object();

            /// <summary>
            /// Fire the event.
            /// </summary>
            /// <param name="sender">The sender of the event.</param>
            /// <param name="args">The event arguments.</param>
            internal static void Invoke(DependencyObject sender, DictionaryEventArgs args)
            {
                var list = new List<IDictionaryEventListener>();

                lock (ListenersLock)
                {
                    foreach (var wr in Listeners.ToList())
                    {
                        var targetReference = wr.Target;
                        if (targetReference != null)
                            list.Add((IDictionaryEventListener)targetReference);
                        else
                            Listeners.Remove(wr);
                    }
                }

                foreach (var item in list)
                    item.ResourceChanged(sender, args);
            }

            /// <summary>
            /// Adds a listener to the inner list of listeners.
            /// </summary>
            /// <param name="listener">The listener to add.</param>
            internal static void AddListener(IDictionaryEventListener listener)
            {
                if (listener == null)
                    return;

                // Check, if this listener already was added.
                bool listenerExists = false;

                lock (ListenersLock)
                {
                    foreach (var wr in Listeners.ToList())
                    {
                        var targetReference = wr.Target;
                        if (targetReference == null)
                            Listeners.Remove(wr);
                        else if (targetReference == listener)
                            listenerExists = true;
                    }

                    // Add it now.
                    if (!listenerExists)
                        Listeners.Add(new WeakReference(listener));
                }
            }

            /// <summary>
            /// Removes a listener from the inner list of listeners.
            /// </summary>
            /// <param name="listener">The listener to remove.</param>
            internal static void RemoveListener(IDictionaryEventListener listener)
            {
                if (listener == null)
                    return;

                lock (ListenersLock)
                {
                    foreach (var wr in Listeners.ToList())
                    {
                        var targetReference = wr.Target;
                        if (targetReference == null)
                            Listeners.Remove(wr);
                        else if ((IDictionaryEventListener)targetReference == listener)
                            Listeners.Remove(wr);
                    }
                }
            }

            /// <summary>
            /// Enumerates all listeners of type T.
            /// </summary>
            /// <typeparam name="T">The listener type.</typeparam>
            /// <returns>An enumeration of listeners.</returns>
            internal static IEnumerable<T> EnumerateListeners<T>()
            {
                lock (ListenersLock)
                {
                    foreach (var wr in Listeners.ToList())
                    {
                        var targetReference = wr.Target;

                        if (targetReference == null)
                            Listeners.Remove(wr);
                        else if (targetReference is T)
                            yield return (T)targetReference;
                    }
                }
            }
        }
        #endregion

        #region CultureInfoDelegateCommand
        private void SetCulture(CultureInfo c)
        {
            Culture = c;
        }

        /// <summary>
        /// A class for culture commands.
        /// </summary>
        internal class CultureInfoDelegateCommand : ICommand
        {
            #region Functions for execution and evaluation
            /// <summary>
            /// Predicate that determines if an object can execute
            /// </summary>
            private readonly Predicate<CultureInfo> _canExecute;

            /// <summary>
            /// The action to execute when the command is invoked
            /// </summary>
            private readonly Action<CultureInfo> _execute;
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CultureInfoDelegateCommand"/> class.
            /// Creates a new command that can always execute.
            /// </summary>
            /// <param name="execute">
            /// The execution logic.
            /// </param>
            public CultureInfoDelegateCommand(Action<CultureInfo> execute)
                : this(execute, null)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="CultureInfoDelegateCommand"/> class.
            /// Creates a new command.
            /// </summary>
            /// <param name="execute">
            /// The execution logic.
            /// </param>
            /// <param name="canExecute">
            /// The execution status logic.
            /// </param>
            public CultureInfoDelegateCommand(Action<CultureInfo> execute, Predicate<CultureInfo> canExecute)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }
            #endregion

            #region ICommand interface
            /// <summary>
            /// Occurs when changes occur that affect whether or not the command should execute.
            /// </summary>
            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            /// <summary>
            /// Determines whether the command can execute in its current state.
            /// </summary>
            /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
            /// <returns>true if this command can be executed; otherwise, false.</returns>
            public bool CanExecute(object parameter)
            {
                return _canExecute == null || _canExecute((CultureInfo)parameter);
            }

            /// <summary>
            /// Is called when the command is invoked.
            /// </summary>
            /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
            public void Execute(object parameter)
            {
                var c = new CultureInfo((string)parameter);
                _execute(c);
            }
            #endregion
        }
        #endregion
    }
}
