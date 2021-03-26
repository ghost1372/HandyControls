using System.Collections.Generic;
using System.Windows.Media;


namespace HandyControl.Tools.Extension
{
    /// <summary>
    ///     Color extension class
    /// </summary>
    public static class ColorExtension
    {
        /// <summary>
        ///     Convert color to decimal representation (rgb order is reversed)
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static int ToInt32(this Color color) => color.R << 16 | color.G << 8 | color.B;

        /// <summary>
        ///     Convert color to decimal representation (rgb order is reversed)
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static int ToInt32Reverse(this Color color) => color.R | color.G << 8 | color.B << 18;

        internal static List<byte> ToList(this Color color) => new List<byte>
        {
            color.R,
            color.G,
            color.B
        };
    }
}
