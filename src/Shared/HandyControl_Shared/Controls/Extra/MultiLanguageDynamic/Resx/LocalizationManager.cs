using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace HandyControl.Controls

{
    /// <summary>
    /// The main class for working with localization
    /// </summary>
    public class LocalizationManager
    {
        private LocalizationManager()
        {

        }

        private static LocalizationManager _localizationManager;

        public static LocalizationManager Instance => _localizationManager ?? (_localizationManager = new LocalizationManager());

        public event EventHandler CultureChanged;

        public CultureInfo CurrentCulture
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
            set
            {
                if (Equals(value, Thread.CurrentThread.CurrentUICulture))
                    return;
                Thread.CurrentThread.CurrentUICulture = value;
#if netle40
                Thread.CurrentThread.CurrentCulture = value;
#else
                CultureInfo.DefaultThreadCurrentUICulture = value;
#endif
                OnCultureChanged();
            }
        }

        public IEnumerable<CultureInfo> Cultures => LocalizationProvider?.Cultures ?? Enumerable.Empty<CultureInfo>();

        public ILocalizationProvider LocalizationProvider { get; set; }

        private void OnCultureChanged()
        {
            CultureChanged?.Invoke(this, EventArgs.Empty);
        }

        public object Localize(string key)
        {
            if (string.IsNullOrEmpty(key))
                return "[NULL]";
            var localizedValue = LocalizationProvider?.Localize(key);
            return localizedValue ?? $"[{key}]";
        }

        public string LocalizeString(string key)
        {
            return Localize(key).ToString();
        }
    }
}
