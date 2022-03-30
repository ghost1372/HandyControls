using System.ComponentModel;

namespace HandyControl.Tools;

/// <summary>
/// Listener for cultural change when localized by key
/// </summary>
public class KeyLocalizationListener : BaseLocalizationListener, INotifyPropertyChanged
{
    public KeyLocalizationListener(ILocalizationProvider provider,string key, object[] args)
    {
        Key = key;
        Args = args;
        Provider = provider;
    }

    private ILocalizationProvider Provider { get; }
    private string Key { get; }

    private object[] Args { get; }

    public object Value
    {
        get
        {
            var value = LocalizationManager.Localize(Provider, Key);
            if (value is string && Args != null)
                value = string.Format((string)value, Args);
            return value;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected override void OnCultureChanged()
    {
        // Notify string change binding
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
    }
}
