using HandyControl.Controls;
using HandyControl.Data;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace HandyControl.Tools.Converter
{
    public class ValueToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var minimumInput = 0;
            var maximumInput = System.Convert.ToInt32(parameter);

            var currentValue = ((double)value - minimumInput) / (maximumInput - minimumInput);

            var color = ResourceHelper.GetResource<Brush>(ResourceToken.PrimaryBrush);
            if (currentValue < 0.30)
                color = ResourceHelper.GetResource<Brush>(ResourceToken.PrimaryBrush);
            else if (currentValue >= 0.30 && currentValue < 0.50)
                color = ResourceHelper.GetResource<Brush>(ResourceToken.SuccessBrush);
            else if (currentValue >= 0.50 && currentValue < 0.80)
                color = ResourceHelper.GetResource<Brush>(ResourceToken.WarningBrush);
            else if (currentValue >= 0.80)
                color = ResourceHelper.GetResource<Brush>(ResourceToken.DangerBrush);

            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
