using System.Globalization;
namespace HandyControl.Tools.Extension;

public interface ICulturedStringTransformer : IStringTransformer
{
    /// <summary>
    /// Transform the input
    /// </summary>
    /// <param name="input">String to be transformed</param>
    /// <param name="culture">The culture</param>
    /// <returns></returns>
    string Transform(string input, CultureInfo culture);
}
