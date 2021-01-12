using System;

namespace Microsoft.Windows.Controls
{
    internal static class PersianCalendarHelper
    {
        public static System.Globalization.Calendar GetCurrentCalendar()
        {
            return new System.Globalization.PersianCalendar();
        }

        // TODO: Need more work to accept (") and (') and (%) and (\) characters.
        public static string ToCurrentCultureString(DateTime dt, string format, System.Globalization.DateTimeFormatInfo formatProvider)
        {
            // Some formats do not need to custom implementation like these
            string[] autoReplaces = new string[] {
                "fffffff", "ffffff", "fffff", "ffff", "fff", "ff", "f",
                "FFFFFFF", "FFFFFF", "FFFFF", "FFFF", "FFF", "FF", "F",
                "gg", "g",
                "hh", "HH", "mm", "ss", "tt", "t"
            };

            var cal = GetCurrentCalendar();
            int year = cal.GetYear(dt);
            int month = cal.GetMonth(dt);
            int day = cal.GetDayOfMonth(dt);

            DayOfWeek dayOfWeek = cal.GetDayOfWeek(dt);

            foreach (var autoReplace in autoReplaces)
            {
                format = format.Replace(autoReplace, dt.ToString(autoReplace, formatProvider));
            }

            format = format.Replace("dddd", formatProvider.GetDayName(dayOfWeek));
            format = format.Replace("ddd", formatProvider.GetAbbreviatedDayName(dayOfWeek));
            format = format.Replace("dd", ((int)dayOfWeek).ToString("00"));
            format = format.Replace("dd", dayOfWeek.ToString());
            format = format.Replace("MMMM", formatProvider.GetMonthName(month));
            format = format.Replace("MMM", formatProvider.GetAbbreviatedMonthName(month));
            format = format.Replace("MM", month.ToString("00"));
            format = format.Replace("M", month.ToString());
            format = format.Replace("yyyy", year.ToString("0000"));
            format = format.Replace("yyy", year.ToString("000"));
            format = format.Replace("yy", (year % 100).ToString("00"));
            format = format.Replace("y", (year % 100).ToString());

            return format;
        }

        public static System.Globalization.DateTimeFormatInfo GetDateTimeFormatInfo()
        {
            System.Globalization.DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo()
            {
                AbbreviatedDayNames = new string[] { "ی", "د", "س", "چ", "پ", "ج", "ش" },
                AbbreviatedMonthGenitiveNames = new string[] { 
                        "فروردین", "اردیبهشت", "خرداد",
                        "تیر", "مرداد", "شهریور",
                        "مهر", "آبان", "آذر",
                        "دی", "بهمن", "اسفند", "" },
                AbbreviatedMonthNames = new string[] {
                        "فروردین", "اردیبهشت", "خرداد",
                        "تیر", "مرداد", "شهریور",
                        "مهر", "آبان", "آذر",
                        "دی", "بهمن", "اسفند", "" },
                AMDesignator = "صبح",
                CalendarWeekRule = System.Globalization.CalendarWeekRule.FirstDay,
                DateSeparator = "/",
                DayNames = new string[] { "یکشنبه", "دوشنبه", "سه‌شنبه", "چهار‌شنبه", "پنجشنبه", "جمعه", "شنبه" },
                FirstDayOfWeek = DayOfWeek.Saturday,
                FullDateTimePattern = "dddd dd MMMM yyyy",
                LongDatePattern = "dd MMMM yyyy",
                LongTimePattern = "hh:mm:ss TT",
                MonthDayPattern = "dd MMMM",
                MonthGenitiveNames = new string[] { 
                        "فروردین", "اردیبهشت", "خرداد",
                        "تیر", "مرداد", "شهریور",
                        "مهر", "آبان", "آذر",
                        "دی", "بهمن", "اسفند", "" },
                MonthNames = new string[] {
                        "فروردین", "اردیبهشت", "خرداد",
                        "تیر", "مرداد", "شهریور",
                        "مهر", "آبان", "آذر",
                        "دی", "بهمن", "اسفند", "" },
                PMDesignator = "عصر",
                ShortDatePattern = "dd/MM/yy",
                ShortestDayNames = new string[] { "ی", "د", "س", "چ", "پ", "ج", "ش" },
                ShortTimePattern = "HH:mm",
                TimeSeparator = ":",
                YearMonthPattern = "MMMM yyyy",
            };

            return dtFormat;
        }
    }
}
