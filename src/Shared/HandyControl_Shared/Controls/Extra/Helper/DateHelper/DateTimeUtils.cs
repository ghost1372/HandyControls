using System;
using System.Linq;

namespace HandyControl.Controls
{
    /// <summary>
    /// متدهای کمکی جهت کار با تاریخ میلادی
    /// </summary>
    public static class DateTimeUtils
    {
        /// <summary>
        /// Iran Standard Time
        /// </summary>
        public static readonly TimeZoneInfo IranStandardTime;

        /// <summary>
        /// Epoch represented as DateTime
        /// </summary>
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        static DateTimeUtils()
        {
            IranStandardTime = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(timeZoneInfo =>
                timeZoneInfo.StandardName.Contains("Iran") ||
                timeZoneInfo.StandardName.Contains("Tehran") ||
                timeZoneInfo.Id.Contains("Iran") ||
                timeZoneInfo.Id.Contains("Tehran"));
            if (IranStandardTime == null)
            {
#if NET40 || NET45 || NET46
                throw new PlatformNotSupportedException($"This OS[{Environment.OSVersion.Platform}, {Environment.OSVersion.Version}] doesn't support IranStandardTime.");
#else
                // throw new PlatformNotSupportedException($"This OS[{System.Runtime.InteropServices.RuntimeInformation.OSDescription}] doesn't support IranStandardTime.");
#endif
            }
        }

        /// <summary>
        /// محاسبه سن
        /// </summary>
        /// <param name="birthday">تاریخ تولد</param>
        /// <param name="comparisonBase">مبنای محاسبه مانند هم اکنون</param>
        /// <param name="dateTimeOffsetPart">کدام جزء این وهله مورد استفاده قرار گیرد؟</param>
        /// <returns>سن</returns>
        public static int GetAge(this DateTimeOffset birthday, DateTime comparisonBase, DateTimeOffsetPart dateTimeOffsetPart = DateTimeOffsetPart.IranLocalDateTime)
        {
            return GetAge(birthday.GetDateTimeOffsetPart(dateTimeOffsetPart), comparisonBase);
        }

        /// <summary>
        /// محاسبه سن
        /// مبنای محاسبه هم اکنون
        /// </summary>
        /// <param name="birthday">تاریخ تولد</param>
        /// <returns>سن</returns>
        public static int GetAge(this DateTimeOffset birthday)
        {
            DateTime birthdayDateTime = birthday.UtcDateTime;
            DateTime now = DateTime.UtcNow;
            return GetAge(birthdayDateTime, now);
        }

        /// <summary>
        /// محاسبه سن
        /// </summary>
        /// <param name="birthday">تاریخ تولد</param>
        /// <param name="comparisonBase">مبنای محاسبه مانند هم اکنون</param>
        /// <returns>سن</returns>
        public static int GetAge(this DateTime birthday, DateTime comparisonBase)
        {
            DateTime now = comparisonBase;
            int age = now.Year - birthday.Year;
            if (now < birthday.AddYears(age))
            {
                age--;
            }

            return age;
        }

        /// <summary>
        /// محاسبه سن
        /// مبنای محاسبه هم اکنون
        /// </summary>
        /// <param name="birthday">تاریخ تولد</param>
        /// <returns>سن</returns>
        public static int GetAge(this DateTime birthday)
        {
            DateTime now = birthday.Kind.GetNow();
            return GetAge(birthday, now);
        }

        /// <summary>
        /// دریافت جزء زمانی ویژه‌ی این وهله
        /// </summary>
        public static DateTime GetDateTimeOffsetPart(
            this DateTimeOffset dateTimeOffset,
            DateTimeOffsetPart dataDateTimeOffsetPart)
        {
            switch (dataDateTimeOffsetPart)
            {
                case DateTimeOffsetPart.DateTime:
                    return dateTimeOffset.DateTime;

                case DateTimeOffsetPart.LocalDateTime:
                    return dateTimeOffset.LocalDateTime;

                case DateTimeOffsetPart.UtcDateTime:
                    return dateTimeOffset.UtcDateTime;

                case DateTimeOffsetPart.IranLocalDateTime:
                    return dateTimeOffset.ToIranTimeZoneDateTimeOffset().DateTime;

                default:
                    throw new ArgumentOutOfRangeException(nameof(dataDateTimeOffsetPart), dataDateTimeOffsetPart, null);
            }
        }

        /// <summary>
        /// بازگشت زمان جاری با توجه به نوع زمان
        /// </summary>
        /// <param name="dataDateTimeKind">نوع زمان ورودی</param>
        /// <returns>هم اکنون</returns>
        public static DateTime GetNow(this DateTimeKind dataDateTimeKind)
        {
            switch (dataDateTimeKind)
            {
                case DateTimeKind.Utc:
                    return DateTime.UtcNow;
                default:
                    return DateTime.Now;
            }
        }

        /// <summary>
        /// تبدیل منطقه زمانی این وهله به منطقه زمانی ایران
        /// </summary>
        public static DateTimeOffset ToIranTimeZoneDateTimeOffset(this DateTimeOffset dateTimeOffset)
        {
            return TimeZoneInfo.ConvertTime(dateTimeOffset, IranStandardTime);
        }

        /// <summary>
        /// تبدیل منطقه زمانی این وهله به منطقه زمانی ایران
        /// </summary>
        public static DateTime ToIranTimeZoneDateTime(this DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTime(dateTime, IranStandardTime);
        }

        /// <summary>
        /// Converts a given <see cref="DateTime"/> to milliseconds from Epoch.
        /// </summary>
        /// <param name="dateTime">A given <see cref="DateTime"/></param>
        /// <returns>Milliseconds since Epoch</returns>
        public static long ToEpochMilliseconds(this DateTime dateTime)
        {
            return (long)dateTime.ToUniversalTime().Subtract(Epoch).TotalMilliseconds;
        }

        /// <summary>
        /// Converts a given <see cref="DateTime"/> to seconds from Epoch.
        /// </summary>
        /// <param name="dateTime">A given <see cref="DateTime"/></param>
        /// <returns>The Unix time stamp</returns>
        public static long ToEpochSeconds(this DateTime dateTime)
        {
            return dateTime.ToEpochMilliseconds() / 1000;
        }

        /// <summary>
        /// Checks the given date is between the two provided dates
        /// </summary>
        public static bool IsBetween(this DateTime date, DateTime startDate, DateTime endDate, bool compareTime = false)
        {
            return compareTime ? date >= startDate && date <= endDate : date.Date >= startDate.Date && date.Date <= endDate.Date;
        }

        /// <summary>
        /// Returns whether the given date is the last day of the month
        /// </summary>
        public static bool IsLastDayOfTheMonth(this DateTime dateTime)
        {
            return dateTime == new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// Returns whether the given date falls in a weekend
        /// </summary>
        public static bool IsWeekend(this DateTime value)
        {
            return value.DayOfWeek == DayOfWeek.Sunday || value.DayOfWeek == DayOfWeek.Saturday;
        }

        /// <summary>
        /// Determines if a given year is a LeapYear or not.
        /// </summary>
        public static bool IsLeapYear(this DateTime value)
        {
            return DateTime.DaysInMonth(value.Year, 2) == 29;
        }

        /// <summary>
        /// Converts a DateTime to a DateTimeOffset
        /// </summary>
        /// <param name="dt">Source DateTime.</param>
        /// <param name="offset">Offset</param>
        public static DateTimeOffset ToDateTimeOffset(this DateTime dt, TimeSpan offset)
        {
            if (dt == DateTime.MinValue)
            {
                return DateTimeOffset.MinValue;
            }

            return new DateTimeOffset(dt.Ticks, offset);
        }

        /// <summary>
        /// Converts a DateTime to a DateTimeOffset
        /// </summary>
        /// <param name="dt">Source DateTime.</param>
        /// <param name="offsetInHours">Offset</param>
        public static DateTimeOffset ToDateTimeOffset(this DateTime dt, double offsetInHours = 0)
        {
            return ToDateTimeOffset(dt, offsetInHours == 0 ? TimeSpan.Zero : TimeSpan.FromHours(offsetInHours));
        }
    }
}
