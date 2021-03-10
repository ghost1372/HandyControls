using System;
using System.Linq;
using System.Text;
using System.IO;
namespace HandyControl.Tools.Extension
{
    public static class StringExtension1
    {
        /// <summary>
        /// Generate MD5 Hash 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GenerateMD5(this string input) => CryptographyHelper.GenerateMD5(input);

        /// <summary>
        /// Generate SHA256 Hash
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GenerateSHA256(this string input) => CryptographyHelper.GenerateSHA256(input);

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

        /// <summary>
        /// Transforms a string using the provided transformers. Transformations are applied in the provided order.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="transformers"></param>
        /// <returns></returns>
        public static string Transform(this string input, params IStringTransformer[] transformers)
        {
            return transformers.Aggregate(input, (current, stringTransformer) => stringTransformer.Transform(current));
        }

        /// <summary>
        /// Truncate the string
        /// </summary>
        /// <param name="input">The string to be truncated</param>
        /// <param name="length">The length to truncate to</param>
        /// <returns>The truncated string</returns>
        public static string Truncate(this string input, int length)
        {
            return input.Truncate(length, "…", Truncator.FixedLength);
        }

        /// <summary>
        /// Truncate the string
        /// </summary>
        /// <param name="input">The string to be truncated</param>
        /// <param name="length">The length to truncate to</param>
        /// <param name="truncator">The truncate to use</param>
        /// <param name="from">The enum value used to determine from where to truncate the string</param>
        /// <returns>The truncated string</returns>
        public static string Truncate(this string input, int length, ITruncator truncator,
            TruncateFrom from = TruncateFrom.Right)
        {
            return input.Truncate(length, "…", truncator, from);
        }

        /// <summary>
        /// Truncate the string
        /// </summary>
        /// <param name="input">The string to be truncated</param>
        /// <param name="length">The length to truncate to</param>
        /// <param name="truncationString">The string used to truncate with</param>
        /// <param name="from">The enum value used to determine from where to truncate the string</param>
        /// <returns>The truncated string</returns>
        public static string Truncate(this string input, int length, string truncationString,
            TruncateFrom from = TruncateFrom.Right)
        {
            return input.Truncate(length, truncationString, Truncator.FixedLength, from);
        }

        /// <summary>
        /// Truncate the string
        /// </summary>
        /// <param name="input">The string to be truncated</param>
        /// <param name="length">The length to truncate to</param>
        /// <param name="truncationString">The string used to truncate with</param>
        /// <param name="truncator">The truncator to use</param>
        /// <param name="from">The enum value used to determine from where to truncate the string</param>
        /// <returns>The truncated string</returns>
        public static string Truncate(this string input, int length, string truncationString, ITruncator truncator,
            TruncateFrom from = TruncateFrom.Right)
        {
            if (truncator == null)
            {
                throw new ArgumentNullException(nameof(truncator));
            }

            if (input == null)
            {
                return null;
            }

            return truncator.Truncate(input, length, truncationString, from);
        }

        /// <summary>
        ///     Converts this <see cref="string"/> from one encoding to another.
        /// </summary>
        /// <param name="value">The input <see cref="string"/>.</param>
        /// <param name="from">The input encoding.</param>
        /// <param name="to">The output encoding.</param>
        /// <returns>A new <see cref="string"/> with its data converted to <paramref name="to"/>.</returns>
        public static string ChangeEncoding(this string value, Encoding from, Encoding to) => to.GetString(value.GetBytes(from));

        public static byte[] GetBytes(this string value, Encoding? encoding = null) => (encoding ?? Encoding.Default).GetBytes(value);

         public static DirectoryInfo ToDirectoryInfo(this string value)
        {
            return new DirectoryInfo(value);
        }

        public static FileInfo ToFileInfo(this string value)
        {
            return new FileInfo(value);
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }
    }
}
