using System;
using System.Collections.Generic;
using System.Globalization;

namespace HandyControl.Controls
{
    /// <summary>
    /// کلاسی برای محاسبات تاریخ قمری، شمسی، میلادی
    /// </summary>
    public static class DateHelper
    {
        private static readonly IDictionary<int, long[]> _yearsMonthsInJd = new Dictionary<int, long[]>();
        private static readonly int _supportedYearsStart;
        private static readonly long[] _yearsStartJd;
        private static readonly long _jdSupportEnd;
        private const long JdSupportStart = 2453766;
        private const int NMonths = 1405 * 12 + 1;

        /// <summary>
        /// ایده گرفته شده از: https://github.com/ebraminio/DroidPersianCalendar
        /// </summary>
        static DateHelper()
        {
            // https://github.com/ilius/starcal/blob/master/scal3/cal_types/hijri-monthes.json
            int[] hijriMonths =
            {
                1427, 30, 29, 29, 30, 29, 30, 30, 30, 30, 29, 29, 30,
                1428, 29, 30, 29, 29, 29, 30, 30, 29, 30, 30, 30, 29,
                1429, 30, 29, 30, 29, 29, 29, 30, 30, 29, 30, 30, 29,
                1430, 30, 30, 29, 29, 30, 29, 30, 29, 29, 30, 30, 29,
                1431, 30, 30, 29, 30, 29, 30, 29, 30, 29, 29, 30, 29,
                1432, 30, 30, 29, 30, 30, 30, 29, 29, 30, 29, 30, 29,
                1433, 29, 30, 29, 30, 30, 30, 29, 30, 29, 30, 29, 30,
                1434, 29, 29, 30, 29, 30, 30, 29, 30, 30, 29, 30, 29,
                1435, 29, 30, 29, 30, 29, 30, 29, 30, 30, 30, 29, 30,
                1436, 29, 30, 29, 29, 30, 29, 30, 29, 30, 29, 30, 30,
                1437, 29, 30, 30, 29, 30, 29, 29, 30, 29, 29, 30, 30,
                1438, 29, 30, 30, 30, 29, 30, 29, 29, 30, 29, 29, 30,
                1439, 29, 30, 30, 30, 30, 29, 30, 29, 29, 30, 29, 29,
                1440, 30, 29, 30, 30, 30, 29, 29, 30, 29, 30, 29, 29,
                1441, 30, 29, 30, 29, 30, 30, 29, 30, 30, 29, 30, 29,
                1442, 29, 30, 29, 30, 29, 30, 29, 30, 30, 29, 30, 29,
                1443, 30, 29, 30, 29, 30, 29, 30, 29, 30, 29, 30, 30,
                1444, 29, 30, 29, 30, 30, 29, 29, 30, 29, 30, 29, 30,
                1445, 29, 30, 30, 30, 29, 30, 29, 29, 30, 29, 29, 30,
                1446, 29, 30, 30, 30, 29, 30, 30, 29, 29, 30, 29, 29,
                1447, 30, 29, 30, 30, 30, 29, 30, 29, 30, 29, 30, 29,
                1448, 29, 30, 29, 30, 30, 29, 30, 30, 29, 30, 29, 30,
                1449, 29, 29, 30, 29, 30, 29, 30, 30, 29, 30, 30, 29,
                1450, 30, 29, 30, 29, 29, 30, 29, 30, 29, 30, 30, 29,
                1451, 30, 30, 29, 30, 29, 29, 30, 29, 30, 29, 30, 29,
                1452, 30, 30, 30, 29, 30, 29, 29, 30, 29, 30, 29, 30,
                1453, 29, 30, 30, 30, 29, 29, 30, 29, 30, 29, 30, 29,
                1454, 29, 30, 30, 30, 29, 30, 29, 30, 29, 30, 29, 30,
                1455, 29, 29, 30, 30, 29, 30, 29, 30, 30, 29, 30, 29,
                1456, 30, 29, 29, 30, 29, 30, 29, 30, 30, 30, 29, 30,
                1457, 29, 30, 29, 29, 30, 29, 29, 30, 30, 29, 30, 30,
                1458, 30, 29, 30, 29, 29, 30, 29, 29, 30, 30, 29, 30,
                1459, 30, 30, 29, 30, 29, 29, 30, 29, 29, 30, 30, 29,
                1460, 30, 30, 29, 30, 29, 30, 29, 30, 29, 29, 30, 30,
                1461, 29, 30, 29, 30, 30, 29, 30, 29, 30, 29, 30, 29,
                1462, 30, 29, 30, 29, 30, 29, 30, 29, 30, 30, 29, 30,
                1463, 29, 30, 29, 29, 30, 29, 30, 30, 29, 30, 30, 29,
                1464, 30, 29, 30, 29, 29, 30, 29, 30, 29, 30, 30, 30,
                1465, 29, 30, 29, 30, 29, 29, 30, 29, 29, 30, 30, 30,
                1466, 30, 29, 30, 29, 30, 29, 29, 30, 29, 30, 29, 30,
                1467, 30, 29, 30, 30, 29, 30, 29, 29, 30, 29, 30, 29,
                1468, 30, 29, 30, 30, 29, 30, 29, 30, 29, 30, 29, 30,
                1469, 29, 29, 30, 30, 29, 30, 30, 29, 30, 30, 29, 29,
                1470, 30, 29, 29, 30, 30, 29, 30, 29, 30, 30, 30, 29,
                1471, 29, 30, 29, 29, 30, 29, 30, 30, 29, 30, 30, 29,
                1472, 30, 29, 30, 29, 30, 29, 29, 30, 29, 30, 30, 29,
                1473, 30, 29, 30, 30, 29, 30, 29, 29, 30, 29, 30, 29,
                1474, 30, 30, 29, 30, 30, 29, 30, 29, 29, 30, 29, 30,
                1475, 29, 30, 29, 30, 30, 30, 29, 30, 29, 29, 30, 29,
                1476, 29, 30, 29, 30, 30, 30, 29, 30, 30, 29, 29, 30,
                1477, 29, 29, 30, 29, 30, 30, 29, 30, 30, 30, 29, 29,
                1478, 30, 29, 29, 30, 29, 30, 30, 29, 30, 30, 29, 30,
                1479, 29, 30, 29, 29, 30, 29, 30, 29, 30, 30, 29, 30,
                1480, 29, 30, 30, 29, 29, 30, 29, 30, 29, 30, 29, 30,
                1481, 29, 30, 30, 29, 30, 30, 29, 30, 29, 29, 30, 29,
                1482, 30, 29, 30, 30, 29, 30, 30, 29, 30, 29, 29, 30,
                1483, 29, 29, 30, 30, 29, 30, 30, 30, 29, 30, 29, 29,
                1484, 30, 29, 29, 30, 30, 29, 30, 30, 29, 30, 30, 29,
                1485, 29, 30, 29, 29, 30, 30, 29, 30, 29, 30, 30, 30,
                1486, 29, 29, 30, 29, 30, 29, 30, 29, 30, 29, 30, 30,
                1487, 29, 30, 29, 30, 29, 30, 29, 29, 30, 29, 30, 30,
                1488, 29, 30, 30, 29, 30, 29, 30, 29, 29, 30, 29, 30,
                1489, 29, 30, 30, 30, 29, 30, 29, 30, 29, 29, 30, 29,
                1490, 30, 29, 30, 30, 29, 30, 30, 29, 30, 29, 29, 30,
                1491, 29, 30, 29, 30, 29, 30, 30, 29, 30, 29, 30, 30
            };

            int years = (int)Math.Ceiling(((float)hijriMonths.Length) / 13);
            _yearsStartJd = new long[years];
            _supportedYearsStart = hijriMonths[0];
            long jd = JdSupportStart;
            for (int y = 0; y < years; ++y)
            {
                int year = hijriMonths[y * 13];

                _yearsStartJd[y] = jd;
                long[] months = new long[12];
                for (int m = 1; m < 13 && y * 13 + m < hijriMonths.Length; ++m)
                {
                    months[m - 1] = jd;
                    jd += hijriMonths[y * 13 + m];
                }
                _yearsMonthsInJd.Add(year, months);
            }
            _jdSupportEnd = jd;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static long HijriToJd(int year, int month, int day)
        {
            if (_yearsMonthsInJd == null)
            {
                throw new InvalidOperationException("yearsMonthsInJd is null.");
            }

            if (!_yearsMonthsInJd.TryGetValue(year, out long[] months))
            {
                return -1;
            }

            long calculatedDay = months[month - 1];
            if (calculatedDay == 0)
            {
                return -1;
            }

            return calculatedDay + day;
        }

        private static int search(long[] array, long r)
        {
            int i = 0;
            while (i < array.Length && array[i] < r)
            {
                ++i;
            }

            return i;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="jd"></param>
        /// <returns></returns>
        public static int[] JdToHijri(long jd)
        {
            if (jd < JdSupportStart || jd >= _jdSupportEnd || _yearsStartJd == null)
            {
                return null;
            }

            int yearIndex = search(_yearsStartJd, jd);
            int year = yearIndex + _supportedYearsStart - 1;
            long[] yearMonths = _yearsMonthsInJd[year];
            if (yearMonths == null)
            {
                return null;
            }
            int month = search(yearMonths, jd);
            if (yearMonths[month - 1] == 0)
            {
                return null;
            }
            int day = (int)(jd - yearMonths[month - 1]);
            return new[] { year, month, day };
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="year">سال قمری</param>
        /// <param name="month">ماه قمری</param>
        /// <param name="day">روز قمری</param>
        /// <returns></returns>
        public static PersianDay IslamicDayToPersianDay(this int year, int month, int day)
        {
            return JdnToPersianDay(IslamicDayToJdn(year, month, day));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="jdn"></param>
        /// <returns></returns>
        public static PersianDay JdnToPersianDay(long jdn)
        {
            long depoch = jdn - PersianDayToJdn(475, 1, 1);
            long cycle = depoch / 1029983;
            long cyear = depoch % 1029983;
            long ycycle;
            long aux1, aux2;

            if (cyear == 1029982)
            {
                ycycle = 2820;
            }
            else
            {
                aux1 = cyear / 366;
                aux2 = cyear % 366;
                ycycle = (long)Math.Floor(((2134 * aux1) + (2816 * aux2) + 2815) / 1028522d)
                        + aux1 + 1;
            }

            int year, month, day;
            year = (int)(ycycle + (2820 * cycle) + 474);
            if (year <= 0)
            {
                year = year - 1;
            }

            long yday = (jdn - PersianDayToJdn(year, 1, 1)) + 1;
            if (yday <= 186)
            {
                month = (int)Math.Ceiling(yday / 31d);
            }
            else
            {
                month = (int)Math.Ceiling((yday - 6) / 30d);
            }

            day = (int)(jdn - PersianDayToJdn(year, month, 1)) + 1;
            return new PersianDay(year, month, day);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static long PersianDayToJdn(int year, int month, int day)
        {
            const long PERSIAN_EPOCH = 1948321; // The JDN of 1 Farvardin 1

            long epbase;
            if (year >= 0)
            {
                epbase = year - 474;
            }
            else
            {
                epbase = year - 473;
            }

            long epyear = 474 + (epbase % 2820);

            long mdays;
            if (month <= 7)
            {
                mdays = (month - 1) * 31;
            }
            else
            {
                mdays = (month - 1) * 30 + 6;
            }

            return day + mdays + ((epyear * 682) - 110) / 2816 + (epyear - 1) * 365
                    + epbase / 2820 * 1029983 + (PERSIAN_EPOCH - 1);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="year">سال قمری</param>
        /// <param name="month">ماه قمری</param>
        /// <param name="day">روز قمری</param>
        /// <returns></returns>
        public static long IslamicDayToJdn(int year, int month, int day)
        {
            long tableResult = HijriToJd(year, month, day);

            if (tableResult != -1)
            {
                return tableResult;
            }

            // NMONTH is the number of months between julian day number 1 and
            // the year 1405 A.H. which started immediatly after lunar
            // conjunction number 1048 which occured on September 1984 25d
            // 3h 10m UT.

            if (year < 0)
            {
                year++;
            }

            long k = month + year * 12 - NMonths; // nunber of months since 1/1/1405

            return (long)Math.Floor(visibility(k + 1048) + day + 0.5);
        }

        private static double visibility(long n)
        {
            // parameters for Makkah: for a new moon to be visible after sunset on
            // a the same day in which it started, it has to have started before
            // (SUNSET-MINAGE)-TIMZ=3 A.M. local time.
            const float TIMZ = 3f, MINAGE = 13.5f, SUNSET = 19.5f, // approximate
                    TIMDIF = (SUNSET - MINAGE);

            double jd = toMoonPhase(n, 0);
            long d = (long)Math.Floor(jd);

            double tf = (jd - d);

            if (tf <= 0.5) // new moon starts in the afternoon
            {
                return (jd + 1f);
            }
            else
            { // new moon starts before noon
                tf = (tf - 0.5) * 24 + TIMZ; // local time
                if (tf > TIMDIF)
                {
                    return (jd + 1d); // age at sunset < min for visiblity
                }
                else
                {
                    return jd;
                }
            }
        }

        private static double toMoonPhase(long n, int nph)
        {
            const double RPD = (1.74532925199433E-02); // radians per degree
                                                       // (pi/180)

            double xtra;

            double k = n + nph / 4d;
            double T = k / 1236.85;
            double t2 = T * T;
            double t3 = t2 * T;
            double jd = 2415020.75933 + 29.53058868 * k - 0.0001178 * t2
                    - 0.000000155 * t3 + 0.00033
                    * Math.Sin(RPD * (166.56 + 132.87 * T - 0.009173 * t2));

            // Sun's mean anomaly
            double sa = RPD
                    * (359.2242 + 29.10535608 * k - 0.0000333 * t2 - 0.00000347 * t3);

            // Moon's mean anomaly
            double ma = RPD
                    * (306.0253 + 385.81691806 * k + 0.0107306 * t2 + 0.00001236 * t3);

            // Moon's argument of latitude
            double tf = RPD
                    * 2d
                    * (21.2964 + 390.67050646 * k - 0.0016528 * t2 - 0.00000239 * t3);

            // should reduce to interval 0-1.0 before calculating further
            switch (nph)
            {
                case 0:
                case 2:
                    xtra = (0.1734 - 0.000393 * T) * Math.Sin(sa) + 0.0021
                            * Math.Sin(sa * 2) - 0.4068 * Math.Sin(ma) + 0.0161
                            * Math.Sin(2 * ma) - 0.0004 * Math.Sin(3 * ma) + 0.0104
                            * Math.Sin(tf) - 0.0051 * Math.Sin(sa + ma) - 0.0074
                            * Math.Sin(sa - ma) + 0.0004 * Math.Sin(tf + sa) - 0.0004
                            * Math.Sin(tf - sa) - 0.0006 * Math.Sin(tf + ma) + 0.001
                            * Math.Sin(tf - ma) + 0.0005 * Math.Sin(sa + 2 * ma);
                    break;
                case 1:
                case 3:
                    xtra = (0.1721 - 0.0004 * T) * Math.Sin(sa) + 0.0021
                            * Math.Sin(sa * 2) - 0.628 * Math.Sin(ma) + 0.0089
                            * Math.Sin(2 * ma) - 0.0004 * Math.Sin(3 * ma) + 0.0079
                            * Math.Sin(tf) - 0.0119 * Math.Sin(sa + ma) - 0.0047
                            * Math.Sin(sa - ma) + 0.0003 * Math.Sin(tf + sa) - 0.0004
                            * Math.Sin(tf - sa) - 0.0006 * Math.Sin(tf + ma) + 0.0021
                            * Math.Sin(tf - ma) + 0.0003 * Math.Sin(sa + 2 * ma)
                            + 0.0004 * Math.Sin(sa - 2 * ma) - 0.0003
                            * Math.Sin(2 * sa + ma);
                    if (nph == 1)
                    {
                        xtra = xtra + 0.0028 - 0.0004 * Math.Cos(sa) + 0.0003
                                * Math.Cos(ma);
                    }
                    else
                    {
                        xtra = xtra - 0.0028 + 0.0004 * Math.Cos(sa) - 0.0003
                                * Math.Cos(ma);
                    }

                    break;
                default:
                    return 0;
            }
            // convert from Ephemeris Time (ET) to (approximate)Universal Time (UT)
            return jd + xtra - (0.41 + 1.2053 * T + 0.4992 * t2) / 1440;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="jd"></param>
        /// <returns></returns>
        public static IslamicDay JdnToIslamicDay(long jd)
        {
            int[] tableResult = JdToHijri(jd);
            if (tableResult != null)
            {
                return new IslamicDay(tableResult[0], tableResult[1], tableResult[2]);
            }

            DateTime gregorian = JdnToGregorianDateTime(jd);
            int year = gregorian.Year;
            int month = gregorian.Month;
            int day = gregorian.Day;

            long k = (long)Math.Floor(0.6 + (year + (month % 2 == 0 ? month : month - 1) / 12d
                    + day / 365f - 1900) * 12.3685);

            double mjd;
            do
            {
                mjd = visibility(k);
                k = k - 1;
            } while (mjd > (jd - 0.5));

            k = k + 1;
            long hm = k - 1048;

            year = 1405 + (int)(hm / 12);
            month = (int)(hm % 12) + 1;

            if (hm != 0 && month <= 0)
            {
                month = month + 12;
                year = year - 1;
            }

            if (year <= 0)
            {
                year = year - 1;
            }

            day = (int)Math.Floor(jd - mjd + 0.5);

            return new IslamicDay(year, month, day);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="jdn"></param>
        /// <returns></returns>
        public static DateTime JdnToGregorianDateTime(long jdn)
        {
            if (jdn <= 2299160)
            {
                return JdnToJulian(jdn);
            }

            long l = jdn + 68569;
            long n = ((4 * l) / 146097);
            l = l - ((146097 * n + 3) / 4);
            long i = ((4000 * (l + 1)) / 1461001);
            l = l - ((1461 * i) / 4) + 31;
            long j = ((80 * l) / 2447);
            int day = (int)(l - ((2447 * j) / 80));
            l = (j / 11);
            int month = (int)(j + 2 - 12 * l);
            int year = (int)(100 * (n - 49) + i + l);
            return new DateTime(year, month, day);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="jdn"></param>
        /// <returns></returns>
        public static DateTime JdnToJulian(long jdn)
        {
            DateTime dateTime = new DateTime();
            try
            {
                long j = jdn + 1402;
                long k = ((j - 1) / 1461);
                long l = j - 1461 * k;
                long n = ((l - 1) / 365) - (l / 1461);
                long i = l - 365 * n + 30;
                j = ((80 * i) / 2447);
                int day = (int)(i - ((2447 * j) / 80));
                i = (j / 11);
                int month = (int)(j + 2 - 12 * i);
                int year = (int)(4 * k + n + i - 4716);

                dateTime = new DateTime(year, month, day);
            }
            catch
            {

            }

            return dateTime;
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به قمری
        /// </summary>
        /// <param name="gregorian">روز میلادی</param>
        /// <returns></returns>
        public static IslamicDay ToIslamicDay(this DateTime gregorian)
        {
            return JdnToIslamicDay(ToJdn(gregorian));
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به قمری
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dateTimeOffsetPart"></param>
        /// <returns></returns>
        public static IslamicDay ToIslamicDay(this DateTimeOffset dt, DateTimeOffsetPart dateTimeOffsetPart = DateTimeOffsetPart.IranLocalDateTime)
        {
            return ToIslamicDay(dt.GetDateTimeOffsetPart(dateTimeOffsetPart));
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به قمری
        /// </summary>
        /// <param name="year">سال میلادی</param>
        /// <param name="month">ماه میلادی</param>
        /// <param name="day">روز میلادی</param>
        /// <returns></returns>
        public static IslamicDay GregorianToIslamicDay(int year, int month, int day)
        {
            return JdnToIslamicDay(GregorianToJdn(year, month, day));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="gregorian"></param>
        /// <returns></returns>
        public static long ToJdn(this DateTime gregorian)
        {
            return GregorianToJdn(gregorian.Year, gregorian.Month, gregorian.Day);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="lYear"></param>
        /// <param name="lMonth"></param>
        /// <param name="lDay"></param>
        /// <returns></returns>
        public static long GregorianToJdn(long lYear, long lMonth, long lDay)
        {
            if ((lYear > 1582)
                    || ((lYear == 1582) && (lMonth > 10))
                    || ((lYear == 1582) && (lMonth == 10) && (lDay > 14)))
            {

                return ((1461 * (lYear + 4800 + ((lMonth - 14) / 12))) / 4)
                        + ((367 * (lMonth - 2 - 12 * (((lMonth - 14) / 12)))) / 12)
                        - ((3 * (((lYear + 4900 + ((lMonth - 14) / 12)) / 100))) / 4)
                        + lDay - 32075;
            }

            return JulianToJdn(lYear, lMonth, lDay);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="lYear"></param>
        /// <param name="lMonth"></param>
        /// <param name="lDay"></param>
        /// <returns></returns>
        public static long JulianToJdn(long lYear, long lMonth, long lDay)
        {
            return 367 * lYear - ((7 * (lYear + 5001 + ((lMonth - 9) / 7))) / 4)
                    + ((275 * lMonth) / 9) + lDay + 1729777;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="gregorian"></param>
        /// <returns></returns>
        public static PersianDay ToPersianDay(this DateTime gregorian)
        {
            return JdnToPersianDay(ToJdn(gregorian));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="islamic"></param>
        /// <returns></returns>
        public static DateTime IslamicDayToGregorian(this IslamicDay islamic)
        {
            return JdnToGregorianDateTime(IslamicDayToJdn(islamic));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="islamic"></param>
        /// <returns></returns>
        public static long IslamicDayToJdn(this IslamicDay islamic)
        {
            return IslamicDayToJdn(islamic.Year, islamic.Month, islamic.Day);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="islamic"></param>
        /// <returns></returns>
        public static PersianDay IslamicDayToPersianDay(this IslamicDay islamic)
        {
            return JdnToPersianDay(IslamicDayToJdn(islamic));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="persian"></param>
        /// <returns></returns>
        public static DateTime PersianDayToGregorian(this PersianDay persian)
        {
            return JdnToGregorianDateTime(PersianDayToJdn(persian));
        }

        /// <summary>
        /// تبدیل تاریخ شمسی به قمری
        /// </summary>
        /// <param name="persian">روز شمسی</param>
        /// <returns></returns>
        public static IslamicDay PersianDayToIslamicDay(this PersianDay persian)
        {
            return JdnToIslamicDay(PersianDayToJdn(persian));
        }

        /// <summary>
        /// تبدیل تاریخ شمسی به قمری
        /// </summary>
        /// <param name="year">سال شمسی</param>
        /// <param name="month">ماه شمسی</param>
        /// <param name="day">روز شمسی</param>
        /// <returns></returns>
        public static IslamicDay PersianDayToIslamicDay(int year, int month, int day)
        {
            return JdnToIslamicDay(PersianDayToJdn(year, month, day));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="persian"></param>
        /// <returns></returns>
        public static long PersianDayToJdn(this PersianDay persian)
        {
            return PersianDayToJdn(persian.Year, persian.Month, persian.Day);
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static PersianDay GregorianToPersianDay(DateTime date)
        {
            System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
            return new PersianDay(pc.GetYear(date), pc.GetMonth(date), pc.GetDayOfMonth(date));

        }
    }
}
