using System.Globalization;
using System.Windows.Data;
using System;

namespace HandyControl.Tools.Converter;

public class PersianDateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            var persianDate = new PersianDateTime(dateTime);
            return $"{persianDate.ShamsiDate} {persianDate.Hour.ToString("00")}:{persianDate.Minute.ToString("00")}:{persianDate.Second.ToString("00")}";
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
