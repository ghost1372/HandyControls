using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using HandyControl.Controls;

namespace HandyControl.Tools.Extension
{
    public static class Extensions
    {
        #region String
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
        public static string Truncate(this string input, int length, ITruncator truncator, TruncateFrom from = TruncateFrom.Right)
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
        public static string Truncate(this string input, int length, string truncationString, TruncateFrom from = TruncateFrom.Right)
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
        public static string Truncate(this string input, int length, string truncationString, ITruncator truncator, TruncateFrom from = TruncateFrom.Right)
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
        #endregion

        #region T
        /// <summary>
        /// This Extension Method Help you to Add Items into ObservableCollection from Another Thread
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        public static void AddOnUI<T>(this ICollection<T> collection, T item)
        {
            Action<T> addMethod = collection.Add;
            Application.Current.Dispatcher.BeginInvoke(addMethod, item);
        }

        /// <summary>
        /// Compare between 2 things
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public static bool Between<T>(this T actual, T lower, T upper) where T : IComparable<T>
        {
            return actual.CompareTo(lower) >= 0 && actual.CompareTo(upper) < 0;
        }

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

        /// <summary>
        /// This Extension Help you to access item index in foreach loop
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static IEnumerable<EnumeratorWithIndex<T>> GetEnumeratorWithIndex<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Select(EnumeratorWithIndex<T>.Create);
        }
        #endregion

        #region DateTime
        /// <summary>
        /// Get Shamsi Date From Miladi Year
        /// </summary>
        /// <param name="dateTime">Enter The Jalali DateTime</param>
        /// <returns></returns>
        public static string ToShamsiDate(this DateTime dateTime)
        {
            PersianDate persianDateShamsi = new PersianDate();
            return persianDateShamsi.GetShamsiYearToString(dateTime) + "/" + persianDateShamsi.GetShamsiMonthString(dateTime) + "/" + persianDateShamsi.GetShamsiDayString(dateTime);
        }
        /// <summary>
        /// Get Short Shamsi Date From Miladi Year
        /// </summary>
        /// <param name="dateTime">Enter The Jalali DateTime</param>
        /// <returns></returns>
        public static string ToShortShamsiDate(this DateTime dateTime)
        {
            PersianDate persianDateShamsi = new PersianDate();
            return persianDateShamsi.GetShortShamsiYear(dateTime) + "/" + persianDateShamsi.GetShamsiMonthString(dateTime) + "/" + persianDateShamsi.GetShamsiDayString(dateTime);
        }
        /// <summary>
        /// Get Long Shamsi Date From Miladi Year
        /// </summary>
        /// <param name="dateTime">Enter The Jalali DateTime</param>
        /// <returns></returns>
        public static string ToLongShamsiDate(this DateTime dateTime)
        {
            PersianDate persianDateShamsi = new PersianDate();
            return persianDateShamsi.GetShamsiDayName(dateTime) + " " + persianDateShamsi.GetShamsiDay(dateTime) + " " + persianDateShamsi.GetShamsiMonthName(dateTime) + " " + persianDateShamsi.GetShamsiYear(dateTime);
        }
        #endregion

        #region Application
        /// <summary>
        /// Get the path of the executable file of the current application, including the name of the executable file.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The path of the executable file of the current application, including the name of the executable file</returns>
        public static string ExecutablePath(this Application value)
        {
            Uri uri = new Uri(Assembly.GetEntryAssembly().CodeBase);
            if (uri.IsFile) return uri.LocalPath + Uri.UnescapeDataString(uri.Fragment);
            else return uri.ToString();
        }

        /// <summary>
        /// Close the application and start a new instance immediately
        /// </summary>
        /// <param name="value"></param>
        public static void Restart(this Application value)
        {
            string cmdLine = Environment.CommandLine;
            string cmdLineArgs0 = Environment.GetCommandLineArgs()[0];
            int i = cmdLine.IndexOf(' ', cmdLine.IndexOf(cmdLineArgs0) + cmdLineArgs0.Length);
            cmdLine = cmdLine.Remove(0, i + 1);

            ProcessStartInfo startInfo = Process.GetCurrentProcess().StartInfo;
            startInfo.FileName = value.ExecutablePath();
            startInfo.Arguments = cmdLine;
            value.Shutdown();
            Process.Start(startInfo);
        }
        #endregion
    }
}
