using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace HandyControl.Controls
{
    /// <summary>
    /// Timeline
    /// </summary>
    /// <remarks>add by zhidanfeng 2017.5.29</remarks>
    public class Timeline : ItemsControl
    {
        #region private fields

        #endregion

        #region DependencyProperty

        #region FirstSlotTemplate

        /// <summary>
        /// Gets or sets the appearance of the first timeline point
        /// </summary>
        [Bindable(true), Description("Gets or sets the appearance of the first timeline point")]
        public DataTemplate FirstSlotTemplate
        {
            get { return (DataTemplate)GetValue(FirstSlotTemplateProperty); }
            set { SetValue(FirstSlotTemplateProperty, value); }
        }

        public static readonly DependencyProperty FirstSlotTemplateProperty =
            DependencyProperty.Register("FirstSlotTemplate", typeof(DataTemplate), typeof(Timeline));

        #endregion

        #region MiddleSlotTemplate

        /// <summary>
        /// Get or set the middle timeline point
        /// </summary>
        [Bindable(true), Description("Get or set the middle timeline point")]
        public DataTemplate MiddleSlotTemplate
        {
            get { return (DataTemplate)GetValue(MiddleSlotTemplateProperty); }
            set { SetValue(MiddleSlotTemplateProperty, value); }
        }

        public static readonly DependencyProperty MiddleSlotTemplateProperty =
            DependencyProperty.Register("MiddleSlotTemplate", typeof(DataTemplate), typeof(Timeline));

        #endregion

        #region LastItemTemplate

        /// <summary>
        /// Gets or sets the appearance of the last timeline point
        /// </summary>
        [Bindable(true), Description("Gets or sets the appearance of the last timeline point")]
        public DataTemplate LastSlotTemplate
        {
            get { return (DataTemplate)GetValue(LastSlotTemplateProperty); }
            set { SetValue(LastSlotTemplateProperty, value); }
        }

        public static readonly DependencyProperty LastSlotTemplateProperty =
            DependencyProperty.Register("LastSlotTemplate", typeof(DataTemplate), typeof(Timeline));

        #endregion

        #region IsCustomEverySlot

        /// <summary>
        /// Gets or sets whether to customize the appearance of each timeline point.
        /// </summary>
        [Bindable(true), Description("Gets or sets whether to customize the appearance of each timeline point. When the property value is True, FirstSlotTemplate、MiddleSlotTemplate、LastSlotTemplate The properties will be invalid. You can only set the SlotTemplate to define the style of each timeline point.")]
        public bool IsCustomEverySlot
        {
            get { return (bool)GetValue(IsCustomEverySlotProperty); }
            set { SetValue(IsCustomEverySlotProperty, value); }
        }

        public static readonly DependencyProperty IsCustomEverySlotProperty =
            DependencyProperty.Register("IsCustomEverySlot", typeof(bool), typeof(Timeline), new PropertyMetadata(false));

        #endregion

        #region SlotTemplate

        /// <summary>
        /// Get or set the appearance of each timeline point
        /// </summary>
        [Bindable(true), Description("Gets or sets the appearance of each timeline point. This property takes effect only when the IsCustomEverySlot property is True")]
        public DataTemplate SlotTemplate
        {
            get { return (DataTemplate)GetValue(SlotTemplateProperty); }
            set { SetValue(SlotTemplateProperty, value); }
        }

        public static readonly DependencyProperty SlotTemplateProperty =
            DependencyProperty.Register("SlotTemplate", typeof(DataTemplate), typeof(Timeline));

        #endregion

        #endregion

        #region Constructors

        static Timeline()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata(typeof(Timeline)));
        }

        #endregion

        #region Override

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            int index = this.ItemContainerGenerator.IndexFromContainer(element);
            TimelineItem timelineItem = element as TimelineItem;
            if (timelineItem == null)
            {
                return;
            }

            if (index == 0)
            {
                timelineItem.IsFirstItem = true;
            }

            if (index == this.Items.Count - 1)
            {
                timelineItem.IsLastItem = true;
            }

            base.PrepareContainerForItemOverride(timelineItem, item);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TimelineItem();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            //The following code is to set the appearance of each item correctly when adding or removing items
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex == 0) //If the newly added item is first, change the original first attribute value
                    {
                        this.SetTimelineItem(e.NewStartingIndex + e.NewItems.Count);
                    }

                    //If the newly added item is placed in the last digit, then the original last digit property value is changed
                    if (e.NewStartingIndex == this.Items.Count - e.NewItems.Count)
                    {
                        this.SetTimelineItem(e.NewStartingIndex - 1);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex == 0) //If the first one is removed, change the attribute value of the updated first item
                    {
                        this.SetTimelineItem(0);
                    }
                    else
                    {
                        this.SetTimelineItem(e.OldStartingIndex - 1);
                    }
                    break;
            }
        }
        #endregion

        #region private function
        /// <summary>
        /// Setting the TimelineItem's Position Property
        /// </summary>
        /// <param name="index"></param>
        private void SetTimelineItem(int index)
        {
            if (index > this.Items.Count || index < 0)
            {
                return;
            }

            TimelineItem TimelineItem = this.ItemContainerGenerator.ContainerFromIndex(index) as TimelineItem;
            if (TimelineItem == null)
            {
                return;
            }
            TimelineItem.IsFirstItem = index == 0;
            TimelineItem.IsLastItem = index == this.Items.Count - 1;
            TimelineItem.IsMiddleItem = index > 0 && index < this.Items.Count - 1;
        }
        #endregion
    }
}
