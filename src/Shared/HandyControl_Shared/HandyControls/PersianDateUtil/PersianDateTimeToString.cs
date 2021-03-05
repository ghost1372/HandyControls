using System;
using System.Globalization;

namespace HandyControl.Tools
{
    public partial class PersianDateTime
    {
        #region Year
        /// <summary>
        /// Get Shamsi Year From Miladi Year
        /// </summary>
        /// <param name="dateTime">Enter The Jalali DateTime</param>
        /// <returns></returns>
        public int GetShamsiYear(DateTime dateTime)
        {
            return persianCalendar.GetYear(dateTime);
        }
        /// <summary>
        /// Get Short Shamsi Year From Miladi Year In String
        /// </summary>
        /// <param name="dateTime">Enter The Jalali DateTime</param>
        /// <returns></returns>
        public string GetShortShamsiYear(DateTime dateTime)
        {
            return dateTime.ToString("yy", CultureInfo.CreateSpecificCulture("fa"));
        }
        /// <summary>
        /// Get Shamsi Year From Miladi Year In String
        /// </summary>
        /// <param name="dateTime">Enter The Jalali DateTime</param>
        /// <returns></returns>
        public string GetShamsiYearToString(DateTime dateTime)
        {
            return persianCalendar.GetYear(dateTime).ToString();
        }
        #endregion

        #region Month
        /// <summary>
        /// Get Shamsi Month From Miladi Month
        /// </summary>
        /// <param name="dateTime">Enter The Jalali DateTime</param>
        /// <returns></returns>
        public int GetShamsiMonth(DateTime dateTime)
        {
            return persianCalendar.GetMonth(dateTime);
        }
        /// <summary>
        /// Get Shamsi Month Number From Miladi Month In String
        /// </summary>
        /// <param name="dateTime">Enter The Jalali DateTime</param>
        /// <returns></returns>
        public string GetShamsiMonthString(DateTime dateTime)
        {
            return persianCalendar.GetMonth(dateTime).ToString("00");
        }
        /// <summary>
        /// Get Shamsi Month From Miladi Month Number
        /// </summary>
        /// <param name="dateTime">Enter The Jalali DateTime</param>
        /// <returns></returns>
        public int GetShamsiMonthBunber(DateTime dateTime)
        {
            return persianCalendar.GetMonth(dateTime);
        }
        /// <summary>
        /// Get Shamsi Month Name From Miladi Month
        /// </summary>
        /// <param name="dateTime">Enter The Jalali DateTime</param>
        /// <returns></returns>
        public string GetShamsiMonthName(DateTime dateTime)
        {
            return dateTime.ToString("MMMM", CultureInfo.CreateSpecificCulture("fa"));
        }
        #endregion

        #region Day
        /// <summary>
        /// Get Shamsi Day From Miladi Month
        /// </summary>
        /// <param name="dateTime">Enter The Jalali DateTime</param>
        /// <returns></returns>
        public int GetShamsiDay(DateTime dateTime)
        {
            return persianCalendar.GetDayOfMonth(dateTime);
        }
        /// <summary>
        /// Get Shamsi Day From Miladi Month In String
        /// </summary>
        /// <param name="dateTime">Enter The Jalali DateTime</param>
        /// <returns></returns>
        public string GetShamsiDayString(DateTime dateTime)
        {
            return persianCalendar.GetDayOfMonth(dateTime).ToString("00");
        }
        /// <summary>
        /// Get Shamsi Day Name From Miladi Month
        /// </summary>
        /// <param name="dateTime">Enter The Jalali DateTime</param>
        /// <returns></returns>
        public string GetShamsiDayName(DateTime dateTime)
        {
            return dateTime.ToString("dddd", CultureInfo.CreateSpecificCulture("fa"));
        }
        /// <summary>
        /// Get Shamsi Day ShortName From Miladi Month
        /// </summary>
        /// <param name="dateTime">Enter The Jalali DateTime</param>
        /// <returns></returns>
        public string GetShamsiDayShortName(DateTime dateTime)
        {
            return dateTime.ToString("dddd", CultureInfo.CreateSpecificCulture("fa")).Substring(0, 1);
        }
        #endregion

        /// <summary>
        ///     شنبه بیست آذر سال یکهزار سیصد و نود وهفت ساعت هفت و سی دقیقه و بیست ثانیه
        /// </summary>
        /// <returns></returns>
        public string ToLongStringYMDHMS()
        {
            return
                $"{DayOfWeek} {PersianUtil.Convert(Day)} {months[Month]} سال  {PersianUtil.Convert(Year)} ساعت {PersianUtil.Convert(Hour)} و {PersianUtil.Convert(Minute)} دقیقه و {PersianUtil.Convert(Second)} ثانیه";
        }

        /// <summary>
        ///     شنبه بیست آذر سال یکهزار سیصد و نود وهفت ساعت هفت و سی دقیقه
        /// </summary>
        /// <returns></returns>
        public string ToLongStringYMDHM()
        {
            return
                $"{DayOfWeek} {PersianUtil.Convert(Day)} {months[Month]} سال  {PersianUtil.Convert(Year)} ساعت {PersianUtil.Convert(Hour)} و {PersianUtil.Convert(Minute)} دقیقه";
        }

        /// <summary>
        ///     شنبه بیست آذر سال یکهزار سیصد و نود وهفت
        /// </summary>
        /// <returns></returns>
        public string ToLongStringYMD()
        {
            return
                $"{DayOfWeek} {PersianUtil.Convert(Day)} {months[Month]} سال  {PersianUtil.Convert(Year)}";
        }

        /// <summary>
        ///     ساعت پانزده و سی دقیقه و ده ثانیه
        /// </summary>
        /// <returns></returns>
        public string ToLongStringHMS()
        {
            return
                $"ساعت {PersianUtil.Convert(Hour)} و {PersianUtil.Convert(Minute)} دقیقه و {PersianUtil.Convert(Second)} ثانیه";
        }

        /// <summary>
        ///     ساعت پانزده و سی دقیقه
        /// </summary>
        /// <returns></returns>
        public string ToLongStringHM()
        {
            return $"ساعت {PersianUtil.Convert(Hour)} و {PersianUtil.Convert(Minute)} دقیقه";
        }

        public override string ToString()
        {
            return $"{Year}/{Month.ToString("00")}/{Day.ToString("00")}";
        }
    }
}
