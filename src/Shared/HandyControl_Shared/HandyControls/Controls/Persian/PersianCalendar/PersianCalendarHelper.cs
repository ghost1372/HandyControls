using System;
using System.Globalization;

namespace Microsoft.Windows.Controls;

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

    public static DateTimeFormatInfo GetDateTimeFormatInfo()
    {
        var info = CultureInfo.CurrentCulture;
        DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
        dtFormat.CalendarWeekRule = CalendarWeekRule.FirstDay;
        dtFormat.DateSeparator = "/";
        dtFormat.FirstDayOfWeek = DayOfWeek.Saturday;
        dtFormat.FullDateTimePattern = "dddd dd MMMM yyyy";
        dtFormat.LongDatePattern = "dd MMMM yyyy";
        dtFormat.LongTimePattern = "hh:mm:ss TT";
        dtFormat.MonthDayPattern = "dd MMMM";
        dtFormat.ShortDatePattern = "dd/MM/yy";
        dtFormat.ShortTimePattern = "HH:mm";
        dtFormat.TimeSeparator = ":";
        dtFormat.YearMonthPattern = "MMMM yyyy";
        dtFormat.AMDesignator = "صبح";
        dtFormat.PMDesignator = "عصر";
        dtFormat.DayNames = new string[] { "یکشنبه", "دوشنبه", "سه‌شنبه", "چهار‌شنبه", "پنجشنبه", "جمعه", "شنبه" };
        dtFormat.AbbreviatedDayNames = new string[] { "ی", "د", "س", "چ", "پ", "ج", "ش" };
        dtFormat.ShortestDayNames = new string[] { "ی", "د", "س", "چ", "پ", "ج", "ش" };

        if (info.Name == "fa-AF" || info.Name == "ps-AF" || info.Name == "prs-AF")
        {
            dtFormat.AbbreviatedMonthGenitiveNames = new string[] {
                    "حمل", "ثور", "جوزا",
                    "سرطان", "اسد", "سنبله",
                    "میزان", "عقرب", "قوس",
                    "جدی", "دلو", "حوت", "" };
            dtFormat.AbbreviatedMonthNames = new string[] {
                    "حمل", "ثور", "جوزا",
                    "سرطان", "اسد", "سنبله",
                    "میزان", "عقرب", "قوس",
                    "جدی", "دلو", "حوت", "" }; 
            dtFormat.MonthGenitiveNames = new string[] {
                    "حمل", "ثور", "جوزا",
                    "سرطان", "اسد", "سنبله",
                    "میزان", "عقرب", "قوس",
                    "جدی", "دلو", "حوت", "" };
            dtFormat.MonthNames = new string[] {
                    "حمل", "ثور", "جوزا",
                    "سرطان", "اسد", "سنبله",
                    "میزان", "عقرب", "قوس",
                    "جدی", "دلو", "حوت", "" };
            return dtFormat;
        }
        else
        {
            dtFormat.AbbreviatedMonthGenitiveNames = new string[] {
                    "فروردین", "اردیبهشت", "خرداد",
                    "تیر", "مرداد", "شهریور",
                    "مهر", "آبان", "آذر",
                    "دی", "بهمن", "اسفند", "" };
            dtFormat.AbbreviatedMonthNames = new string[] {
                    "فروردین", "اردیبهشت", "خرداد",
                    "تیر", "مرداد", "شهریور",
                    "مهر", "آبان", "آذر",
                    "دی", "بهمن", "اسفند", "" };
            dtFormat.MonthGenitiveNames = new string[] {
                    "فروردین", "اردیبهشت", "خرداد",
                    "تیر", "مرداد", "شهریور",
                    "مهر", "آبان", "آذر",
                    "دی", "بهمن", "اسفند", "" };
            dtFormat.MonthNames = new string[] {
                    "فروردین", "اردیبهشت", "خرداد",
                    "تیر", "مرداد", "شهریور",
                    "مهر", "آبان", "آذر",
                    "دی", "بهمن", "اسفند", "" };
            return dtFormat;
        }
    }
}
