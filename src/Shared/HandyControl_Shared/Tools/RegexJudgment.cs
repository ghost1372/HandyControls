using System;
using System.Linq;
using System.Text.RegularExpressions;
using HandyControl.Data;

namespace HandyControl.Tools
{
    /// <summary>
    ///     Contains some regular verification operations
    /// </summary>
    public static class RegexJudgment
    {
        private static readonly RegexPatterns RegexPatterns = new();

        /// <summary>
        ///     Determine whether the string format meets certain requirements
        /// </summary>
        /// <param name="str">The string to be judged</param>
        /// <param name="pattern">Regular expression</param>
        /// <returns></returns>
        public static bool IsKindOf(this string str, string pattern)
        {
            return Regex.IsMatch(str, pattern);
        }

        /// <summary>
        ///     Determine whether the string meets the specified format
        /// </summary>
        /// <param name="text">The string to be judged</param>
        /// <param name="textType">Specified formatted text</param>
        /// <returns></returns>
        public static bool IsKindOf(this string text, TextType textType)
        {
            if (textType == TextType.Common) return true;
            return Regex.IsMatch(text,
                RegexPatterns.GetValue(Enum.GetName(typeof(TextType), textType) + "Pattern").ToString());
        }

        /// <summary>
        ///     Determine whether the string format is email
        /// </summary>
        /// <param name="email">Email string to be judged</param>
        /// <returns>Method returns boolean</returns>
        public static bool IsEmail(this string email)
        {
            return Regex.IsMatch(email, RegexPatterns.MailPattern);
        }

        /// <summary>
        ///     Determine whether the string format is a specified type of IP address
        /// </summary>
        /// <param name="ip">IP string to be judged</param>
        /// <param name="ipType">Specified IP type</param>
        /// <returns>Method returns boolean</returns>
        public static bool IsIp(this string ip, IpType ipType)
        {
            return ipType switch
            {
                IpType.A => Regex.IsMatch(ip, RegexPatterns.IpAPattern),
                IpType.B => Regex.IsMatch(ip, RegexPatterns.IpBPattern),
                IpType.C => Regex.IsMatch(ip, RegexPatterns.IpCPattern),
                IpType.D => Regex.IsMatch(ip, RegexPatterns.IpDPattern),
                IpType.E => Regex.IsMatch(ip, RegexPatterns.IpEPattern),
                _ => false
            };
        }

        /// <summary>
        ///     Determine whether the string format is an IP address
        /// </summary>
        /// <param name="ip">IP string to be judged</param>
        /// <returns>Method returns boolean</returns>
        public static bool IsIp(this string ip)
        {
            return Regex.IsMatch(ip, RegexPatterns.IpPattern);
        }

        /// <summary>
        ///     Determine whether the string format is a single Chinese character
        /// </summary>
        /// <param name="str">Single Chinese character string to be judged</param>
        /// <returns>Method returns boolean</returns>
        public static bool IsChinese(this string str)
        {
            return Regex.IsMatch(str, RegexPatterns.ChinesePattern);
        }

        /// <summary>
        ///     Determine whether the string format is url
        /// </summary>
        /// <param name="str">URL string to be judged</param>
        /// <returns>Method returns boolean</returns>
        public static bool IsUrl(this string str)
        {
            return Regex.IsMatch(str, RegexPatterns.UrlPattern);
        }

        /// <summary>
        ///    Determine whether the string format is persian
        /// </summary>
        /// <param name="str">string to be judged</param>
        /// <returns>Method returns boolean</returns>
        public static bool IsPersian(this string str)
        {
            return Regex.IsMatch(str, RegexPatterns.PersianPattern);
        }

        /// <summary>
        ///     Determine whether the string format is Iraniana National Code
        /// </summary>
        /// <param name="str">string to be judged</param>
        /// <returns></returns>
        public static bool IsIranNationalCode(this string str)
        {
            // input has 10 digits that all of them are not equal
            if (!Regex.IsMatch(str, RegexPatterns.IranNationalCodePattern))
                return false;

            var check = Convert.ToInt32(str.Substring(9, 1));
            var sum = System.Linq.Enumerable.Range(0, 9)
                .Select(x => Convert.ToInt32(str.Substring(x, 1)) * (10 - x))
                .Sum() % 11;

            return sum < 2 && check == sum || sum >= 2 && check + sum == 11;
        }
    }
}
