using System.Globalization;

namespace HandyControl.Tools.Extension;

internal class ToLowerCase : ICulturedStringTransformer
{
    public string Transform(string input)
    {
        return Transform(input, null);
    }

    public string Transform(string input, CultureInfo culture)
    {
        culture ??= CultureInfo.CurrentCulture;

        return culture.TextInfo.ToLower(input);
    }
}
