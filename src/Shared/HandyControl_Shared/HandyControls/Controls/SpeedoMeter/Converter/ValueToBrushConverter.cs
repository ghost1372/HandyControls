using HandyControl.Data;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace HandyControl.Tools.Converter
{
    public class ValueToBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null)
            {
                var currentValue = System.Convert.ToInt32(values[0]) / (double)values[1];

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
            return ResourceHelper.GetResource<Brush>(ResourceToken.PrimaryBrush);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
