using System;
using System.Windows.Media;
using System.Globalization;
using System.Reflection;
namespace HandyControl.Tools;

public static class ColorHelper
{
    private const int Win32RedShift = 0;
    private const int Win32GreenShift = 8;
    private const int Win32BlueShift = 16;

    public static int ToWin32(Color c) => c.R << Win32RedShift | c.G << Win32GreenShift | c.B << Win32BlueShift;

    public static Color ToColor(uint c)
    {
        var bytes = BitConverter.GetBytes(c);
        return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
    }

    /// <summary>
    /// Get Color from LinearGradientBrush, SolidColorBrush and Brush
    /// </summary>
    /// <param name="brush"></param>
    /// <returns></returns>
    public static Color GetColorFromBrush(Brush brush)
    {
        if (brush.GetType() == typeof(LinearGradientBrush))
        {
            var linearBrush = (LinearGradientBrush) brush;
            return new SolidColorBrush(linearBrush.GradientStops[1].Color).Color;
        }
        else if (brush.GetType() == typeof(SolidColorBrush))
        {
            var solidBrush = (SolidColorBrush) brush;
            return Color.FromArgb(solidBrush.Color.A, solidBrush.Color.R, solidBrush.Color.G, solidBrush.Color.B);
        }
        else
        {
            return ((SolidColorBrush) brush).Color;
        }
    }

    /// <summary>
    /// Get Hex Code from Color
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static string GetHexFromColor(Color color)
    {
        return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    /// <summary>
    /// Get Hex Code from Brush
    /// </summary>
    /// <param name="brush"></param>
    /// <returns></returns>
    public static string GetHexFromBrush(Brush brush)
    {
        var color = GetColorFromBrush(brush);
        return GetHexFromColor(color);
    }

#if !NET40
    /// <summary>
    /// Creates a Color from a XAML color string.
    /// </summary>
    /// <param name="colorString"></param>
    /// <returns></returns>
    public static Color GetColorFromString(string colorString)
    {
        if (string.IsNullOrEmpty(colorString))
        {
            throw new ArgumentException(nameof(colorString));
        }

        if (colorString[0] == '#')
        {
            switch (colorString.Length)
            {
                case 9:
                    {
                        var cuint = Convert.ToUInt32(colorString.Substring(1), 16);
                        var a = (byte) (cuint >> 24);
                        var r = (byte) ((cuint >> 16) & 0xff);
                        var g = (byte) ((cuint >> 8) & 0xff);
                        var b = (byte) (cuint & 0xff);

                        return Color.FromArgb(a, r, g, b);
                    }

                case 7:
                    {
                        var cuint = Convert.ToUInt32(colorString.Substring(1), 16);
                        var r = (byte) ((cuint >> 16) & 0xff);
                        var g = (byte) ((cuint >> 8) & 0xff);
                        var b = (byte) (cuint & 0xff);

                        return Color.FromArgb(255, r, g, b);
                    }

                case 5:
                    {
                        var cuint = Convert.ToUInt16(colorString.Substring(1), 16);
                        var a = (byte) (cuint >> 12);
                        var r = (byte) ((cuint >> 8) & 0xf);
                        var g = (byte) ((cuint >> 4) & 0xf);
                        var b = (byte) (cuint & 0xf);
                        a = (byte) (a << 4 | a);
                        r = (byte) (r << 4 | r);
                        g = (byte) (g << 4 | g);
                        b = (byte) (b << 4 | b);

                        return Color.FromArgb(a, r, g, b);
                    }

                case 4:
                    {
                        var cuint = Convert.ToUInt16(colorString.Substring(1), 16);
                        var r = (byte) ((cuint >> 8) & 0xf);
                        var g = (byte) ((cuint >> 4) & 0xf);
                        var b = (byte) (cuint & 0xf);
                        r = (byte) (r << 4 | r);
                        g = (byte) (g << 4 | g);
                        b = (byte) (b << 4 | b);

                        return Color.FromArgb(255, r, g, b);
                    }

                default:
                    throw new FormatException(string.Format("The {0} string passed in the colorString argument is not a recognized Color format.", colorString));
            }
        }

        if (colorString.Length > 3 && colorString[0] == 's' && colorString[1] == 'c' && colorString[2] == '#')
        {
            var values = colorString.Split(',');

            if (values.Length == 4)
            {
                var scA = double.Parse(values[0].Substring(3), CultureInfo.InvariantCulture);
                var scR = double.Parse(values[1], CultureInfo.InvariantCulture);
                var scG = double.Parse(values[2], CultureInfo.InvariantCulture);
                var scB = double.Parse(values[3], CultureInfo.InvariantCulture);

                return Color.FromArgb((byte) (scA * 255), (byte) (scR * 255), (byte) (scG * 255), (byte) (scB * 255));
            }

            if (values.Length == 3)
            {
                var scR = double.Parse(values[0].Substring(3), CultureInfo.InvariantCulture);
                var scG = double.Parse(values[1], CultureInfo.InvariantCulture);
                var scB = double.Parse(values[2], CultureInfo.InvariantCulture);

                return Color.FromArgb(255, (byte) (scR * 255), (byte) (scG * 255), (byte) (scB * 255));
            }

            throw new FormatException(string.Format("The {0} string passed in the colorString argument is not a recognized Color format (sc#[scA,]scR,scG,scB).", colorString));
        }

        var prop = typeof(Colors).GetTypeInfo().GetDeclaredProperty(colorString);

        if (prop != null)
        {
            return (Color) prop.GetValue(null);
        }

        throw new FormatException(string.Format("The {0} string passed in the colorString argument is not a recognized Color.", colorString));
    }
#endif
}
