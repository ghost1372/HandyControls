using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace HandyControl.Controls
{
    public class TimelineItem : ContentControl
    {
        #region DependencyProperty

        #region IsFirstItem
        /// <summary>
        /// Gets or sets whether the item is the first in the list
        /// </summary>
        [Bindable(true), Description("Gets or sets whether the item is the first in the list")]
        public bool IsFirstItem
        {
            get { return (bool)GetValue(IsFirstItemProperty); }
            set { SetValue(IsFirstItemProperty, value); }
        }

        public static readonly DependencyProperty IsFirstItemProperty =
            DependencyProperty.Register("IsFirstItem", typeof(bool), typeof(TimelineItem), new PropertyMetadata(false));

        #endregion

        #region IsMiddleItem

        /// <summary>
        /// Gets or sets whether the item is in the middle of the list
        /// </summary>
        [Bindable(true), Description("Gets or sets whether the item is in the middle of the list")]
        public bool IsMiddleItem
        {
            get { return (bool)GetValue(IsMiddleItemProperty); }
            set { SetValue(IsMiddleItemProperty, value); }
        }

        public static readonly DependencyProperty IsMiddleItemProperty =
            DependencyProperty.Register("IsMiddleItem", typeof(bool), typeof(TimelineItem), new PropertyMetadata(false));

        #endregion

        #region IsLastItem
        /// <summary>
        /// Gets or sets whether the item is the last one in the list
        /// </summary>
        [Bindable(true), Description("Gets or sets whether the item is the last one in the list")]
        public bool IsLastItem
        {
            get { return (bool)GetValue(IsLastItemProperty); }
            set { SetValue(IsLastItemProperty, value); }
        }

        public static readonly DependencyProperty IsLastItemProperty =
            DependencyProperty.Register("IsLastItem", typeof(bool), typeof(TimelineItem), new PropertyMetadata(false));

        #endregion

        #endregion

        #region Constructors

        static TimelineItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimelineItem), new FrameworkPropertyMetadata(typeof(TimelineItem)));
        }

        #endregion
    }
}
