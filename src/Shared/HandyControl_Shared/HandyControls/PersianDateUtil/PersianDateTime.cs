using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

namespace HandyControl.Tools
{
    public partial class PersianDateTime : IConvertible
    {
        private static readonly System.Globalization.PersianCalendar persianCalendar =
            new System.Globalization.PersianCalendar();

        private DateTime dateTime;

        public DateTime DateTime
        {
            get => dateTime;
            set => Initial(value);
        }
        public int Millisecond { get; }
        public int Second { get; private set; }
        public int Minute { get; private set; }
        public int Hour { get; private set; }
        public int Day { get; private set; }
        public int Month { get; private set; }
        public int Year { get; private set; }

        public string DayOfWeek => dayOfWeek[DateTime.DayOfWeek.GetHashCode()];

        public string MonthOfYear => months[Month];

        public string ShamsiDate => ToString();

        public TimeSpan TimeOfDay => DateTime.TimeOfDay;

        public static PersianDateTime Now => new PersianDateTime(DateTime.Now);

        public static PersianDateTime Today =>
            new PersianDateTime(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0,
                0));

        public static PersianDateTime UtcNow => new PersianDateTime(DateTime.UtcNow);


        public TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }

        public bool ToBoolean(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return DateTime;
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(
                $"{Year}{Month.ToString("00")}{Day.ToString("00")}{Hour.ToString("00")}{Minute.ToString("00")}{Second.ToString("00")}");
        }

        public double ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(
                $"{Year.ToString("00")}{Month.ToString("00")}{Day.ToString("00")}{Hour.ToString("00")}{Minute.ToString("00")}{Second.ToString("00")}");
        }

        public short ToInt16(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32($"{Year.ToString("00")}{Month.ToString("00")}{Day.ToString("00")}");
        }

        public long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt32($"{Year.ToString("00")}{Month.ToString("00")}{Day.ToString("00")}");
        }

        public sbyte ToSByte(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider)
        {
            return
                $"{Year}/{Month.ToString("00")}/{Day.ToString("00")} {Hour.ToString("00")}:{Minute.ToString("00")}:{Second.ToString("00")}";
        }

        public object ToType(Type conversionType, IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }


        public bool Equals(PersianDateTime other)
        {
            return this == other;
        }

        public string ToString(string format)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("fa-IR");
            var dateTime = new DateTime(Year, Month, Day, Hour, Minute, Second, Millisecond, persianCalendar);
            return dateTime.ToString(format, new CultureInfo("fa-IR"));
        }

        #region Fields

        private static readonly string[] dayOfWeek =
            {"یکشنبه", "دوشنبه", "سه شنبه", "چهارشنبه", "پنج شنبه", "جمعه", "شنبه"};

        private static readonly string[] months =
            {"", "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"};

        private const string TimePattern =
            @"^((([0-1]?[0-9])|(2[0-3]):[0-5]?[0-9])|(([0-1]?[0-9])|(2[0-3]):[0-5]?[0-9]:[0-5]?[0-9]))$";

        private const string PersianDatePattern =
            @"^$|^([1][0-9]{3}[/\/]([0][1-6])[/\/]([0][1-9]|[12][0-9]|[3][01])|[1][0-9]{3}[/\/]([0][7-9]|[1][012])[/\/]([0][1-9]|[12][0-9]|(30)))$";

        #endregion


        #region Constructor

        public PersianDateTime(int year, int month, int day, int hour = 0, int minute = 0, int second = 0,
            int millisecond = 0)
        {
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
            Second = second;
            Millisecond = millisecond;
            DateTime = persianCalendar.ToDateTime(Year, Month, Day, Hour, Minute, Second, Millisecond);
        }

        public bool IsTimeValid(string time)
        {
            return Regex.IsMatch(time, TimePattern);
        }

        public bool IsPersianDateValid(string persianDate)
        {
            return Regex.IsMatch(persianDate, PersianDatePattern);
        }

        public PersianDateTime(string shamsiDate = "1399/10/03 20:43:00")
        {
            if (!IsPersianDateValid(shamsiDate.Replace(" ", "").Substring(0, 10)))
            {
                throw new ArgumentOutOfRangeException("فرمت تاریخ وارد شده صحیح نمی باشد");
            }

            if (shamsiDate.Length > 10)
            {
                if (IsTimeValid(shamsiDate.Substring(11, shamsiDate.Length - 11)))
                {
                    int h = 0, m = 0, s = 0;
                    if (shamsiDate.Length > 11)
                    {
                        int.TryParse(shamsiDate.Substring(11, 2), out h);
                    }

                    if (shamsiDate.Length > 14)
                    {
                        int.TryParse(shamsiDate.Substring(14, 2), out m);
                    }

                    if (shamsiDate.Length > 17)
                    {
                        int.TryParse(shamsiDate.Substring(17, 2), out s);
                    }

                    Hour = h;
                    Minute = m;
                    Second = s;
                }
            }

            Year = Convert.ToInt32(shamsiDate.Substring(0, 4));
            Month = Convert.ToInt32(shamsiDate.Substring(5, 2));
            Day = Convert.ToInt32(shamsiDate.Substring(8, 2));
            DateTime = persianCalendar.ToDateTime(Year, Month, Day, Hour, Minute, Second, Millisecond);
        }

        public PersianDateTime(DateTime dateTime)
        {
            Initial(dateTime);
        }

        private void Initial(DateTime datetime)
        {
            dateTime = datetime;
            Year = persianCalendar.GetYear(datetime);
            Month = persianCalendar.GetMonth(datetime);
            Day = persianCalendar.GetDayOfMonth(datetime);
            Hour = persianCalendar.GetHour(datetime);
            Minute = persianCalendar.GetMinute(datetime);
            Second = persianCalendar.GetSecond(datetime);
        }

        #endregion
    }
}
