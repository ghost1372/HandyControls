namespace HandyControl.Tools.Extension
{
    public static class ConvertTypes
    {
        /// <summary>
        /// Converts any type in to an Int32
        /// </summary>
        /// <typeparam name="T">Any Object</typeparam>
        /// <param name="value">Value to convert</param>
        /// <returns>The integer, 0 if unsuccessful</returns>
        public static int ToInt32<T>(this T value)
        {
            int result;
            if (int.TryParse(value.ToString(), out result))
            {
                return result;
            }
            return 0;
        }

        /// <summary>
        /// Converts any type in to an Int32 but if null then returns the default
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <typeparam name="T">Any Object</typeparam>
        /// <param name="defaultValue">Default to use</param>
        /// <returns>The defaultValue if unsuccessful</returns>
        public static int ToInt32<T>(this T value, int defaultValue)
        {
            int result;
            if (int.TryParse(value.ToString(), out result))
            {
                return result;
            }
            return defaultValue;
        }
    }
}
