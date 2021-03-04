using HandyControl.Controls;
using System;
using System.Globalization;
namespace HandyControl.Tools.Extension
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// Get Shamsi Date From Miladi Year
        /// </summary>
        /// <param name="dateTime">Enter The Jalali DateTime</param>
        /// <returns></returns>
        public static string ToShamsiDate(this DateTime dateTime)
        {
            PersianDateTime persianDateShamsi = new PersianDateTime();
            return persianDateShamsi.GetShamsiYearToString(dateTime) + "/" +
                   persianDateShamsi.GetShamsiMonthString(dateTime) + "/" +
                   persianDateShamsi.GetShamsiDayString(dateTime);
        }

        /// <summary>
        /// Get Short Shamsi Date From Miladi Year
        /// </summary>
        /// <param name="dateTime">Enter The Jalali DateTime</param>
        /// <returns></returns>
        public static string ToShortShamsiDate(this DateTime dateTime)
        {
            PersianDateTime persianDateShamsi = new PersianDateTime();
            return persianDateShamsi.GetShortShamsiYear(dateTime) + "/" +
                   persianDateShamsi.GetShamsiMonthString(dateTime) + "/" +
                   persianDateShamsi.GetShamsiDayString(dateTime);
        }

        /// <summary>
        /// Get Long Shamsi Date From Miladi Year
        /// </summary>
        /// <param name="dateTime">Enter The Jalali DateTime</param>
        /// <returns></returns>
        public static string ToLongShamsiDate(this DateTime dateTime)
        {
            PersianDateTime persianDateShamsi = new PersianDateTime();
            return persianDateShamsi.GetShamsiDayName(dateTime) + " " + persianDateShamsi.GetShamsiDay(dateTime) + " " +
                   persianDateShamsi.GetShamsiMonthName(dateTime) + " " + persianDateShamsi.GetShamsiYear(dateTime);
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
            return new DateTime(gc.GetYear(persianDateTime.DateTime), gc.GetMonth(persianDateTime.DateTime),
                gc.GetDayOfMonth(persianDateTime.DateTime), gc.GetHour(persianDateTime.DateTime),
                gc.GetMinute(persianDateTime.DateTime), gc.GetSecond(persianDateTime.DateTime),
                new System.Globalization.PersianCalendar());
        }

        public static DateTime ToGregorianDate(this DateTime dateTime)
        {
            GregorianCalendar gc = new GregorianCalendar();
            return new DateTime(gc.GetYear(dateTime), gc.GetMonth(dateTime), gc.GetDayOfMonth(dateTime),
                gc.GetHour(dateTime), gc.GetMinute(dateTime), gc.GetSecond(dateTime),
                new System.Globalization.PersianCalendar());
        }
    }
}
