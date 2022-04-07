using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using HandyControl.Themes;
using HandyControl.Tools.Interop;
#if NET40
using Microsoft.Windows.Shell;
#else
using System.Windows.Shell;
#endif

namespace HandyControl.Tools;

public enum BackdropType
{
    Auto = 1,
    Mica = 2,
    Acrylic = 3,
    Tabbed = 4,
    Disable = 5
}

/// <summary>
/// Contains static handlers for applying background Mica effects from Windows 11.
/// </summary>
public static class MicaHelper
{
    private static System.Windows.Window _window;

    private static int _pvTrueAttribute = 0x01;

    private static int _pvFalseAttribute = 0x00;

    /// <summary>
    /// Checks if the current <see cref="Windows"/> supports selected <see cref="BackgroundType"/>.
    /// </summary>
    /// <param name="type">Background type to check.</param>
    /// <returns><see langword="true"/> if <see cref="BackgroundType"/> is supported.</returns>
    public static bool IsSupported(this BackdropType type)
    {
        if (!OSVersionHelper.IsWindowsNT) { return false; }

        return type switch
        {
            BackdropType.Auto => OSVersionHelper.OSVersion >= new Version(10, 0, 22523), // Insider with new API                
            BackdropType.Tabbed => OSVersionHelper.OSVersion >= new Version(10, 0, 22523),
            BackdropType.Mica => OSVersionHelper.OSVersion >= new Version(10, 0, 22000),
            BackdropType.Acrylic => (OSVersionHelper.OSVersion >= new Version(6, 0) && OSVersionHelper.OSVersion < new Version(6, 3)) || (OSVersionHelper.OSVersion >= new Version(10, 0) && OSVersionHelper.OSVersion < new Version(10, 0, 22000)) || OSVersionHelper.OSVersion >= new Version(10, 0, 22523),
            _ => false
        };
    }

    /// <summary>
    /// Applies selected background effect to <see cref="Window"/> when is rendered.
    /// </summary>
    /// <param name="window">Window to apply effect.</param>
    /// <param name="type">Background type.</param>
    /// <param name="force">Skip the compatibility check.</param>
    public static bool Apply(this System.Windows.Window window, BackdropType type, bool force = false)
    {
        if (!force && (!IsSupported(type))) { return false; }

        var windowHandle = new WindowInteropHelper(window).EnsureHandle();
        _window = window;

        if (windowHandle == IntPtr.Zero) { return false; }

        if (window is not HandyControl.Controls.Window)
        {
            void SetStyle()
            {
                if (window.Style != null)
                {
                    foreach (Setter setter in window.Style.Setters)
                    {
                        if (setter.Property == Control.BackgroundProperty && setter.Value == Brushes.Transparent)
                        {
                            goto stylesetted;
                        }
                    }
                    Style style = new Style
                    {
                        TargetType = typeof(Window),
                        BasedOn = window.Style
                    };
                    style.Setters.Add(new Setter
                    {
                        Property = FrameworkElement.TagProperty,
                        Value = true
                    });
                    style.Setters.Add(new Setter
                    {
                        Property = Control.BackgroundProperty,
                        Value = Brushes.Transparent
                    });
                    window.Style = style;
stylesetted:;
                }
                else
                {
                    Style style = new Style
                    {
                        TargetType = typeof(Window)
                    };
                    style.Setters.Add(new Setter
                    {
                        Property = FrameworkElement.TagProperty,
                        Value = true
                    });
                    style.Setters.Add(new Setter
                    {
                        Property = Control.BackgroundProperty,
                        Value = Brushes.Transparent
                    });
                    window.Style = style;
                }
            }

            if (window.IsLoaded)
            {
                SetStyle();
            }
            else
            {
                window.Loaded += (sender, e) => SetStyle();
            }
        }
        var chrome = GetWindowChrome();
        WindowChrome.SetWindowChrome(window, chrome);
        Apply(windowHandle, type);
        
        return true;
    }

    /// <summary>
    /// Applies selected background effect to <c>hWnd</c> by it's pointer.
    /// </summary>
    /// <param name="handle">Pointer to the window handle.</param>
    /// <param name="type">Background type.</param>
    /// <param name="force">Skip the compatibility check.</param>
    public static bool Apply(this IntPtr handle, BackdropType type, bool force = false)
    {
        if (!force && (!IsSupported(type))) { return false; }

        if (handle == IntPtr.Zero) { return false; }

        if (ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark) { ApplyDarkMode(handle); }

        return type switch
        {
            BackdropType.Auto => TryApplyAuto(handle),
            BackdropType.Mica => TryApplyMica(handle),
            BackdropType.Acrylic => TryApplyAcrylic(handle),
            BackdropType.Tabbed => TryApplyTabbed(handle),
            _ => false
        };
    }

    /// <summary>
    /// Tries to remove background effects if they have been applied to the <see cref="Window"/>.
    /// </summary>
    /// <param name="window">The window from which the effect should be removed.</param>
    public static void Remove(this System.Windows.Window window)
    {
        var windowHandle = new WindowInteropHelper(window).EnsureHandle();

        if (windowHandle == IntPtr.Zero) return;

        if (window is not HandyControl.Controls.Window)
        {
            if (window.Style != null)
            {
                foreach (Setter setter in window.Style.Setters)
                {
                    if (setter.Property == FrameworkElement.TagProperty && setter.Value is bool boolen && boolen)
                    {
                        if (window.Style.BasedOn != null)
                        {
                            window.Style = window.Style.BasedOn;
                        }
                        else
                        {
                            window.ClearValue(FrameworkElement.StyleProperty);
                        }
                    }
                }
            }
        }

        Remove(windowHandle);
    }

    /// <summary>
    /// Tries to remove all effects if they have been applied to the <c>hWnd</c>.
    /// </summary>
    /// <param name="handle">Pointer to the window handle.</param>
    public static void Remove(this IntPtr handle)
    {
        if (handle == IntPtr.Zero) return;

        int backdropPvAttribute = (int) InteropValues.DWMSBT.DWMSBT_DISABLE;

        RemoveDarkMode(handle);

        InteropMethods.DwmSetWindowAttribute(handle, InteropValues.DWMWINDOWATTRIBUTE.DWMWA_MICA_EFFECT, ref _pvFalseAttribute,
            Marshal.SizeOf(typeof(int)));

        InteropMethods.DwmSetWindowAttribute(handle, InteropValues.DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
            ref backdropPvAttribute,
            Marshal.SizeOf(typeof(int)));
    }

    /// <summary>
    /// Tries to inform the operating system that this window uses dark mode.
    /// </summary>
    /// <param name="window">Window to apply effect.</param>
    public static void ApplyDarkMode(this System.Windows.Window window)
    {
        var windowHandle = new WindowInteropHelper(window).EnsureHandle();

        if (windowHandle == IntPtr.Zero) return;
        ApplyDarkMode(windowHandle);
    }

    /// <summary>
    /// Tries to inform the operating system that this <c>hWnd</c> uses dark mode.
    /// </summary>
    /// <param name="handle">Pointer to the window handle.</param>
    public static void ApplyDarkMode(this IntPtr handle)
    {
        if (handle == IntPtr.Zero) return;

        var dwAttribute = InteropValues.DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE;

        if (OSVersionHelper.OSVersion < new Version(10, 0, 18985))
        {
            dwAttribute = InteropValues.DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE_OLD;
        }

        InteropMethods.DwmSetWindowAttribute(handle, dwAttribute,
            ref _pvTrueAttribute,
            Marshal.SizeOf(typeof(int)));
    }

    /// <summary>
    /// Tries to clear the dark theme usage information.
    /// </summary>
    /// <param name="window">Window to remove effect.</param>
    public static void RemoveDarkMode(this System.Windows.Window window)
    {
        var windowHandle = new WindowInteropHelper(window).EnsureHandle();

        if (windowHandle == IntPtr.Zero) return;
        RemoveDarkMode(windowHandle);
    }

    /// <summary>
    /// Tries to clear the dark theme usage information.
    /// </summary>
    /// <param name="handle">Pointer to the window handle.</param>
    public static void RemoveDarkMode(this IntPtr handle)
    {
        if (handle == IntPtr.Zero) { return; }

        var dwAttribute = InteropValues.DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE;

        if (OSVersionHelper.OSVersion < new Version(10, 0, 18985))
        {
            dwAttribute = InteropValues.DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE_OLD;
        }

        InteropMethods.DwmSetWindowAttribute(handle, dwAttribute,
            ref _pvFalseAttribute,
            Marshal.SizeOf(typeof(int)));
    }

    /// <summary>
    /// Tries to remove default TitleBar from <c>hWnd</c>.
    /// </summary>
    /// <param name="handle">Pointer to the window handle.</param>
    /// <returns><see langowrd="false"/> is problem occurs.</returns>
    public static bool RemoveTitleBar(this IntPtr handle)
    {
        // Hide default TitleBar
        // https://stackoverflow.com/questions/743906/how-to-hide-close-button-in-wpf-window
        try
        {
            if (_window != null && _window is HandyControl.Controls.Window && _window.ResizeMode == ResizeMode.NoResize)
            {
                return false;
            }
            else
            {
                InteropMethods.SetWindowLong(handle, -16, InteropMethods.GetWindowLong(handle, -16) & ~0x80000);
                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool TryApplyAuto(this IntPtr handle)
    {
        int backdropPvAttribute = (int) InteropValues.DWMSBT.DWMSBT_AUTO;

        InteropMethods.DwmSetWindowAttribute(handle, InteropValues.DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
            ref backdropPvAttribute,
            Marshal.SizeOf(typeof(int)));

        return true;
    }

    public static bool TryApplyTabbed(this IntPtr handle)
    {
        int backdropPvAttribute = (int) InteropValues.DWMSBT.DWMSBT_TABBEDWINDOW;

        InteropMethods.DwmSetWindowAttribute(handle, InteropValues.DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
            ref backdropPvAttribute,
            Marshal.SizeOf(typeof(int)));

        return true;
    }

    public static bool TryApplyMica(this IntPtr handle)
    {
        int backdropPvAttribute;

        if (OSVersionHelper.OSVersion >= new Version(10, 0, 22523))
        {
            backdropPvAttribute = (int) InteropValues.DWMSBT.DWMSBT_MAINWINDOW;

            InteropMethods.DwmSetWindowAttribute(handle, InteropValues.DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
                ref backdropPvAttribute,
                Marshal.SizeOf(typeof(int)));

            return true;
        }

        if (!RemoveTitleBar(handle)) { return false; }

        backdropPvAttribute = _pvTrueAttribute;

        InteropMethods.DwmSetWindowAttribute(handle, InteropValues.DWMWINDOWATTRIBUTE.DWMWA_MICA_EFFECT,
            ref backdropPvAttribute,
            Marshal.SizeOf(typeof(int)));

        return true;
    }

    public static bool TryApplyAcrylic(this IntPtr handle)
    {
        if (OSVersionHelper.OSVersion >= new Version(10, 0, 22523))
        {
            int backdropPvAttribute = (int) InteropValues.DWMSBT.DWMSBT_TRANSIENTWINDOW;

            InteropMethods.DwmSetWindowAttribute(handle, InteropValues.DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
                ref backdropPvAttribute,
                Marshal.SizeOf(typeof(int)));

            return true;
        }

        if (OSVersionHelper.OSVersion >= new Version(10, 0, 17763))
        {
            InteropValues.ACCENTPOLICY accentPolicy = new InteropValues.ACCENTPOLICY
            {
                AccentState = InteropValues.ACCENTSTATE.ACCENT_ENABLE_ACRYLICBLURBEHIND,
                GradientColor = (0 << 24) | (0x990000 & 0xFFFFFF)
            };

            int accentStructSize = Marshal.SizeOf(accentPolicy);

            IntPtr accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accentPolicy, accentPtr, false);

            InteropValues.WINCOMPATTRDATA data = new InteropValues.WINCOMPATTRDATA
            {
                Attribute = InteropValues.WINDOWCOMPOSITIONATTRIB.WCA_ACCENT_POLICY,
                DataSize = accentStructSize,
                Data = accentPtr
            };

            InteropMethods.SetWindowCompositionAttribute(handle, ref data);
            Marshal.FreeHGlobal(accentPtr);

            return true;
        }

        if (OSVersionHelper.OSVersion >= new Version(10, 0))
        {
            InteropValues.ACCENTPOLICY accentPolicy = new InteropValues.ACCENTPOLICY
            {
                AccentState = InteropValues.ACCENTSTATE.ACCENT_ENABLE_BLURBEHIND,
            };

            int accentStructSize = Marshal.SizeOf(accentPolicy);

            IntPtr accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accentPolicy, accentPtr, false);

            InteropValues.WINCOMPATTRDATA data = new InteropValues.WINCOMPATTRDATA
            {
                Attribute = InteropValues.WINDOWCOMPOSITIONATTRIB.WCA_ACCENT_POLICY,
                DataSize = accentStructSize,
                Data = accentPtr
            };

            InteropMethods.SetWindowCompositionAttribute(handle, ref data);

            Marshal.FreeHGlobal(accentPtr);

            return true;
        }

        if (OSVersionHelper.OSVersion >= new Version(6, 0))
        {
            return true;
        }

        return false;
    }

    private static WindowChrome GetWindowChrome()
    {
#if NET40
            var chrome = new WindowChrome
            {
                CornerRadius = new CornerRadius(),
                GlassFrameThickness = new Thickness(-1),
                ResizeBorderThickness = new Thickness(8)
            };
#else
        var chrome = new WindowChrome
        {
            CornerRadius = new CornerRadius(),
            ResizeBorderThickness = new Thickness(8),
            GlassFrameThickness = new Thickness(-1),
            NonClientFrameEdges = NonClientFrameEdges.None,
            UseAeroCaptionButtons = false
        };
#endif
        return chrome;
    }
}
