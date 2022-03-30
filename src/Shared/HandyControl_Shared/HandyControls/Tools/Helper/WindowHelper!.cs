using Microsoft.Win32;

namespace HandyControl.Tools;

public static partial class WindowHelper
{
    private const string SkinTypeRegistryKeyName = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

    private const string SkinTypeRegistryValueName = "AppsUseLightTheme";

    public static bool DetermineIfInLightThemeMode()
    {
        var value = Registry.GetValue(SkinTypeRegistryKeyName, SkinTypeRegistryValueName, "0");

        return value != null && value.ToString() != "0";
    }
}
