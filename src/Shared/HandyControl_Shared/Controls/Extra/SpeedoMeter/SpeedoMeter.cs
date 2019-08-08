using HandyControl.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HandyControl.Controls
{
    public class SpeedoMeter : ContentControl
    {
        internal static SpeedoMeter speedo;
        public SpeedoMeter()
        {
            speedo = this;
        }

        public static string getMaximumValue()
        {
            return speedo.MaximumValue.ToString();
        }
        public static string getMinimumValue()
        {
            return speedo.MinimumValue.ToString();
        }

        #region Value
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(SpeedoMeter), new PropertyMetadata(ValueBoxes.Double0Box, null, OnCoerceValueChanged));

        private static object OnCoerceValueChanged(DependencyObject d, object value)
        {
            var s = (SpeedoMeter)d;
            double val = (double)value;
            if (val > s.MaximumValue)
                val = s.MaximumValue;
            return val;
        }

        public double ValueFontSize
        {
            get { return (double)GetValue(ValueFontSizeProperty); }
            set { SetValue(ValueFontSizeProperty, value); }
        }

        public static readonly DependencyProperty ValueFontSizeProperty =
            DependencyProperty.Register("ValueFontSize", typeof(double), typeof(SpeedoMeter), new PropertyMetadata(ValueBoxes.Double100Box));


        public Brush ValueColor
        {
            get { return (Brush)GetValue(ValueColorProperty); }
            set { SetValue(ValueColorProperty, value); }
        }

        public static readonly DependencyProperty ValueColorProperty =
            DependencyProperty.Register("ValueColor", typeof(Brush), typeof(SpeedoMeter), new PropertyMetadata(null));
        #endregion

        #region Status
        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(SpeedoMeter), new PropertyMetadata("Km/h"));


        public Brush StatusColor
        {
            get { return (Brush)GetValue(StatusColorProperty); }
            set { SetValue(StatusColorProperty, value); }
        }

        public static readonly DependencyProperty StatusColorProperty =
            DependencyProperty.Register("StatusColor", typeof(Brush), typeof(SpeedoMeter), new PropertyMetadata(null));



        public double StatusFontSize
        {
            get { return (double)GetValue(StatusFontSizeProperty); }
            set { SetValue(StatusFontSizeProperty, value); }
        }

        public static readonly DependencyProperty StatusFontSizeProperty =
            DependencyProperty.Register("StatusFontSize", typeof(double), typeof(SpeedoMeter), new PropertyMetadata(ValueBoxes.Double20Box));


        #endregion


        public double MinimumValue
        {
            get { return (double)GetValue(MinimumValueProperty); }
            set { SetValue(MinimumValueProperty, value); }
        }

        public static readonly DependencyProperty MinimumValueProperty =
            DependencyProperty.Register("MinimumValue", typeof(double), typeof(SpeedoMeter), new PropertyMetadata(ValueBoxes.Double0Box));



        public double MaximumValue
        {
            get { return (double)GetValue(MaximumValueProperty); }
            set { SetValue(MaximumValueProperty, value); }
        }

        public static readonly DependencyProperty MaximumValueProperty =
            DependencyProperty.Register("MaximumValue", typeof(double), typeof(SpeedoMeter), new PropertyMetadata(ValueBoxes.Double100Box, OnMaximumValueChanged));

        private static void OnMaximumValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //complete circle
            if ((double)e.NewValue > 180)
                speedo.MaximumValue = 180;
        }

        public Brush StrokeColor
        {
            get { return (Brush)GetValue(StrokeColorProperty); }
            set { SetValue(StrokeColorProperty, value); }
        }

        public static readonly DependencyProperty StrokeColorProperty =
            DependencyProperty.Register("StrokeColor", typeof(Brush), typeof(SpeedoMeter), new PropertyMetadata(null));






    }
}
