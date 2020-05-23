namespace HandyControl.Tools.Extension
{
    public static class InlineFormatString
    {
        /// <summary>
        /// Enable quick and more natural string.Format calls
        /// </summary>
        /// <param name="input"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Format(this string input, params object[] args)
        {
            return string.Format(input, args);
        }
    }
}
