using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
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

        /// <summary>
        /// This Extension Help you to Easily implement search, sort, and group operations
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static CollectionViewExtension<TSource> ShapeView<TSource>(this IEnumerable<TSource> source)
        {
            var view = CollectionViewSource.GetDefaultView(source);
            return new CollectionViewExtension<TSource>(view);
        }

        /// <summary>
        /// This Extension Help you to Easily implement search, sort, and group operations
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="view"></param>
        /// <returns></returns>
        public static CollectionViewExtension<TSource> Shape<TSource>(this ICollectionView view)
        {
            return new CollectionViewExtension<TSource>(view);
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
            PersianDateTime persianDateShamsi = new PersianDateTime();
            return persianDateShamsi.GetShamsiYearToString(dateTime) + "/" + persianDateShamsi.GetShamsiMonthString(dateTime) + "/" + persianDateShamsi.GetShamsiDayString(dateTime);
        }
        /// <summary>
        /// Get Short Shamsi Date From Miladi Year
        /// </summary>
        /// <param name="dateTime">Enter The Jalali DateTime</param>
        /// <returns></returns>
        public static string ToShortShamsiDate(this DateTime dateTime)
        {
            PersianDateTime persianDateShamsi = new PersianDateTime();
            return persianDateShamsi.GetShortShamsiYear(dateTime) + "/" + persianDateShamsi.GetShamsiMonthString(dateTime) + "/" + persianDateShamsi.GetShamsiDayString(dateTime);
        }
        /// <summary>
        /// Get Long Shamsi Date From Miladi Year
        /// </summary>
        /// <param name="dateTime">Enter The Jalali DateTime</param>
        /// <returns></returns>
        public static string ToLongShamsiDate(this DateTime dateTime)
        {
            PersianDateTime persianDateShamsi = new PersianDateTime();
            return persianDateShamsi.GetShamsiDayName(dateTime) + " " + persianDateShamsi.GetShamsiDay(dateTime) + " " + persianDateShamsi.GetShamsiMonthName(dateTime) + " " + persianDateShamsi.GetShamsiYear(dateTime);
        }

        public static string GetDiffrenceToNow(this DateTime dt)
        {
            var Current = DateTime.Now;
            var ts = (Current - dt);
            string opr = "پیش";
            if (dt > DateTime.Now)
            {
                opr = "بعد";
                ts = dt - Current;
            }
            if (ts.TotalMinutes < 1)
                return "اکنون";
            if (ts.TotalMinutes < 60)
                return $"{ts.Minutes} دقیقه {opr}";
            if (ts.TotalDays < 1)
            {
                return $"{ts.Hours} ساعت و {ts.Minutes} دقیقه {opr}";
            }
            if (ts.TotalDays < 30)
            {
                return $"{ts.Days} روز و {ts.Hours} ساعت و {ts.Minutes} دقیقه {opr}";
            }
            if (ts.TotalDays > 30 && ts.TotalDays < 365)
            {
                var months = Math.Floor(ts.TotalDays / 30);
                var days = Math.Floor(ts.TotalDays % 30);
                return (months > 0 ? $"{months} ماه و " : String.Empty) +
                       (days > 0 ? $"{days} روز و " : String.Empty) +
                       (ts.Hours > 0 ? $"{ts.Hours} ساعت و " : String.Empty) +
                       (ts.Minutes > 0 ? $"{ts.Minutes} دقیقه " : String.Empty) + opr;
            }
            if (ts.TotalDays >= 365)
            {
                var year = Math.Floor(ts.TotalDays / 365);
                var months = Math.Floor(ts.TotalDays % 365 / 30);
                var days = Math.Floor(ts.TotalDays % 365 / 30);
                return (year > 0 ? $"{year} سال و " : String.Empty) +
                       (months > 0 ? $"{months} ماه و " : String.Empty) +
                       (days > 0 ? $"{days} روز و " : String.Empty) +
                       (ts.Hours > 0 ? $"{ts.Hours} ساعت و " : String.Empty) +
                       (ts.Minutes > 0 ? $"{ts.Minutes} دقیقه " : String.Empty) + opr;
            }
            return "نامشخص";
        }
        public static string GetDiffrenceToNow(this PersianDateTime dt) =>
            GetDiffrenceToNow(dt.DateTime);

        public static string ConvertToPersianDigit(this string num)
        {
            return num.Replace("0", "۰").Replace("1", "۱").Replace("2", "۲").Replace("3", "۳").Replace("4", "۴")
                .Replace("5", "۵").Replace("6", "۶").Replace("7", "۷").Replace("8", "۸").Replace("9", "۹")
                .Replace(".", ".");
        }

        public static string ConvertToPersianChar(this string str)
        {
            return str.Replace("q", "ض").Replace("w", "ص").Replace("e", "ث").Replace("r", "ق").Replace("t", "ف")
                .Replace("y", "غ").Replace("u", "ع").Replace("i", "ه").Replace("o", "خ").Replace("p", "ح")
                .Replace("[", "ج").Replace("]", "چ").Replace("a", "ش").Replace("s", "س").Replace("d", "ی")
                .Replace("f", "ب").Replace("g", "ل").Replace("h", "ا").Replace("j", "ت").Replace("k", "ن")
                .Replace("l", "م").Replace(";", "ک").Replace("\"", "گ").Replace("z", "ظ").Replace("x", "ط")
                .Replace("c", "ز").Replace("v", "ر").Replace("b", "ذ").Replace("n", "د").Replace("m", "پ")
                .Replace(")", "و").Replace("?", "؟");
        }

        public static string ConvertToEnglishDigit(this string num)
        {
            return num.Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4")
                .Replace("۵", "5").Replace("۶", "6").Replace("۷", "7").Replace("۸", "8").Replace("۹", "9")
                .Replace(".", ".");
        }

        public static double DateDifference(this PersianDateTime d1, PersianDateTime d2)
            => (d1.DateTime - d2.DateTime).TotalDays;

        public static DateTime ToGregorianDate(this PersianDateTime persianDateTime)
        {
            GregorianCalendar gc = new GregorianCalendar();
            return new DateTime(gc.GetYear(persianDateTime.DateTime), gc.GetMonth(persianDateTime.DateTime), gc.GetDayOfMonth(persianDateTime.DateTime), gc.GetHour(persianDateTime.DateTime), gc.GetMinute(persianDateTime.DateTime), gc.GetSecond(persianDateTime.DateTime), new System.Globalization.PersianCalendar());
        }

        public static DateTime ToGregorianDate(this DateTime dateTime)
        {
            GregorianCalendar gc = new GregorianCalendar();
            return new DateTime(gc.GetYear(dateTime), gc.GetMonth(dateTime), gc.GetDayOfMonth(dateTime), gc.GetHour(dateTime), gc.GetMinute(dateTime), gc.GetSecond(dateTime), new System.Globalization.PersianCalendar());
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
