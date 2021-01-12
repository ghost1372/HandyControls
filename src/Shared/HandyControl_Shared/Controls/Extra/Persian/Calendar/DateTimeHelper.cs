// (c) Copyright Microsoft Corporation.
// This source is subject to [###LICENSE_NAME###].
// Please see [###LICENSE_LINK###] for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace Microsoft.Windows.Controls
{
    // NOTICE: This date time helper assumes it is working in a Gregorian calendar
    //         If we ever support non Gregorian calendars this class would need to be redesigned
    internal static class DateTimeHelper
    {
        private static System.Globalization.Calendar cal = PersianCalendarHelper.GetCurrentCalendar();

        public static System.Globalization.Calendar Calendar
        {
            get { return cal; }
        }

        public static DateTime? AddDays(DateTime time, int days)
        {
            try
            {
                return cal.AddDays(time, days);
            }
            catch (System.ArgumentException)
            {
                return null;
            }
        }

        public static DateTime? AddMonths(DateTime time, int months)
        {
            try
            {
                return cal.AddMonths(time, months);
            }
            catch (System.ArgumentException)
            {
                return null;
            }
        }

        public static DateTime? AddYears(DateTime time, int years)
        {
            try
            {
                return cal.AddYears(time, years);
            }
            catch (System.ArgumentException)
            {
                return null;
            }
        }

        public static DateTime? SetYear(DateTime date, DateTime yearDate)
        {
            int curYear = cal.GetYear(date);
            int newYear = cal.GetYear(yearDate);
            return DateTimeHelper.AddYears(date, newYear - curYear);
        }

        public static DateTime? SetYearMonth(DateTime date, DateTime yearMonth)
        {
            DateTime? target = SetYear(date, yearMonth);
            if (target.HasValue)
            {
                int month = cal.GetMonth(date);
                int newMonth = cal.GetMonth(yearMonth);

                target = DateTimeHelper.AddMonths(target.Value, newMonth - month);
            }

            return target;
        }

        public static int CompareDays(DateTime dt1, DateTime dt2)
        {
            return DateTime.Compare(DiscardTime(dt1).Value, DiscardTime(dt2).Value);
        }

        public static int CompareYearMonth(DateTime dt1, DateTime dt2)
        {
            //return ((dt1.Year - dt2.Year) * 12) + (dt1.Month - dt2.Month);
            int year1 = cal.GetYear(dt1);
            int year2 = cal.GetYear(dt2);
            int month1 = cal.GetMonth(dt1);
            int month2 = cal.GetMonth(dt2);

            return ((year1 - year2) * 12) + (month1 - month2);
        }

        public static DateTime DecadeOfDate(DateTime date)
        {
            int year = cal.GetYear(date);
            int decade = year - (year % 10);
            DateTime newDate = cal.ToDateTime(decade, 1, 1, 0, 0, 0, 0);
            return newDate;
        }

        public static DateTime DiscardDayTime(DateTime d)
        {
            int year = cal.GetYear(d);
            int month = cal.GetMonth(d);            
            return cal.ToDateTime(year, month, 1, 0, 0, 0, 0);
        }

        public static DateTime GetFirstDayOfMonth(DateTime dt)
        {
            int year = cal.GetYear(dt);
            int month = cal.GetMonth(dt);
            return cal.ToDateTime(year, month, 1, 0, 0, 0, 0);
        }

        public static DateTime DiscardMonthDayTime(DateTime d)
        {
            int year = cal.GetYear(d);
            return cal.ToDateTime(year, 1, 1, 0, 0, 0, 0);
        }

        public static DateTime? DiscardTime(DateTime? d)
        {
            if (d == null)
            {
                return null;
            }

            return d.Value.Date;
        }

        public static DateTime GetLastMonth(DateTime d)
        {
            int year = cal.GetYear(d);
            return cal.ToDateTime(year, 12, 1, 0, 0, 0, 0);
        }

        public static DateTime EndOfDecade(DateTime date)
        {
            return cal.AddYears(DecadeOfDate(date), 9);
        }

        public static DateTimeFormatInfo GetCurrentDateFormat()
        {
            return GetDateFormat(CultureInfo.CurrentCulture);
        }

        internal static CultureInfo GetCulture(FrameworkElement element)
        {
            CultureInfo culture;
            if (DependencyPropertyHelper.GetValueSource(element, FrameworkElement.LanguageProperty).BaseValueSource != BaseValueSource.Default)
            {
                culture = GetCultureInfo(element);
            }
            else
            {
                culture = CultureInfo.CurrentCulture;
            }
            return culture;
        }

        // ------------------------------------------------------------------
        // Retrieve CultureInfo property from specified element.
        // ------------------------------------------------------------------
        internal static CultureInfo GetCultureInfo(DependencyObject element)
        {
            XmlLanguage language = (XmlLanguage)element.GetValue(FrameworkElement.LanguageProperty);
            try
            {
                return language.GetSpecificCulture();
            }
            catch (InvalidOperationException)
            {
                // We default to en-US if no part of the language tag is recognized.
                return CultureInfo.ReadOnly(new CultureInfo("en-us", false));
            }
        }

        internal static DateTimeFormatInfo GetDateFormat(CultureInfo culture)
        {
            return PersianCalendarHelper.GetDateTimeFormatInfo();
        }

        // returns if the date is included in the range
        public static bool InRange(DateTime date, CalendarDateRange range)
        {
            return InRange(date, range.Start, range.End);
        }

        // returns if the date is included in the range
        public static bool InRange(DateTime date, DateTime start, DateTime end)
        {
            Debug.Assert(DateTime.Compare(start, end) < 1);

            if (CompareDays(date, start) > -1 && CompareDays(date, end) < 1)
            {
                return true;
            }

            return false;
        }

        public static string ToDayString(DateTime? date, CultureInfo culture)
        {
            string result = string.Empty;
            DateTimeFormatInfo format = GetDateFormat(culture);

            if (date.HasValue && format != null)
            {
                result = cal.GetDayOfMonth(date.Value).ToString(format);
            }

            return result;
        }

        public static string ToDecadeRangeString(DateTime decade, CultureInfo culture)
        {
            string result = string.Empty;
            DateTimeFormatInfo format = culture.DateTimeFormat;

            if (format != null)
            {
                int decadeYear = cal.GetYear(decade);
                int decadeEndYear = decadeYear + 9;

                result = decadeYear.ToString(format) + "-" + decadeEndYear.ToString(format);
            }

            return result;
        }

        public static string ToYearMonthPatternString(DateTime? date, CultureInfo culture)
        {
            string result = string.Empty;
            DateTimeFormatInfo format = GetDateFormat(culture);

            if (date.HasValue && format != null)
            {
                //result = date.Value.ToString(format.YearMonthPattern, format);
                result = PersianCalendarHelper.ToCurrentCultureString(date.Value, format.YearMonthPattern, format);
            }

            return result;
        }

        public static string ToYearString(DateTime? date, CultureInfo culture)
        {
            string result = string.Empty;
            DateTimeFormatInfo format = GetDateFormat(culture);

            if (date.HasValue && format != null)
            {
                result = cal.GetYear(date.Value).ToString(format);
            }

            return result;
        }

        public static string ToAbbreviatedMonthString(DateTime? date, CultureInfo culture)
        {
            string result = string.Empty;
            DateTimeFormatInfo format = GetDateFormat(culture);

            if (date.HasValue && format != null)
            {
                //string[] monthNames = format.AbbreviatedMonthNames;
                //if (monthNames != null && monthNames.Length > 0)
                //{
                //    result = monthNames[(date.Value.Month - 1) % monthNames.Length];
                //}
                result = PersianCalendarHelper.ToCurrentCultureString(date.Value, "MMM", format);
            }

            return result;
        }

        public static string ToLongDateString(DateTime? date, CultureInfo culture)
        {
            string result = string.Empty;
            DateTimeFormatInfo format = GetDateFormat(culture);

            if (date.HasValue && format != null)
            {
                //result = date.Value.Date.ToString(format.LongDatePattern, format);
                result = PersianCalendarHelper.ToCurrentCultureString(date.Value.Date, format.LongDatePattern, format);
            }

            return result;
        }
    }
}
