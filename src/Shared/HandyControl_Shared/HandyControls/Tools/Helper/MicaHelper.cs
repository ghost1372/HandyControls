using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using HandyControl.Themes;
using HandyControl.Tools.Interop;

namespace HandyControl.Tools;

/// <summary>
/// Contains static handlers for applying background Mica effects from Windows 11.
/// </summary>
public static class MicaHelper
{
    private static int _pvTrueAttribute = 0x01;

    private static int _pvFalseAttribute = 0x00;

    private static readonly List<IntPtr> Containers = new List<IntPtr>() { };

    private static IntPtr _windowHandle;

    private static Window _window;

    /// <summary>
    /// Static singleton identifier determining whether the Mica effect has been applied.
    /// </summary>
    public static bool IsMicaEffectApplied { get; set; } = false;

    /// <summary>
    /// Applies a Mica effect when the <see cref="Window"/> is loaded.
    /// </summary>
    /// <param name="window">Active instance of <see cref="Window"/>.</param>
    public static void ApplyMicaEffect(object window)
    {
        var decWindow = window as Window;

        if (decWindow == null)
        {
            throw new Exception("Only Window controls can have the Mica effect applied.");
        }

        decWindow.Loaded += OnWindowLoaded;
    }

    /// <summary>
    /// Tries to remove the Mica effect from all defined pointers.
    /// </summary>
    public static void RemoveMicaEffect()
    {
        if (Containers == null || Containers.Count < 1)
        {
            return;
        }

        Containers.ForEach(RemoveMicaEffect);
        Containers.Clear();
    }

    /// <summary>
    /// Event handler triggered after the window is loaded that applies the <see cref="Mica"/> effect.
    /// </summary>
    /// <param name="sender">The window whose background is to be set.</param>
    /// <param name="e"><see cref="RoutedEventArgs"/></param>
    public static void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        var window = sender as Window;
        _window = window;
        if (window == null)
        {
            throw new Exception("Only windows can have the Mica effect applied.");
        }

        window.Background = Brushes.Transparent;

        _windowHandle = new WindowInteropHelper(window).Handle;

        PresentationSource.FromVisual(window)!.ContentRendered += OnContentRendered;
        ThemeManager.Current.SystemThemeChanged += Current_SystemThemeChanged;
    }

    private static void Current_SystemThemeChanged(object sender, Data.FunctionEventArgs<ThemeManager.SystemTheme> e)
    {
        var isDark = !WindowHelper.DetermineIfInLightThemeMode();
        ApplyMicaEffect(_windowHandle, isDark);
    }

    private static void OnContentRendered(object sender, EventArgs e)
    {
        var isDark = !WindowHelper.DetermineIfInLightThemeMode();
        ThemeManager.Current.ApplicationTheme = isDark ? ApplicationTheme.Dark : ApplicationTheme.Light;
        ApplyMicaEffect(((HwndSource) sender).Handle, isDark);
    }

    public static void ApplyMicaEffect(IntPtr handle, bool isDark)
    {
        if (handle == IntPtr.Zero)
        {
            return;
        }

        // Hide default TitleBar
        // https://stackoverflow.com/questions/743906/how-to-hide-close-button-in-wpf-window
        try
        {
            if (_window != null && _window.ResizeMode == ResizeMode.NoResize)
            {


            }
            else
            {
                InteropMethods.SetWindowLong(handle, -16, InteropMethods.GetWindowLong(handle, -16) & ~0x80000);
            }
        }
        catch (Exception e)
        {
#if DEBUG
            Console.WriteLine(e);
#endif
        }

        if (isDark)
        {
            InteropMethods.DwmSetWindowAttribute(handle, InteropValues.DWMWINDOWATTRIBUTE.USE_IMMERSIVE_DARK_MODE, ref _pvTrueAttribute,
                Marshal.SizeOf(typeof(int)));
        }
        else
        {
            InteropMethods.DwmSetWindowAttribute(handle, InteropValues.DWMWINDOWATTRIBUTE.USE_IMMERSIVE_DARK_MODE, ref _pvFalseAttribute,
                Marshal.SizeOf(typeof(int)));
        }

        InteropMethods.DwmSetWindowAttribute(handle, InteropValues.DWMWINDOWATTRIBUTE.MICA_EFFECT, ref _pvTrueAttribute,
            Marshal.SizeOf(typeof(int)));


        Containers.Add(handle);

        IsMicaEffectApplied = true;
    }

    public static void RemoveMicaEffect(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
        {
            return;
        }

        InteropMethods.DwmSetWindowAttribute(handle, InteropValues.DWMWINDOWATTRIBUTE.USE_IMMERSIVE_DARK_MODE, ref _pvFalseAttribute,
            Marshal.SizeOf(typeof(int)));

        InteropMethods.DwmSetWindowAttribute(handle, InteropValues.DWMWINDOWATTRIBUTE.MICA_EFFECT, ref _pvFalseAttribute,
            Marshal.SizeOf(typeof(int)));
    }
}
