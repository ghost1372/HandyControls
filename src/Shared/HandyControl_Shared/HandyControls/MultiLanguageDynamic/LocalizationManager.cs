using System;
using HandyControl.Data;

namespace HandyControl.Controls

{
    /// <summary>
    /// The main class for working with localization
    /// </summary>
    public class LocalizationManager
    {
        private LocalizationManager() { }

        private static LocalizationManager _localizationManager;

        public static LocalizationManager Instance => _localizationManager ?? (_localizationManager = new LocalizationManager());

        public event EventHandler<FunctionEventArgs<string>> CultureChanged;

        public ILocalizationProvider LocalizationProvider { get; set; }

        internal void OnCultureChanged(string culture)
        {
            CultureChanged?.Invoke(this, new FunctionEventArgs<string>(culture));
        }

        public object Localize(string key)
        {
            return localize(key);
        }

        public string LocalizeString(string key)
        {
            return localize(key).ToString();
        }

        private object localize(string key)
        {
            if (string.IsNullOrEmpty(key))
                return "[NULL]";
            var localizedValue = LocalizationProvider?.Localize(key);
            return localizedValue ?? $"[{key}]";
        }
    }
}
