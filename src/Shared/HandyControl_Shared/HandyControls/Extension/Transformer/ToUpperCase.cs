namespace HandyControl.Tools.Extension
{
    internal class ToUpperCase : IStringTransformer
    {
        public string Transform(string input)
        {
            return input.ToUpper();
        }
    }
}
