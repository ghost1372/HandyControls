using System;
using System.Collections.Generic;
using System.Text;

namespace HandyControl.Controls
{
    public static class ToShamsi
    {
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
    }
}
