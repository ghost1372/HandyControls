using HandyControl.Data;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HandyControl.Controls
{
    public class SpeedoMeter : Control
    {

        #region Value
        protected virtual void OnValueChanged(FunctionEventArgs<double> e) => RaiseEvent(e);

        /// <summary>
        ///     值改变事件
        /// </summary>
        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble,
                typeof(EventHandler<FunctionEventArgs<double>>), typeof(SpeedoMeter));

        /// <summary>
        ///     值改变事件
        /// </summary>
        public event EventHandler<FunctionEventArgs<double>> ValueChanged
        {
            add => AddHandler(ValueChangedEvent, value);
            remove => RemoveHandler(ValueChangedEvent, value);
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }


        //todo: onceorce removed
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(SpeedoMeter), new PropertyMetadata(ValueBoxes.Double0Box, OnValueChanged, OnCoerceValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (SpeedoMeter)d;
            var v = (double)e.NewValue;

            ctl.OnValueChanged(new FunctionEventArgs<double>(ValueChangedEvent, ctl)
            {
                Info = v
            });
        }

        private static object OnCoerceValueChanged(DependencyObject d, object value)
        {
            var s = (SpeedoMeter)d;
            double val = (double)value;
            if (val > s.MaximumValue)
                val = s.MaximumValue;
            return val;
        }

        public double MaximumValue
        {
            get { return (double)GetValue(MaximumValueProperty); }
            set { SetValue(MaximumValueProperty, value); }
        }

        public static readonly DependencyProperty MaximumValueProperty =
            DependencyProperty.Register("MaximumValue", typeof(double), typeof(SpeedoMeter), new PropertyMetadata(ValueBoxes.Double100Box));



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



        public Visibility ValueVisibility
        {
            get { return (Visibility)GetValue(ValueVisibilityProperty); }
            set { SetValue(ValueVisibilityProperty, value); }
        }

        public static readonly DependencyProperty ValueVisibilityProperty =
            DependencyProperty.Register("ValueVisibility", typeof(Visibility), typeof(SpeedoMeter), new PropertyMetadata(Visibility.Visible));

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

        public Visibility StatusVisibility
        {
            get { return (Visibility)GetValue(StatusVisibilityProperty); }
            set { SetValue(StatusVisibilityProperty, value); }
        }

        public static readonly DependencyProperty StatusVisibilityProperty =
            DependencyProperty.Register("StatusVisibility", typeof(Visibility), typeof(SpeedoMeter), new PropertyMetadata(Visibility.Visible));


        #endregion

        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(SpeedoMeter), new PropertyMetadata(180d));

        public Brush StrokeColor
        {
            get { return (Brush)GetValue(StrokeColorProperty); }
            set { SetValue(StrokeColorProperty, value); }
        }

        public static readonly DependencyProperty StrokeColorProperty =
            DependencyProperty.Register("StrokeColor", typeof(Brush), typeof(SpeedoMeter), new PropertyMetadata(null));
    }
}
