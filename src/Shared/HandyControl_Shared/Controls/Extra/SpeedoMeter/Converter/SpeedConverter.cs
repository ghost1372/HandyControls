﻿using HandyControl.Controls;
using System;
using System.Globalization;
using System.Windows.Data;

namespace HandyControl.Tools.Converter
{
    public class SpeedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var startAngle = System.Convert.ToDouble(parameter) * -1;
            var endAngle = startAngle + (((double)value) * 2);
            return endAngle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
