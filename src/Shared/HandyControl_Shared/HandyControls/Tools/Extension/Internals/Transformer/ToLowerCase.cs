using System.Globalization;

namespace HandyControl.Tools.Extension
{
    internal class ToLowerCase : IStringTransformer
    {
        public string Transform(string input)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToLower(input);
        }
    }
}
