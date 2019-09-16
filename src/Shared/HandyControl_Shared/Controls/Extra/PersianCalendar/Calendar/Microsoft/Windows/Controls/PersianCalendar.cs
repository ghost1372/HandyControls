//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using Microsoft.Windows.Controls.Primitives;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using CalendarAutomationPeer = Microsoft.Windows.Automation.Peers.CalendarAutomationPeer;
using Microsoft.Windows.Controls;
using CalendarMode = Microsoft.Windows.Controls.CalendarMode;
using SelectedDatesCollection = Microsoft.Windows.Controls.SelectedDatesCollection;
using CalendarBlackoutDatesCollection = Microsoft.Windows.Controls.CalendarBlackoutDatesCollection;
using CalendarModeChangedEventArgs = Microsoft.Windows.Controls.CalendarModeChangedEventArgs;
using CalendarSelectionMode = Microsoft.Windows.Controls.CalendarSelectionMode;
using CalendarDateChangedEventArgs = Microsoft.Windows.Controls.CalendarDateChangedEventArgs;
using CalendarDateRange = Microsoft.Windows.Controls.CalendarDateRange;

namespace HandyControl.Controls
{
    /// <summary>
    /// Represents a control that enables a user to select a date by using a visual calendar display. 
    /// </summary>
    [TemplatePart(Name = PersianCalendar.ElementRoot, Type = typeof(Panel))]
    [TemplatePart(Name = PersianCalendar.ElementMonth, Type = typeof(CalendarItem))]
    public class PersianCalendar : Control
    {
        #region Constants

        private const string ElementRoot = "PART_Root";
        private const string ElementMonth = "PART_CalendarItem";

        private const int COLS = 7;
        private const int ROWS = 7;
        private const int YEAR_ROWS = 3;
        private const int YEAR_COLS = 4;
        private const int YEARS_PER_DECADE = 10;

        #endregion Constants

        #region Data
        private DateTime? _hoverStart;
        private DateTime? _hoverEnd;
        private bool _isShiftPressed;
        private DateTime? _currentDate;
        private CalendarItem _monthControl;
        private readonly CalendarBlackoutDatesCollection _blackoutDates;
        private readonly SelectedDatesCollection _selectedDates;

        #endregion Data

        #region Public Events

        public static readonly RoutedEvent SelectedDatesChangedEvent = EventManager.RegisterRoutedEvent("SelectedDatesChanged", RoutingStrategy.Direct, typeof(EventHandler<SelectionChangedEventArgs>), typeof(PersianCalendar));

        /// <summary>
        /// Occurs when a date is selected.
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectedDatesChanged
        {
            add { AddHandler(SelectedDatesChangedEvent, value); }
            remove { RemoveHandler(SelectedDatesChangedEvent, value); }
        }

        /// <summary>
        /// Occurs when the DisplayDate property is changed.
        /// </summary>
        public event EventHandler<CalendarDateChangedEventArgs> DisplayDateChanged;

        /// <summary>
        /// Occurs when the DisplayMode property is changed. 
        /// </summary>
        public event EventHandler<CalendarModeChangedEventArgs> DisplayModeChanged;

        /// <summary>
        /// Occurs when the SelectionMode property is changed. 
        /// </summary>
        public event EventHandler<EventArgs> SelectionModeChanged;

        #endregion Public Events

        /// <summary>
        /// Static constructor
        /// </summary>
        static PersianCalendar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PersianCalendar), new FrameworkPropertyMetadata(typeof(PersianCalendar)));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(PersianCalendar), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(PersianCalendar), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));

            EventManager.RegisterClassHandler(typeof(PersianCalendar), UIElement.GotFocusEvent, new RoutedEventHandler(OnGotFocus));
            LanguageProperty.OverrideMetadata(typeof(PersianCalendar), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnLanguageChanged)));

        }

        /// <summary>
        /// Initializes a new instance of the Calendar class.
        /// </summary>
        public PersianCalendar()
        {
            _blackoutDates = new CalendarBlackoutDatesCollection(this);
            _selectedDates = new SelectedDatesCollection(this);
            DisplayDate = DateTime.Today;

            System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CurrentCulture;
            if (culture.LCID.Equals(1065))
            {
                SetFlowDirection(this, FlowDirection.RightToLeft);
            }
            else
            {
                SetFlowDirection(this, FlowDirection.LeftToRight);
            }
          }
        #region Public Properties

        #region BlackoutDates

        /// <summary>
        /// Gets or sets the dates that are not selectable.
        /// </summary>
        public CalendarBlackoutDatesCollection BlackoutDates => _blackoutDates;

        #endregion BlackoutDates

        #region CalendarButtonStyle

        /// <summary>
        /// Gets or sets the style for displaying a CalendarButton.
        /// </summary>
        public Style CalendarButtonStyle
        {
            get => (Style)GetValue(CalendarButtonStyleProperty);
            set => SetValue(CalendarButtonStyleProperty, value);
        }

        /// <summary>
        /// Identifies the CalendarButtonStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty CalendarButtonStyleProperty =
            DependencyProperty.Register(
            "CalendarButtonStyle",
            typeof(Style),
            typeof(PersianCalendar));

        #endregion CalendarButtonStyle

        #region CalendarDayButtonStyle

        /// <summary>
        /// Gets or sets the style for displaying a day.
        /// </summary>
        public Style CalendarDayButtonStyle
        {
            get => (Style)GetValue(CalendarDayButtonStyleProperty);
            set => SetValue(CalendarDayButtonStyleProperty, value);
        }

        /// <summary>
        /// Identifies the DayButtonStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty CalendarDayButtonStyleProperty =
            DependencyProperty.Register(
            "CalendarDayButtonStyle",
            typeof(Style),
            typeof(PersianCalendar));

        #endregion CalendarDayButtonStyle

        #region CalendarItemStyle

        /// <summary>
        /// Gets or sets the style for a Month.
        /// </summary>
        public Style CalendarItemStyle
        {
            get => (Style)GetValue(CalendarItemStyleProperty);
            set => SetValue(CalendarItemStyleProperty, value);
        }

        /// <summary>
        /// Identifies the MonthStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty CalendarItemStyleProperty =
            DependencyProperty.Register(
            "CalendarItemStyle",
            typeof(Style),
            typeof(PersianCalendar));

        #endregion CalendarItemStyle

        #region DisplayDate

        /// <summary>
        /// Gets or sets the date to display.
        /// </summary>
        /// 
        public DateTime DisplayDate
        {
            get => (DateTime)GetValue(DisplayDateProperty);
            set => SetValue(DisplayDateProperty, value);
        }

        /// <summary>
        /// Identifies the DisplayDate dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayDateProperty =
            DependencyProperty.Register(
            "DisplayDate",
            typeof(DateTime),
            typeof(PersianCalendar),
            new FrameworkPropertyMetadata(DateTime.MinValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDisplayDateChanged, CoerceDisplayDate));

        /// <summary>
        /// DisplayDateProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its DisplayDate.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnDisplayDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PersianCalendar c = d as PersianCalendar;
            Debug.Assert(c != null);

            c.DisplayDateInternal = DateTimeHelper.DiscardDayTime((DateTime)e.NewValue);
            c.UpdateCellItems();
            c.OnDisplayDateChanged(new CalendarDateChangedEventArgs((DateTime)e.OldValue, (DateTime)e.NewValue));


        }

        private static object CoerceDisplayDate(DependencyObject d, object value)
        {
            PersianCalendar c = d as PersianCalendar;

            DateTime date = (DateTime)value;
            if (c.DisplayDateStart.HasValue && (date < c.DisplayDateStart.Value))
            {
                value = c.DisplayDateStart.Value;
            }
            else if (c.DisplayDateEnd.HasValue && (date > c.DisplayDateEnd.Value))
            {
                value = c.DisplayDateEnd.Value;
            }

            return value;
        }

        #endregion DisplayDate

        #region DisplayDateEnd

        /// <summary>
        /// Gets or sets the last date to be displayed.
        /// </summary>
        /// 
        public DateTime? DisplayDateEnd
        {
            get => (DateTime?)GetValue(DisplayDateEndProperty);
            set => SetValue(DisplayDateEndProperty, value);
        }

        /// <summary>
        /// Identifies the DisplayDateEnd dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayDateEndProperty =
            DependencyProperty.Register(
            "DisplayDateEnd",
            typeof(DateTime?),
            typeof(PersianCalendar),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDisplayDateEndChanged, CoerceDisplayDateEnd));

        /// <summary>
        /// DisplayDateEndProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its DisplayDateEnd.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnDisplayDateEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PersianCalendar c = d as PersianCalendar;
            Debug.Assert(c != null);

            c.CoerceValue(DisplayDateProperty);
            c.UpdateCellItems();
        }

        private static object CoerceDisplayDateEnd(DependencyObject d, object value)
        {
            PersianCalendar c = d as PersianCalendar;

            DateTime? date = (DateTime?)value;

            if (date.HasValue)
            {
                if (c.DisplayDateStart.HasValue && (date.Value < c.DisplayDateStart.Value))
                {
                    value = c.DisplayDateStart;
                }

                DateTime? maxSelectedDate = c.SelectedDates.MaximumDate;
                if (maxSelectedDate.HasValue && (date.Value < maxSelectedDate.Value))
                {
                    value = maxSelectedDate;
                }
            }

            return value;
        }

        #endregion DisplayDateEnd

        #region DisplayDateStart

        /// <summary>
        /// Gets or sets the first date to be displayed.
        /// </summary>
        /// 
        public DateTime? DisplayDateStart
        {
            get => (DateTime?)GetValue(DisplayDateStartProperty);
            set => SetValue(DisplayDateStartProperty, value);
        }

        /// <summary>
        /// Identifies the DisplayDateStart dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayDateStartProperty =
            DependencyProperty.Register(
            "DisplayDateStart",
            typeof(DateTime?),
            typeof(PersianCalendar),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDisplayDateStartChanged, CoerceDisplayDateStart));

        /// <summary>
        /// DisplayDateStartProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its DisplayDateStart.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnDisplayDateStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PersianCalendar c = d as PersianCalendar;
            Debug.Assert(c != null);

            c.CoerceValue(DisplayDateEndProperty);
            c.CoerceValue(DisplayDateProperty);
            c.UpdateCellItems();
        }

        private static object CoerceDisplayDateStart(DependencyObject d, object value)
        {
            PersianCalendar c = d as PersianCalendar;

            DateTime? date = (DateTime?)value;

            if (date.HasValue)
            {
                DateTime? minSelectedDate = c.SelectedDates.MinimumDate;
                if (minSelectedDate.HasValue && (date.Value > minSelectedDate.Value))
                {
                    value = minSelectedDate;
                }
            }

            return value;
        }

        #endregion DisplayDateStart

        #region DisplayMode

        /// <summary>
        /// Gets or sets a value indicating whether the calendar is displayed in months or years.
        /// </summary>
        public CalendarMode DisplayMode
        {
            get => (CalendarMode)GetValue(DisplayModeProperty);
            set => SetValue(DisplayModeProperty, value);
        }

        /// <summary>
        /// Identifies the DisplayMode dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register(
            "DisplayMode",
            typeof(CalendarMode),
            typeof(PersianCalendar),
            new FrameworkPropertyMetadata(CalendarMode.Month, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDisplayModePropertyChanged),
            new ValidateValueCallback(IsValidDisplayMode));

        /// <summary>
        /// DisplayModeProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its DisplayMode.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnDisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PersianCalendar c = d as PersianCalendar;
            Debug.Assert(c != null);
            CalendarMode mode = (CalendarMode)e.NewValue;
            CalendarMode oldMode = (CalendarMode)e.OldValue;
            CalendarItem monthControl = c.MonthControl;

            switch (mode)
            {
                case CalendarMode.Month:
                    {
                        if (oldMode == CalendarMode.Year || oldMode == CalendarMode.Decade)
                        {
                            // Cancel highlight when switching to month display mode
                            c.HoverStart = c.HoverEnd = null;
                            c.CurrentDate = c.DisplayDate;
                        }

                        c.UpdateCellItems();
                        break;
                    }

                case CalendarMode.Year:
                case CalendarMode.Decade:
                    {
                        if (oldMode == CalendarMode.Month)
                        {
                            c.DisplayDate = c.CurrentDate;
                        }

                        c.UpdateCellItems();
                        break;
                    }

                default:
                    Debug.Assert(false);
                    break;
            }

            c.OnDisplayModeChanged(new CalendarModeChangedEventArgs((CalendarMode)e.OldValue, mode));
        }

        #endregion DisplayMode

        #region FirstDayOfWeek

        /// <summary>
        /// Gets or sets the day that is considered the beginning of the week.
        /// </summary>
        public DayOfWeek FirstDayOfWeek
        {
            get => (DayOfWeek)GetValue(FirstDayOfWeekProperty);
            set => SetValue(FirstDayOfWeekProperty, value);
        }

        /// <summary>
        /// Identifies the FirstDayOfWeek dependency property.
        /// </summary>
        public static readonly DependencyProperty FirstDayOfWeekProperty =
            DependencyProperty.Register(
            "FirstDayOfWeek",
            typeof(DayOfWeek),
            typeof(PersianCalendar),
            new FrameworkPropertyMetadata(DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek,
                                OnFirstDayOfWeekChanged),
            new ValidateValueCallback(IsValidFirstDayOfWeek));

        /// <summary>
        /// FirstDayOfWeekProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its FirstDayOfWeek.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnFirstDayOfWeekChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PersianCalendar c = d as PersianCalendar;
            c.UpdateCellItems();
        }

        #endregion FirstDayOfWeek

        #region IsTodayHighlighted

        /// <summary>
        /// Gets or sets a value indicating whether the current date is highlighted.
        /// </summary>
        public bool IsTodayHighlighted
        {
            get => (bool)GetValue(IsTodayHighlightedProperty);
            set => SetValue(IsTodayHighlightedProperty, value);
        }

        /// <summary>
        /// Identifies the IsTodayHighlighted dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTodayHighlightedProperty =
            DependencyProperty.Register(
            "IsTodayHighlighted",
            typeof(bool),
            typeof(PersianCalendar),
            new FrameworkPropertyMetadata(true, OnIsTodayHighlightedChanged));

        /// <summary>
        /// IsTodayHighlightedProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its IsTodayHighlighted.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnIsTodayHighlightedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PersianCalendar c = d as PersianCalendar;

            int i = DateTimeHelper.CompareYearMonth(c.DisplayDateInternal, DateTime.Today);

            if (i > -2 && i < 2)
            {
                c.UpdateCellItems();
            }

        }

        #endregion IsTodayHighlighted

        #region Language
        private static void OnLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                PersianCalendar c = d as PersianCalendar;

                if (DependencyPropertyHelper.GetValueSource(d, PersianCalendar.FirstDayOfWeekProperty).BaseValueSource == BaseValueSource.Default)
                {
                    c.CoerceValue(FirstDayOfWeekProperty);
                    c.UpdateCellItems();
                }
            }
            catch
            {

            }
        }
        #endregion

        #region SelectedDate

        /// <summary>
        /// Gets or sets the currently selected date.
        /// </summary>
        /// 
        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        /// <summary>
        /// Identifies the SelectedDate dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register(
            "SelectedDate",
            typeof(DateTime?),
            typeof(PersianCalendar),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedDateChanged));

        /// <summary>
        /// SelectedDateProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its SelectedDate.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PersianCalendar c = d as PersianCalendar;
            Debug.Assert(c != null);

            if (c.SelectionMode != CalendarSelectionMode.None || e.NewValue == null)
            {
                DateTime? addedDate;

                addedDate = (DateTime?)e.NewValue;
                if (IsValidDateSelection(c, addedDate))
                {
                    if (!addedDate.HasValue)
                    {
                        c.SelectedDates.ClearInternal(true /*fireChangeNotification*/);
                    }
                    else
                    {
                        if (addedDate.HasValue && !(c.SelectedDates.Count > 0 && c.SelectedDates[0] == addedDate.Value))
                        {
                            c.SelectedDates.ClearInternal();
                            c.SelectedDates.Add(addedDate.Value);
                        }
                    }

                    // We update the current date for only the Single mode.For the other modes it automatically gets updated
                    if (c.SelectionMode == CalendarSelectionMode.SingleDate)
                    {
                        if (addedDate.HasValue)
                        {
                            c.CurrentDate = addedDate.Value;
                        }
                        c.UpdateCellItems();
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException("d", SR.Get(SRID.Calendar_OnSelectedDateChanged_InvalidValue));
                }
            }
            else
            {
                throw new InvalidOperationException(SR.Get(SRID.Calendar_OnSelectedDateChanged_InvalidOperation));
            }

        }

        public string GetSelectedDateToGregorianDate()
        {
            GregorianCalendar pc = new GregorianCalendar();
            if (SelectedDate != null)
            {
                return pc.GetYear(SelectedDate.Value) + "/" + pc.GetMonth(SelectedDate.Value).ToString("00") + "/" + pc.GetDayOfMonth(SelectedDate.Value).ToString("00");
            }
            else
            {
                return pc.GetYear(CurrentDate) + "/" + pc.GetMonth(CurrentDate).ToString("00") + "/" + pc.GetDayOfMonth(CurrentDate).ToString("00");
            }
        }

        public string SelectedDateToString()
        {
            return string.Format("{0:yyyy/MM/dd}", _currentDate);
        }
        #endregion SelectedDate

        #region SelectedDates

        // TODO: Should it be of type ObservableCollection?

        /// <summary>
        /// Gets the dates that are currently selected.
        /// </summary>
        public SelectedDatesCollection SelectedDates => _selectedDates;

        #endregion SelectedDates

        #region SelectionMode

        /// <summary>
        /// Gets or sets the selection mode for the calendar.
        /// </summary>
        public CalendarSelectionMode SelectionMode
        {
            get => (CalendarSelectionMode)GetValue(SelectionModeProperty);
            set => SetValue(SelectionModeProperty, value);
        }

        /// <summary>
        /// Identifies the SelectionMode dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(
            "SelectionMode",
            typeof(CalendarSelectionMode),
            typeof(PersianCalendar),
            new FrameworkPropertyMetadata(CalendarSelectionMode.SingleDate, OnSelectionModeChanged),
            new ValidateValueCallback(IsValidSelectionMode));

        private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PersianCalendar c = d as PersianCalendar;
            Debug.Assert(c != null);

            c.HoverStart = c.HoverEnd = null;
            c.SelectedDates.ClearInternal(true /*fireChangeNotification*/);
            c.OnSelectionModeChanged(EventArgs.Empty);
        }

        #endregion SelectionMode

        #endregion Public Properties

        #region Internal Events

        internal event MouseButtonEventHandler DayButtonMouseUp;

        internal event RoutedEventHandler DayOrMonthPreviewKeyDown;

        #endregion Internal Events

        #region Internal Properties

        /// <summary>
        /// This flag is used to determine whether DatePicker should change its 
        /// DisplayDate because of a SelectedDate change on its Calendar
        /// </summary>
        internal bool DatePickerDisplayDateFlag
        {
            get;
            set;
        }

        internal DateTime DisplayDateInternal
        {
            get;
            private set;
        }

        internal DateTime DisplayDateEndInternal => DisplayDateEnd.GetValueOrDefault(PersianCalendarHelper.GetCurrentCalendar().MaxSupportedDateTime);

        internal DateTime DisplayDateStartInternal => DisplayDateStart.GetValueOrDefault(PersianCalendarHelper.GetCurrentCalendar().MinSupportedDateTime);

        internal DateTime CurrentDate
        {
            get => _currentDate.GetValueOrDefault(DisplayDateInternal);
            set => _currentDate = value;
        }

        internal DateTime? HoverStart
        {
            get => SelectionMode == CalendarSelectionMode.None ? null : _hoverStart;

            set => _hoverStart = value;
        }

        internal DateTime? HoverEnd
        {
            get => SelectionMode == CalendarSelectionMode.None ? null : _hoverEnd;

            set => _hoverEnd = value;
        }

        internal CalendarItem MonthControl => _monthControl;

        internal DateTime DisplayMonth => DateTimeHelper.DiscardDayTime(DisplayDate);

        internal DateTime DisplayYear =>
                //return new DateTime(DisplayDate.Year, 1, 1);
                DateTimeHelper.DiscardMonthDayTime(DisplayDate);

        #endregion Internal Properties

        #region Private Properties

        private bool IsRightToLeft => FlowDirection == FlowDirection.RightToLeft;
        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Invoked whenever application code or an internal process, 
        /// such as a rebuilding layout pass, calls the ApplyTemplate method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (_monthControl != null)
            {
                _monthControl.Owner = null;
            }

            base.OnApplyTemplate();

            _monthControl = GetTemplateChild(ElementMonth) as CalendarItem;

            if (_monthControl != null)
            {
                _monthControl.Owner = this;
            }

            CurrentDate = DisplayDate;
            UpdateCellItems();
        }

        /// <summary>
        /// Provides a text representation of the selected date.
        /// </summary>
        /// <returns>A text representation of the selected date, or an empty string if SelectedDate is a null reference.</returns>
        public override string ToString()
        {
            if (SelectedDate != null)
            {
                return SelectedDate.Value.ToString(DateTimeHelper.GetDateFormat(DateTimeHelper.GetCulture(this)));
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void OnSelectedDatesChanged(SelectionChangedEventArgs e)
        {
            RaiseEvent(e);

        }

        protected virtual void OnDisplayDateChanged(CalendarDateChangedEventArgs e)
        {
            EventHandler<CalendarDateChangedEventArgs> handler = DisplayDateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnDisplayModeChanged(CalendarModeChangedEventArgs e)
        {
            EventHandler<CalendarModeChangedEventArgs> handler = DisplayModeChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnSelectionModeChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = SelectionModeChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Creates the automation peer for this Calendar Control.
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CalendarAutomationPeer(this);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = ProcessCalendarKey(e);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (!e.Handled)
            {
                if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                {
                    ProcessShiftKeyUp();
                }
            }
        }

        #endregion Protected Methods

        #region Internal Methods

        internal CalendarDayButton FindDayButtonFromDay(DateTime day)
        {
            if (MonthControl != null)
            {
                foreach (CalendarDayButton b in MonthControl.GetCalendarDayButtons())
                {
                    if (b.DataContext is DateTime)
                    {
                        if (DateTimeHelper.CompareDays((DateTime)b.DataContext, day) == 0)
                        {
                            return b;
                        }
                    }
                }
            }

            return null;
        }

        internal static bool IsValidDateSelection(PersianCalendar cal, object value)
        {
            return (value == null) || (!cal.BlackoutDates.Contains((DateTime)value));
        }

        internal void OnDayButtonMouseUp(MouseButtonEventArgs e)
        {
            MouseButtonEventHandler handler = DayButtonMouseUp;
            if (null != handler)
            {
                handler(this, e);
            }
        }

        internal void OnDayOrMonthPreviewKeyDown(RoutedEventArgs e)
        {
            RoutedEventHandler handler = DayOrMonthPreviewKeyDown;
            if (null != handler)
            {
                handler(this, e);
            }
        }

        // If the day is a trailing day, Update the DisplayDate
        internal void OnDayClick(DateTime selectedDate)
        {
            if (SelectionMode == CalendarSelectionMode.None)
            {
                CurrentDate = selectedDate;
            }

            if (DateTimeHelper.CompareYearMonth(selectedDate, DisplayDateInternal) != 0)
            {
                MoveDisplayTo(selectedDate);
            }
            else
            {
                UpdateCellItems();
                FocusDate(selectedDate);
            }
        }

        internal void OnCalendarButtonPressed(CalendarButton b, bool switchDisplayMode)
        {
            if (b.DataContext is DateTime)
            {
                DateTime d = (DateTime)b.DataContext;

                DateTime? newDate = null;
                CalendarMode newMode = CalendarMode.Month;

                switch (DisplayMode)
                {
                    case CalendarMode.Month:
                        {
                            Debug.Assert(false);
                            break;
                        }

                    case CalendarMode.Year:
                        {
                            newDate = DateTimeHelper.SetYearMonth(DisplayDate, d);
                            newMode = CalendarMode.Month;
                            break;
                        }

                    case CalendarMode.Decade:
                        {
                            newDate = DateTimeHelper.SetYear(DisplayDate, d);
                            newMode = CalendarMode.Year;
                            break;
                        }

                    default:
                        Debug.Assert(false);
                        break;
                }

                if (newDate.HasValue)
                {
                    DisplayDate = newDate.Value;
                    if (switchDisplayMode)
                    {
                        DisplayMode = newMode;
                        FocusDate(DisplayMode == CalendarMode.Month ? CurrentDate : DisplayDate);
                    }
                }
            }
        }

        private DateTime? GetDateOffset(DateTime date, int offset, CalendarMode displayMode)
        {
            DateTime? result = null;
            switch (displayMode)
            {
                case CalendarMode.Month:
                    {
                        result = DateTimeHelper.AddMonths(date, offset);
                        break;
                    }

                case CalendarMode.Year:
                    {
                        result = DateTimeHelper.AddYears(date, offset);
                        break;
                    }

                case CalendarMode.Decade:
                    {
                        result = DateTimeHelper.AddYears(DisplayDate, offset * YEARS_PER_DECADE);
                        break;
                    }

                default:
                    Debug.Assert(false);
                    break;
            }

            return result;
        }

        private void MoveDisplayTo(DateTime? date)
        {
            if (date.HasValue)
            {
                DateTime d = date.Value.Date;
                switch (DisplayMode)
                {
                    case CalendarMode.Month:
                        {
                            DisplayDate = DateTimeHelper.DiscardDayTime(d);
                            CurrentDate = d;
                            UpdateCellItems();

                            break;
                        }

                    case CalendarMode.Year:
                    case CalendarMode.Decade:
                        {
                            DisplayDate = d;
                            UpdateCellItems();

                            break;
                        }

                    default:
                        Debug.Assert(false);
                        break;
                }

                FocusDate(d);
            }
        }

        internal void OnNextClick()
        {
            DateTime? nextDate = GetDateOffset(DisplayDate, 1, DisplayMode);
            if (nextDate.HasValue)
            {
                MoveDisplayTo(DateTimeHelper.DiscardDayTime(nextDate.Value));
            }
        }

        internal void OnPreviousClick()
        {
            DateTime? nextDate = GetDateOffset(DisplayDate, -1, DisplayMode);
            if (nextDate.HasValue)
            {
                MoveDisplayTo(DateTimeHelper.DiscardDayTime(nextDate.Value));
            }
        }

        internal void OnSelectedDatesCollectionChanged(SelectionChangedEventArgs e)
        {
            if (IsSelectionChanged(e))
            {
                if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected) ||
                    AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection) ||
                    AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
                {
                    CalendarAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this) as CalendarAutomationPeer;
                    if (peer != null)
                    {
                        peer.RaiseSelectionEvents(e);
                    }
                }

                CoerceFromSelection();
                OnSelectedDatesChanged(e);
            }
        }

        internal void UpdateCellItems()
        {
            CalendarItem monthControl = MonthControl;
            if (monthControl != null)
            {
                switch (DisplayMode)
                {
                    case CalendarMode.Month:
                        {
                            monthControl.UpdateMonthMode();
                            break;
                        }

                    case CalendarMode.Year:
                        {
                            monthControl.UpdateYearMode();
                            break;
                        }

                    case CalendarMode.Decade:
                        {
                            monthControl.UpdateDecadeMode();
                            break;
                        }

                    default:
                        Debug.Assert(false);
                        break;
                }
            }
        }

        #endregion Internal Methods

        #region Private Methods

        private void CoerceFromSelection()
        {
            CoerceValue(DisplayDateStartProperty);
            CoerceValue(DisplayDateEndProperty);
            CoerceValue(DisplayDateProperty);
        }

        // This method adds the days that were selected by Keyboard to the SelectedDays Collection 
        private void AddKeyboardSelection()
        {
            if (HoverStart != null)
            {
                SelectedDates.ClearInternal();

                // In keyboard selection, we are sure that the collection does not include any blackout days
                SelectedDates.AddRange(HoverStart.Value, CurrentDate);
            }
        }

        private static bool IsSelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != e.RemovedItems.Count)
            {
                return true;
            }

            foreach (DateTime addedDate in e.AddedItems)
            {
                if (!e.RemovedItems.Contains(addedDate))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsValidDisplayMode(object value)
        {
            CalendarMode mode = (CalendarMode)value;

            return mode == CalendarMode.Month
                || mode == CalendarMode.Year
                || mode == CalendarMode.Decade;
        }

        internal static bool IsValidFirstDayOfWeek(object value)
        {
            DayOfWeek day = (DayOfWeek)value;

            return day == DayOfWeek.Sunday
                || day == DayOfWeek.Monday
                || day == DayOfWeek.Tuesday
                || day == DayOfWeek.Wednesday
                || day == DayOfWeek.Thursday
                || day == DayOfWeek.Friday
                || day == DayOfWeek.Saturday;
        }

        private static bool IsValidKeyboardSelection(PersianCalendar cal, object value)
        {
            if (value == null)
            {
                return true;
            }
            else
            {
                if (cal.BlackoutDates.Contains((DateTime)value))
                {
                    return false;
                }
                else
                {
                    return DateTime.Compare((DateTime)value, cal.DisplayDateStartInternal) >= 0 && DateTime.Compare((DateTime)value, cal.DisplayDateEndInternal) <= 0;
                }
            }
        }

        private static bool IsValidSelectionMode(object value)
        {
            CalendarSelectionMode mode = (CalendarSelectionMode)value;

            return mode == CalendarSelectionMode.SingleDate
                || mode == CalendarSelectionMode.SingleRange
                || mode == CalendarSelectionMode.MultipleRange
                || mode == CalendarSelectionMode.None;
        }

        private void OnSelectedMonthChanged(DateTime? selectedMonth)
        {
            if (selectedMonth.HasValue)
            {
                Debug.Assert(DisplayMode == CalendarMode.Year);
                DisplayDate = selectedMonth.Value;

                UpdateCellItems();

                FocusDate(selectedMonth.Value);
            }
        }

        private void OnSelectedYearChanged(DateTime? selectedYear)
        {
            if (selectedYear.HasValue)
            {
                Debug.Assert(DisplayMode == CalendarMode.Decade);
                DisplayDate = selectedYear.Value;

                UpdateCellItems();

                FocusDate(selectedYear.Value);
            }
        }

        internal void FocusDate(DateTime date)
        {
            if (MonthControl != null)
            {
                MonthControl.FocusDate(date);
            }
        }

        /// <summary>
        ///     Called when this element gets focus.
        /// </summary>
        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            // When Calendar gets focus move it to the DisplayDate
            PersianCalendar c = (PersianCalendar)sender;
            if (!e.Handled && e.OriginalSource == c)
            {
                // This check is for the case where the DisplayDate is the first of the month
                // and the SelectedDate is in the middle of the month.  If you tab into the Calendar
                // the focus should go to the SelectedDate, not the DisplayDate.
                if (c.SelectedDate.HasValue && DateTimeHelper.CompareYearMonth(c.SelectedDate.Value, c.DisplayDateInternal) == 0)
                {
                    c.FocusDate(c.SelectedDate.Value);
                }
                else
                {
                    c.FocusDate(c.DisplayDate);
                }

                e.Handled = true;
            }


        }

        private bool ProcessCalendarKey(KeyEventArgs e)
        {
            if (DisplayMode == CalendarMode.Month)
            {
                // If a blackout day is inactive, when clicked on it, the previous inactive day which is not a blackout day can get the focus.
                // In this case we should allow keyboard functions on that inactive day
                CalendarDayButton currentDayButton = (MonthControl != null) ? MonthControl.GetCalendarDayButton(CurrentDate) : null;

                if (DateTimeHelper.CompareYearMonth(CurrentDate, DisplayDateInternal) != 0 && currentDayButton != null && !currentDayButton.IsInactive)
                {
                    return false;
                }
            }

            KeyboardHelper.GetMetaKeyState(out bool ctrl, out bool shift);

            switch (e.Key)
            {
                case Key.Up:
                    {
                        ProcessUpKey(ctrl, shift);
                        return true;
                    }

                case Key.Down:
                    {
                        ProcessDownKey(ctrl, shift);
                        return true;
                    }

                case Key.Left:
                    {
                        ProcessLeftKey(shift);
                        return true;
                    }

                case Key.Right:
                    {
                        ProcessRightKey(shift);
                        return true;
                    }

                case Key.PageDown:
                    {
                        ProcessPageDownKey(shift);
                        return true;
                    }

                case Key.PageUp:
                    {
                        ProcessPageUpKey(shift);
                        return true;
                    }

                case Key.Home:
                    {
                        ProcessHomeKey(shift);
                        return true;
                    }

                case Key.End:
                    {
                        ProcessEndKey(shift);
                        return true;
                    }

                case Key.Enter:
                case Key.Space:
                    {
                        return ProcessEnterKey();
                    }
            }

            return false;
        }

        private void ProcessDownKey(bool ctrl, bool shift)
        {
            switch (DisplayMode)
            {
                case CalendarMode.Month:
                    {
                        if (!ctrl || shift)
                        {
                            DateTime? selectedDate = _blackoutDates.GetNonBlackoutDate(DateTimeHelper.AddDays(CurrentDate, COLS), 1);
                            ProcessSelection(shift, selectedDate);
                        }

                        break;
                    }

                case CalendarMode.Year:
                    {
                        if (ctrl)
                        {
                            DisplayMode = CalendarMode.Month;
                            FocusDate(DisplayDate);
                        }
                        else
                        {
                            DateTime? selectedMonth = DateTimeHelper.AddMonths(DisplayDate, YEAR_COLS);
                            OnSelectedMonthChanged(selectedMonth);
                        }

                        break;
                    }

                case CalendarMode.Decade:
                    {
                        if (ctrl)
                        {
                            DisplayMode = CalendarMode.Year;
                            FocusDate(DisplayDate);
                        }
                        else
                        {
                            DateTime? selectedYear = DateTimeHelper.AddYears(DisplayDate, YEAR_COLS);
                            OnSelectedYearChanged(selectedYear);
                        }

                        break;
                    }
            }
        }

        private void ProcessEndKey(bool shift)
        {
            switch (DisplayMode)
            {
                case CalendarMode.Month:
                    {
                        if (DisplayDate != null)
                        {
                            //DateTime? selectedDate = new DateTime(this.DisplayDateInternal.Year, this.DisplayDateInternal.Month, 1);
                            DateTime? selectedDate = DateTimeHelper.DiscardDayTime(DisplayDateInternal);

                            if (DateTimeHelper.CompareYearMonth(DateTime.MaxValue, selectedDate.Value) > 0)
                            {
                                // since DisplayDate is not equal to DateTime.MaxValue we are sure selectedDate is not null
                                selectedDate = DateTimeHelper.AddMonths(selectedDate.Value, 1).Value;
                                selectedDate = DateTimeHelper.AddDays(selectedDate.Value, -1).Value;
                            }
                            else
                            {
                                selectedDate = DateTime.MaxValue;
                            }

                            ProcessSelection(shift, selectedDate);
                        }

                        break;
                    }

                case CalendarMode.Year:
                    {
                        //DateTime selectedMonth = new DateTime(this.DisplayDate.Year, 12, 1);
                        DateTime selectedMonth = DateTimeHelper.GetLastMonth(DisplayDate);
                        OnSelectedMonthChanged(selectedMonth);
                        break;
                    }

                case CalendarMode.Decade:
                    {
                        DateTime? selectedYear = DateTimeHelper.EndOfDecade(DisplayDate);
                        OnSelectedYearChanged(selectedYear);
                        break;
                    }
            }
        }

        private bool ProcessEnterKey()
        {
            switch (DisplayMode)
            {
                case CalendarMode.Year:
                    {
                        DisplayMode = CalendarMode.Month;
                        FocusDate(DisplayDate);
                        return true;
                    }

                case CalendarMode.Decade:
                    {
                        DisplayMode = CalendarMode.Year;
                        FocusDate(DisplayDate);
                        return true;
                    }
            }

            return false;
        }

        private void ProcessHomeKey(bool shift)
        {
            switch (DisplayMode)
            {
                case CalendarMode.Month:
                    {
                        // TODO: Not all types of calendars start with Day1. If Non-Gregorian is supported check this:
                        //DateTime? selectedDate = new DateTime(this.DisplayDateInternal.Year, this.DisplayDateInternal.Month, 1);
                        DateTime? selectedDate = DateTimeHelper.DiscardDayTime(DisplayDateInternal);
                        ProcessSelection(shift, selectedDate);
                        break;
                    }

                case CalendarMode.Year:
                    {
                        //DateTime selectedMonth = new DateTime(this.DisplayDate.Year, 1, 1);
                        DateTime selectedMonth = DateTimeHelper.DiscardMonthDayTime(DisplayDate);
                        OnSelectedMonthChanged(selectedMonth);
                        break;
                    }

                case CalendarMode.Decade:
                    {
                        DateTime? selectedYear = DateTimeHelper.DecadeOfDate(DisplayDate);
                        OnSelectedYearChanged(selectedYear);
                        break;
                    }
            }
        }

        private void ProcessLeftKey(bool shift)
        {
            int moveAmmount = (!IsRightToLeft) ? -1 : 1;
            switch (DisplayMode)
            {
                case CalendarMode.Month:
                    {
                        DateTime? selectedDate = _blackoutDates.GetNonBlackoutDate(DateTimeHelper.AddDays(CurrentDate, moveAmmount), moveAmmount);
                        ProcessSelection(shift, selectedDate);
                        break;
                    }

                case CalendarMode.Year:
                    {
                        DateTime? selectedMonth = DateTimeHelper.AddMonths(DisplayDate, moveAmmount);
                        OnSelectedMonthChanged(selectedMonth);
                        break;
                    }

                case CalendarMode.Decade:
                    {
                        DateTime? selectedYear = DateTimeHelper.AddYears(DisplayDate, moveAmmount);
                        OnSelectedYearChanged(selectedYear);
                        break;
                    }
            }
        }

        private void ProcessPageDownKey(bool shift)
        {
            switch (DisplayMode)
            {
                case CalendarMode.Month:
                    {
                        DateTime? selectedDate = _blackoutDates.GetNonBlackoutDate(DateTimeHelper.AddMonths(CurrentDate, 1), 1);
                        ProcessSelection(shift, selectedDate);
                        break;
                    }

                case CalendarMode.Year:
                    {
                        DateTime? selectedMonth = DateTimeHelper.AddYears(DisplayDate, 1);
                        OnSelectedMonthChanged(selectedMonth);
                        break;
                    }

                case CalendarMode.Decade:
                    {
                        DateTime? selectedYear = DateTimeHelper.AddYears(DisplayDate, 10);
                        OnSelectedYearChanged(selectedYear);
                        break;
                    }
            }
        }

        private void ProcessPageUpKey(bool shift)
        {
            switch (DisplayMode)
            {
                case CalendarMode.Month:
                    {
                        DateTime? selectedDate = _blackoutDates.GetNonBlackoutDate(DateTimeHelper.AddMonths(CurrentDate, -1), -1);
                        ProcessSelection(shift, selectedDate);
                        break;
                    }

                case CalendarMode.Year:
                    {
                        DateTime? selectedMonth = DateTimeHelper.AddYears(DisplayDate, -1);
                        OnSelectedMonthChanged(selectedMonth);
                        break;
                    }

                case CalendarMode.Decade:
                    {
                        DateTime? selectedYear = DateTimeHelper.AddYears(DisplayDate, -10);
                        OnSelectedYearChanged(selectedYear);
                        break;
                    }
            }
        }

        private void ProcessRightKey(bool shift)
        {
            int moveAmmount = (!IsRightToLeft) ? 1 : -1;
            switch (DisplayMode)
            {
                case CalendarMode.Month:
                    {
                        DateTime? selectedDate = _blackoutDates.GetNonBlackoutDate(DateTimeHelper.AddDays(CurrentDate, moveAmmount), moveAmmount);
                        ProcessSelection(shift, selectedDate);
                        break;
                    }

                case CalendarMode.Year:
                    {
                        DateTime? selectedMonth = DateTimeHelper.AddMonths(DisplayDate, moveAmmount);
                        OnSelectedMonthChanged(selectedMonth);
                        break;
                    }

                case CalendarMode.Decade:
                    {
                        DateTime? selectedYear = DateTimeHelper.AddYears(DisplayDate, moveAmmount);
                        OnSelectedYearChanged(selectedYear);
                        break;
                    }
            }
        }

        private void ProcessSelection(bool shift, DateTime? lastSelectedDate)
        {
            if (SelectionMode == CalendarSelectionMode.None && lastSelectedDate != null)
            {
                OnDayClick(lastSelectedDate.Value);
                return;
            }

            if (lastSelectedDate != null && IsValidKeyboardSelection(this, lastSelectedDate.Value))
            {
                if (SelectionMode == CalendarSelectionMode.SingleRange || SelectionMode == CalendarSelectionMode.MultipleRange)
                {
                    SelectedDates.ClearInternal();
                    if (shift)
                    {
                        _isShiftPressed = true;
                        if (!HoverStart.HasValue)
                        {
                            HoverStart = HoverEnd = CurrentDate;
                        }

                        // If we hit a BlackOutDay with keyboard we do not update the HoverEnd
                        CalendarDateRange range;

                        if (DateTime.Compare(HoverStart.Value, lastSelectedDate.Value) < 0)
                        {
                            range = new CalendarDateRange(HoverStart.Value, lastSelectedDate.Value);
                        }
                        else
                        {
                            range = new CalendarDateRange(lastSelectedDate.Value, HoverStart.Value);
                        }

                        if (!BlackoutDates.ContainsAny(range))
                        {
                            _currentDate = lastSelectedDate;
                            HoverEnd = lastSelectedDate;
                        }

                        OnDayClick(CurrentDate);
                    }
                    else
                    {
                        HoverStart = HoverEnd = CurrentDate = lastSelectedDate.Value;
                        AddKeyboardSelection();
                        OnDayClick(lastSelectedDate.Value);
                    }
                }
                else
                {
                    // ON CLEAR 
                    CurrentDate = lastSelectedDate.Value;
                    HoverStart = HoverEnd = null;
                    if (SelectedDates.Count > 0)
                    {
                        SelectedDates[0] = lastSelectedDate.Value;
                    }
                    else
                    {
                        SelectedDates.Add(lastSelectedDate.Value);
                    }

                    OnDayClick(lastSelectedDate.Value);
                }

                UpdateCellItems();
            }
        }

        private void ProcessShiftKeyUp()
        {
            if (_isShiftPressed && (SelectionMode == CalendarSelectionMode.SingleRange || SelectionMode == CalendarSelectionMode.MultipleRange))
            {
                AddKeyboardSelection();
                _isShiftPressed = false;
                HoverStart = HoverEnd = null;
            }
        }

        private void ProcessUpKey(bool ctrl, bool shift)
        {
            switch (DisplayMode)
            {
                case CalendarMode.Month:
                    {
                        if (ctrl)
                        {
                            DisplayMode = CalendarMode.Year;
                            FocusDate(DisplayDate);
                        }
                        else
                        {
                            DateTime? selectedDate = _blackoutDates.GetNonBlackoutDate(DateTimeHelper.AddDays(CurrentDate, -COLS), -1);
                            ProcessSelection(shift, selectedDate);
                        }

                        break;
                    }

                case CalendarMode.Year:
                    {
                        if (ctrl)
                        {
                            DisplayMode = CalendarMode.Decade;
                            FocusDate(DisplayDate);
                        }
                        else
                        {
                            DateTime? selectedMonth = DateTimeHelper.AddMonths(DisplayDate, -YEAR_COLS);
                            OnSelectedMonthChanged(selectedMonth);
                        }

                        break;
                    }

                case CalendarMode.Decade:
                    {
                        if (!ctrl)
                        {
                            DateTime? selectedYear = DateTimeHelper.AddYears(DisplayDate, -YEAR_COLS);
                            OnSelectedYearChanged(selectedYear);
                        }

                        break;
                    }
            }
        }

        #endregion Private Methods
    }
}
