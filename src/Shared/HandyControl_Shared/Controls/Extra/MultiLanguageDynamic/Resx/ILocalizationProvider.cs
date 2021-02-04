namespace HandyControl.Controls

{
    /// <summary>
    /// Interface for implementing a localized string provider
    /// </summary>
    public interface ILocalizationProvider
    {
        /// <summary>
        /// Returns a localized object by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns></returns>
        object Localize(string key);
    }
}
