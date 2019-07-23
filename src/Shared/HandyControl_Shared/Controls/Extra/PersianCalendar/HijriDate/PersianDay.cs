namespace HandyControl
{
    /// <summary>
    /// اجزای روز شمسی
    /// </summary>
    public class PersianDay
    {
        /// <summary>
        /// سال شمسی
        /// </summary>
        public int Year { set; get; }

        /// <summary>
        /// ماه شمسی
        /// </summary>
        public int Month { set; get; }

        /// <summary>
        /// روز شمسی
        /// </summary>
        public int Day { set; get; }

        /// <summary>
        /// اجزای روز شمسی
        /// </summary>
        public PersianDay() { }

        /// <summary>
        /// اجزای روز شمسی
        /// </summary>
        public PersianDay(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        /// <summary>
        /// ToString()
        /// </summary>
        public override string ToString()
        {
            return $"{Year}/{Month.ToString("00")}/{Day.ToString("00")}";
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            PersianDay day = obj as PersianDay;
            if (day == null)
            {
                return false;
            }

            return Year == day.Year &&
                   Month == day.Month &&
                   Day == day.Day;
        }

        /// <summary>
        /// GetHashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Year.GetHashCode();
                hash = hash * 23 + Month.GetHashCode();
                hash = hash * 23 + Day.GetHashCode();
                return hash;
            }
        }
    }
}
