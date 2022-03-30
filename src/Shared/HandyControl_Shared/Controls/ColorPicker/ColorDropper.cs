using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using HandyControl.Data;
using HandyControl.Tools;
using HandyControl.Tools.Interop;

namespace HandyControl.Controls;

internal class ColorDropper
{
    private bool _cursorIsSetted;

    private readonly ColorPicker _colorPicker;

    Window window;
    Rectangle rect;
    TextBlock txtHex;
    public ColorDropper(ColorPicker colorPicker)
    {
        _colorPicker = colorPicker;
    }

    private void DrawPreviewWindow()
    {
        window = new Window
        {
            ShowInTaskbar = false,
            Width = 100,
            Height = 125,
            ResizeMode = ResizeMode.NoResize,
            WindowStyle = WindowStyle.None,
            Background = Brushes.Transparent,
            AllowsTransparency = true,
            BorderThickness = new Thickness(0),
            Title = "XColorDropperWindowX"
        };
        Border border = new Border
        {
            BorderThickness = new Thickness(1),
            BorderBrush = ResourceHelper.GetResource<Brush>(ResourceToken.BorderBrush),
            Background = ResourceHelper.GetResource<Brush>(ResourceToken.SecondaryRegionBrush),
            CornerRadius = new CornerRadius(6)
        };

        rect = new Rectangle
        {
            Margin = new Thickness(5),
            Stretch = Stretch.UniformToFill
        };

        txtHex = new TextBlock()
        {
            Margin = new Thickness(0, 0, 0, 10),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = ResourceHelper.GetResource<Brush>(ResourceToken.PrimaryTextBrush),
            FontSize = 14
        };

        var panel = new StackPanel();
        panel.Children.Add(rect);
        panel.Children.Add(txtHex);
        border.Child = panel;
        window.Content = border;
        window.Show();
    }

    public void Update(bool isShow)
    {
        if (isShow)
        {
            Mouse.OverrideCursor = Cursors.Cross;
            MouseHook.Start();
            MouseHook.StatusChanged += MouseHook_StatusChanged;
            DrawPreviewWindow();
            MoveWindowNextToMouse();
        }
        else
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            MouseHook.Stop();
            MouseHook.StatusChanged -= MouseHook_StatusChanged;
            ColorPicker.IsCheckedToggleButtonDropper(false);
            window?.Close();
        }
    }

    private void MoveWindowNextToMouse()
    {
        string title;
        IntPtr handle;

        InteropMethods.FindWindowTitleMatch(window.Title, out handle, out title);
        if (handle == IntPtr.Zero)
        {
            return;
        }

        // Restore the window.
        InteropMethods.SetWindowPlacement(handle, InteropValues.ShowWindowCommands.Restore);

        var position = InteropMethods.GetCursorPos();
        InteropMethods.SetWindowPos(handle, IntPtr.Zero, (int) position.X - 20, (int) position.Y - -20, 0, 0, (int) InteropValues.SetWindowPosFlags.IgnoreZOrder | (int) InteropValues.SetWindowPosFlags.IgnoreResize | (int) InteropValues.SetWindowPosFlags.ShowWindow);
    }
    private void MouseHook_StatusChanged(object sender, MouseHookEventArgs e)
    {
        UpdateCursor(true);
        var brush = new SolidColorBrush(GetColorAt(e.Point.X, e.Point.Y));
        _colorPicker.SelectedBrush = brush;
        rect.Fill = brush;
        txtHex.Text = ColorHelper.GetHexFromBrush(brush).ToUpper();
        MoveWindowNextToMouse();
        if (e.MessageType == MouseHookMessageType.LeftButtonDown)
        {
            UpdateCursor(false);
            Update(false);
        }
    }

    private void UpdateCursor(bool isDropper)
    {
        if (isDropper)
        {
            Mouse.Captured?.ReleaseMouseCapture();

            if (!_cursorIsSetted)
            {
                Mouse.OverrideCursor = Cursors.Cross;
                _cursorIsSetted = true;
            }
        }
        else
        {
            if (_cursorIsSetted)
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                _cursorIsSetted = false;
            }
        }
    }

    public static Color GetColorAt(int x, int y)
    {
        var desk = InteropMethods.GetDesktopWindow();
        var dc = InteropMethods.GetWindowDC(desk);
        var a = (int) InteropMethods.GetPixel(dc, x, y);
        InteropMethods.ReleaseDC(desk, dc);
        return Color.FromArgb(255, (byte) ((a >> 0) & 0xff), (byte) ((a >> 8) & 0xff), (byte) ((a >> 16) & 0xff));
    }
}
