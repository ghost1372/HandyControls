using HandyControl.Data;
using HandyControl.Tools;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace HandyControl.Controls
{
    [TemplatePart(Name = Canvas_Key, Type = typeof(Canvas))]
    [TemplatePart(Name = Grid_Key, Type = typeof(Grid))]
    [TemplatePart(Name = LabelContent_Key, Type = typeof(Label))]
    [TemplatePart(Name = LabelAnima_Key, Type = typeof(Label))]
    public class NeonLabel : ContentControl
    {
        private const string Canvas_Key = "PART_CvaMain";
        private const string Grid_Key = "PART_GrdMain";
        private const string LabelContent_Key = "PART_LblContent";
        private const string LabelAnima_Key = "PART_LblAnima";

        Canvas canvasMain;
        Grid gridMain;
        Label labelContent;
        Label labelAnima;

        static NeonLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NeonLabel),
                   new FrameworkPropertyMetadata(typeof(NeonLabel)));

        }

        public Style NeonStyle
        {
            get { return (Style)GetValue(NeonStyleProperty); }
            set { SetValue(NeonStyleProperty, value); }
        }

        public static readonly DependencyProperty NeonStyleProperty =
            DependencyProperty.Register("NeonStyle", typeof(Style), typeof(NeonLabel), new PropertyMetadata(ResourceHelper.GetResource<Style>(ResourceToken.LabelDefault)));


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            canvasMain = GetTemplateChild(Canvas_Key) as Canvas;
            gridMain = GetTemplateChild(Grid_Key) as Grid;
            labelContent = GetTemplateChild(LabelContent_Key) as Label;
            labelAnima = GetTemplateChild(LabelAnima_Key) as Label;

            canvasMain.SizeChanged += CanvasMain_SizeChanged;

        }

        private void CanvasMain_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            gridMain.Width = canvasMain.ActualWidth;
            gridMain.Height = canvasMain.ActualHeight;
        }

        /// <summary>
        /// Change to Next.
        /// </summary>
        /// <param name="neonLabelType">Neon label type.</param>
        /// <param name="next">Next content, or brush.</param>
        /// <param name="durationSecond"></param>
        public void Next(NeonLabelType neonLabelType, object next = null, double durationSecond = 0.5)
        {
            switch (neonLabelType)
            {
                case NeonLabelType.FadeNext:
                    FadeNext(next, durationSecond);
                    break;
                case NeonLabelType.SlideNext:
                    SlideNext(next, durationSecond);
                    break;
                case NeonLabelType.ScrollToEnd:
                    ScrollToEnd(durationSecond);
                    break;
            }
        }

        private void FadeNext(object nextContent, double durationSecond)
        {
            labelAnima.Foreground = labelContent.Foreground;
            labelContent.Margin = new Thickness(0, 0, 0, 0);
            labelAnima.Content = nextContent;
            var doubleAnima1 = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(durationSecond),
            };
            var doubleAnima2 = new DoubleAnimation()
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(durationSecond),
            };
            doubleAnima2.Completed += delegate
            {
                labelAnima.BeginAnimation(OpacityProperty, null);
                labelContent.BeginAnimation(OpacityProperty, null);
                Content = nextContent;
                labelAnima.Opacity = 0;
                labelContent.Opacity = 1;
            };
            labelAnima.BeginAnimation(OpacityProperty, doubleAnima1);
            labelContent.BeginAnimation(OpacityProperty, doubleAnima2);

        }

        private void SlideNext(object nextContent, double durationSecond)
        {
            labelAnima.Foreground = labelContent.Foreground;
            labelAnima.Opacity = 1;
            labelContent.Margin = new Thickness(0, 0, 0, 0);
            labelAnima.Content = nextContent;
            var thicknessAnima1 = new ThicknessAnimation()
            {
                From = new Thickness(0, ActualHeight * 2, 0, 0),
                To = new Thickness(0),
                Duration = TimeSpan.FromSeconds(durationSecond),
            };
            var thicknessAnima2 = new ThicknessAnimation()
            {
                From = new Thickness(0),
                To = new Thickness(0, -ActualHeight * 2, 0, 0),
                Duration = TimeSpan.FromSeconds(durationSecond),
            };
            thicknessAnima2.Completed += delegate
            {
                labelAnima.BeginAnimation(MarginProperty, null);
                labelContent.BeginAnimation(MarginProperty, null);
                Content = nextContent;
                labelContent.Margin = new Thickness(0);
                labelAnima.Opacity = 0;
            };
            labelAnima.BeginAnimation(MarginProperty, thicknessAnima1);
            labelContent.BeginAnimation(MarginProperty, thicknessAnima2);

        }

        private void ScrollToEnd(double durationSecond)
        {
            var offset = labelContent.ActualWidth - gridMain.ActualWidth + FontSize;
            labelContent.Margin = new Thickness(0, 0, 0, 0);

            if (offset < 0)
                offset = 0;

            var thicknessAnima = new ThicknessAnimation()
            {
                From = new Thickness(0),
                To = new Thickness(-offset, 0, 0, 0),
                BeginTime = TimeSpan.FromSeconds(durationSecond * 0.3),
                Duration = TimeSpan.FromSeconds(durationSecond * 0.7),
            };
            thicknessAnima.Completed += delegate
            {
                labelContent.BeginAnimation(MarginProperty, null);
                labelContent.Margin = new Thickness(-offset, 0, 0, 0);
            };
            labelContent.BeginAnimation(MarginProperty, thicknessAnima);


        }
    }
}
