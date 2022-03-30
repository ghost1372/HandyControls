//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using PersianCalendar = HandyControl.Controls.PersianCalendar;

namespace Microsoft.Windows.Controls.Primitives
{
    [TemplatePart(Name = CalendarItem.ElementRoot, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = CalendarItem.ElementHeaderButton, Type = typeof(Button))]
    [TemplatePart(Name = CalendarItem.ElementPreviousButton, Type = typeof(Button))]
    [TemplatePart(Name = CalendarItem.ElementNextButton, Type = typeof(Button))]
    [TemplatePart(Name = CalendarItem.ElementDayTitleTemplate, Type = typeof(DataTemplate))]
    [TemplatePart(Name = CalendarItem.ElementMonthView, Type = typeof(Grid))]
    [TemplatePart(Name = CalendarItem.ElementYearView, Type = typeof(Grid))]
    [TemplatePart(Name = CalendarItem.ElementDisabledVisual, Type = typeof(FrameworkElement))]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    public sealed partial class CalendarItem : Control
    {
        #region Constants
        private const string ElementRoot = "PART_Root";
        private const string ElementHeaderButton = "PART_HeaderButton";
        private const string ElementPreviousButton = "PART_PreviousButton";
        private const string ElementNextButton = "PART_NextButton";
        private const string ElementDayTitleTemplate = "DayTitleTemplate";
        private const string ElementMonthView = "PART_MonthView";
        private const string ElementYearView = "PART_YearView";
        private const string ElementDisabledVisual = "PART_DisabledVisual";

        private const int COLS = 7;
        private const int ROWS = 7;
        private const int YEAR_COLS = 4;
        private const int YEAR_ROWS = 3;
        private const int NUMBER_OF_DAYS_IN_WEEK = 7;

        #endregion Constants

        #region Data

        private System.Globalization.Calendar _calendar = PersianCalendarHelper.GetCurrentCalendar();
        private DataTemplate _dayTitleTemplate;
        private FrameworkElement _disabledVisual;
        private Button _headerButton;
        private Grid _monthView;
        private Button _nextButton;
        private Button _previousButton;
        private Grid _yearView;
        private bool _isMonthPressed;
        private bool _isDayPressed;

        #endregion Data

        static CalendarItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarItem), new FrameworkPropertyMetadata(typeof(CalendarItem)));
            FocusableProperty.OverrideMetadata(typeof(CalendarItem), new FrameworkPropertyMetadata(false));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(CalendarItem), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(CalendarItem), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));
        }

        /// <summary>
        /// Represents the month that is used in PersianCalendar Control.
        /// </summary>
        public CalendarItem()
        {
        }

        #region Internal Properties

        internal Grid MonthView
        {
            get { return _monthView; }
        }

        internal PersianCalendar Owner
        {
            get;
            set;
        }

        internal Grid YearView
        {
            get { return _yearView; }
        }

        #endregion Internal Properties

        #region Private Properties

        /// <summary>
        /// Gets a value indicating whether the calendar is displayed in months, years or decades.
        /// </summary>
        private CalendarMode DisplayMode
        {
            get
            {
                return (this.Owner != null) ? this.Owner.DisplayMode : CalendarMode.Month;
            }
        }

        private Button HeaderButton
        {
            get
            {
                return this._headerButton;
            }
        }

        private Button NextButton
        {
            get
            {
                return this._nextButton;
            }
        }

        private Button PreviousButton
        {
            get
            {
                return this._previousButton;
            }
        }

        private DateTime DisplayDate
        {
            get
            {
                return (Owner != null) ? Owner.DisplayDate : DateTime.Today;
            }
        }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Invoked whenever application code or an internal process, 
        /// such as a rebuilding layout pass, calls the ApplyTemplate method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this._previousButton != null)
            {
                this._previousButton.Click -= new RoutedEventHandler(PreviousButton_Click);
            }

            if (this._nextButton != null)
            {
                this._nextButton.Click -= new RoutedEventHandler(NextButton_Click);
            }

            if (this._headerButton != null)
            {
                this._headerButton.Click -= new RoutedEventHandler(HeaderButton_Click);
            }

            _monthView = GetTemplateChild(ElementMonthView) as Grid;
            _yearView = GetTemplateChild(ElementYearView) as Grid;
            _previousButton = GetTemplateChild(ElementPreviousButton) as Button;
            _nextButton = GetTemplateChild(ElementNextButton) as Button;
            _headerButton = GetTemplateChild(ElementHeaderButton) as Button;
            _disabledVisual = GetTemplateChild(ElementDisabledVisual) as FrameworkElement;

            // WPF Compat: Unlike SL, WPF is not able to get elements in template resources with GetTemplateChild()
            _dayTitleTemplate = null;
            if (Template != null && Template.Resources.Contains(ElementDayTitleTemplate))
            {
                _dayTitleTemplate = Template.Resources[ElementDayTitleTemplate] as DataTemplate;
            }

            if (this._previousButton != null)
            {
                // If the user does not provide a Content value in template, we provide a helper text that can be used in Accessibility
                // this text is not shown on the UI, just used for Accessibility purposes
                if (this._previousButton.Content == null)
                {
                    this._previousButton.Content = "Previous button";
                }

                this._previousButton.Click += new RoutedEventHandler(PreviousButton_Click);
            }

            if (this._nextButton != null)
            {
                // If the user does not provide a Content value in template, we provide a helper text that can be used in Accessibility
                // this text is not shown on the UI, just used for Accessibility purposes
                if (this._nextButton.Content == null)
                {
                    this._nextButton.Content = "Next button";
                }

                this._nextButton.Click += new RoutedEventHandler(NextButton_Click);
            }

            if (this._headerButton != null)
            {
                this._headerButton.Click += new RoutedEventHandler(HeaderButton_Click);
            }

            PopulateGrids();

            if (this.Owner != null)
            {
                switch (this.Owner.DisplayMode)
                {
                    case CalendarMode.Year: 
                        UpdateYearMode(); 
                        break;
                    case CalendarMode.Decade: 
                        UpdateDecadeMode(); 
                        break;
                    case CalendarMode.Month: 
                        UpdateMonthMode(); 
                        break;

                    default: 
                        Debug.Assert(false); 
                        break;
                }
            }
            else
            {
                UpdateMonthMode();
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (this.IsMouseCaptured)
            {
                this.ReleaseMouseCapture();
            }

            this._isMonthPressed = false;
            this._isDayPressed = false;

            // In Month mode, we may need to end a drag selection even if  the mouse up isn't on the calendar.
            if (!e.Handled &&
                this.Owner.DisplayMode == CalendarMode.Month &&
                this.Owner.HoverEnd.HasValue)
            {
                FinishSelection(this.Owner.HoverEnd.Value);
            }
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);

            if (!this.IsMouseCaptured)
            {
                this._isDayPressed = false;
                this._isMonthPressed = false;
            }
        }

        #endregion

        #region Internal Methods

        internal void UpdateDecadeMode()
        {
            DateTime selectedYear;

            if (this.Owner != null)
            {
                selectedYear = this.Owner.DisplayYear;
            }
            else
            {
                selectedYear = DateTime.Today;
            }

            DateTime decade = GetDecadeForDecadeMode(selectedYear);
            DateTime decadeEnd = _calendar.AddYears(decade, 9);

            SetDecadeModeHeaderButton(decade);
            SetDecadeModePreviousButton(decade);
            SetDecadeModeNextButton(decadeEnd);

            if (_yearView != null)
            {
                SetYearButtons(decade, decadeEnd);
            }
        }

        internal void UpdateMonthMode()
        {
            SetMonthModeHeaderButton();
            SetMonthModePreviousButton();
            SetMonthModeNextButton();

            if (_monthView != null)
            {
                SetMonthModeDayTitles();
                SetMonthModeCalendarDayButtons();
                AddMonthModeHighlight();
            }
        }

        internal void UpdateYearMode()
        {
            SetYearModeHeaderButton();
            SetYearModePreviousButton();
            SetYearModeNextButton();

            if (_yearView != null)
            {
                SetYearModeMonthButtons();
            }
        }

        internal IEnumerable<CalendarDayButton> GetCalendarDayButtons()
        {
            // TODO: should be updated if we support MultiCalendar
            int count = ROWS * COLS;
            if (MonthView != null)
            {
                UIElementCollection dayButtonsHost = MonthView.Children;
                for (int childIndex = COLS; childIndex < count; childIndex++)
                {
                    CalendarDayButton b = dayButtonsHost[childIndex] as CalendarDayButton;
                    if (b != null)
                    {
                        yield return b;
                    }
                }
            }
        }

        internal CalendarDayButton GetFocusedCalendarDayButton()
        {
            foreach (CalendarDayButton b in GetCalendarDayButtons())
            {
                if (b != null && b.IsFocused)
                {
                    return b;
                }
            }

            return null;
        }

        internal CalendarDayButton GetCalendarDayButton(DateTime date)
        {
            foreach (CalendarDayButton b in GetCalendarDayButtons())
            {
                if (b != null && b.DataContext is DateTime)
                {
                    if (DateTimeHelper.CompareDays(date, (DateTime)b.DataContext) == 0)
                    {
                        return b;
                    }
                }
            }

            return null;
        }

        internal CalendarButton GetCalendarButton(DateTime date, CalendarMode mode)
        {
            Debug.Assert(mode != CalendarMode.Month);

            foreach (CalendarButton b in GetCalendarButtons())
            {
                if (b != null && b.DataContext is DateTime)
                {
                    if (mode == CalendarMode.Year)
                    {
                        if (DateTimeHelper.CompareYearMonth(date, (DateTime)b.DataContext) == 0)
                        {
                            return b;
                        }
                    }
                    else
                    {
                        if (_calendar.GetYear(date) == _calendar.GetYear(((DateTime)b.DataContext)))
                        {
                            return b;
                        }
                    }
                }
            }

            return null;
        }

        internal CalendarButton GetFocusedCalendarButton()
        {
            foreach (CalendarButton b in GetCalendarButtons())
            {
                if (b != null && b.IsFocused)
                {
                    return b;
                }
            }

            return null;
        }

        private IEnumerable<CalendarButton> GetCalendarButtons()
        {            
            foreach (UIElement element in this.YearView.Children)
            {
                CalendarButton b = element as CalendarButton;
                if (b != null)
                {
                    yield return b;
                }
            }
        }

        internal void FocusDate(DateTime date)
        {
            FrameworkElement focusTarget = null;

            switch (this.DisplayMode)
            {
                case CalendarMode.Month:
                {
                    focusTarget = GetCalendarDayButton(date);
                    break;
                }

                case CalendarMode.Year:
                case CalendarMode.Decade:
                {
                    focusTarget = GetCalendarButton(date, this.DisplayMode);
                    break;
                }

                default:
                {
                    Debug.Assert(false);
                    break;
                }
            }

            if (focusTarget != null && !focusTarget.IsFocused)
            {
                focusTarget.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }
        }

        #endregion Internal Methods

        #region Private Methods

        private DateTime GetDecadeForDecadeMode(DateTime selectedYear)
        {
            DateTime decade = DateTimeHelper.DecadeOfDate(selectedYear);

            // Adjust the decade value if the mouse move selection is on,
            // such that if first or last year among the children are selected
            // then return the current selected decade as is.
            if (_isMonthPressed && _yearView != null)
            {
                UIElementCollection yearViewChildren = _yearView.Children;
                int count = yearViewChildren.Count;

                if (count > 0)
                {
                    CalendarButton child = yearViewChildren[0] as CalendarButton;
                    if (child != null &&
                        child.DataContext is DateTime &&
                        _calendar.GetYear(((DateTime)child.DataContext)) == _calendar.GetYear(selectedYear))
                    {
                        return _calendar.AddYears(decade, 10);
                    }
                }

                if (count > 1)
                {
                    CalendarButton child = yearViewChildren[count - 1] as CalendarButton;
                    if (child != null &&
                        child.DataContext is DateTime &&
                        ((DateTime)child.DataContext).Year == selectedYear.Year)
                    {
                        return _calendar.AddYears(decade, -10);
                    }
                }
            }
            return decade;
        }

        private void EndDrag(bool ctrl, DateTime selectedDate)
        {
            if (this.Owner != null)
            {
                this.Owner.CurrentDate = selectedDate;

                if (this.Owner.HoverStart.HasValue)
                {
                    if (
                        ctrl &&
                        DateTime.Compare(this.Owner.HoverStart.Value, selectedDate) == 0 &&
                        (Owner.SelectionMode == CalendarSelectionMode.SingleDate || Owner.SelectionMode == CalendarSelectionMode.MultipleRange))
                    {
                        // Ctrl + single click = toggle
                        this.Owner.SelectedDates.Toggle(selectedDate);
                    }
                    else
                    {
                        // this is selection with Mouse, we do not guarantee the range does not include BlackOutDates.
                        // Use the internal AddRange that omits BlackOutDates based on the SelectionMode
                        this.Owner.SelectedDates.AddRangeInternal(this.Owner.HoverStart.Value, selectedDate);
                    }

                    Owner.OnDayClick(selectedDate);
                }
            }
        }


        private void CellOrMonth_PreviewKeyDown(object sender, RoutedEventArgs e)
        {
            Debug.Assert(e != null);

            if (this.Owner == null)
            {
                return;
            }

            this.Owner.OnDayOrMonthPreviewKeyDown(e);
        }

        private void Cell_Clicked(object sender, RoutedEventArgs e)
        {
            if (this.Owner == null)
            {
                return;
            }

            CalendarDayButton b = sender as CalendarDayButton;
            Debug.Assert(b != null);

            if (!(b.DataContext is DateTime))
            {
                return;
            }

            // If the day is a blackout day selection is not allowed
            if (!b.IsBlackedOut)
            {
                DateTime clickedDate = (DateTime)b.DataContext;
                bool ctrl, shift;

                KeyboardHelper.GetMetaKeyState(out ctrl, out shift);

                switch (this.Owner.SelectionMode)
                {
                    case CalendarSelectionMode.None:
                    {
                        break;
                    }

                    case CalendarSelectionMode.SingleDate:
                    {
                        if (!ctrl)
                        {
                            this.Owner.SelectedDate = clickedDate;
                        }
                        else
                        {
                            this.Owner.SelectedDates.Toggle(clickedDate);
                        }

                        break;
                    }

                    case CalendarSelectionMode.SingleRange:
                        {
                            DateTime? lastDate = this.Owner.CurrentDate;
                            this.Owner.SelectedDates.ClearInternal(true /*fireChangeNotification*/);
                            if (shift && lastDate.HasValue)
                            {
                                this.Owner.SelectedDates.AddRangeInternal(lastDate.Value, clickedDate);
                            }
                            else
                            {
                                this.Owner.SelectedDate = clickedDate;
                                this.Owner.HoverStart = null;
                                this.Owner.HoverEnd = null;
                            }

                            break;
                        }

                    case CalendarSelectionMode.MultipleRange:
                        {
                            if (!ctrl)
                            {
                                this.Owner.SelectedDates.ClearInternal(true /*fireChangeNotification*/);
                            }

                            if (shift)
                            {
                                this.Owner.SelectedDates.AddRangeInternal(this.Owner.CurrentDate, clickedDate);
                            }
                            else
                            {
                                if (!ctrl)
                                {
                                    this.Owner.SelectedDate = clickedDate;
                                }
                                else
                                {
                                    this.Owner.SelectedDates.Toggle(clickedDate);
                                    this.Owner.HoverStart = null;
                                    this.Owner.HoverEnd = null;
                                }
                            }

                            break;
                        }
                }

                this.Owner.OnDayClick(clickedDate);
            }
        }

        private void Cell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CalendarDayButton b = sender as CalendarDayButton;

            if (b == null)
            {
                return;
            }

            if (this.Owner == null || !(b.DataContext is DateTime))
            {
                return;
            }

            if (b.IsBlackedOut)
            {
                this.Owner.HoverStart = null;
            }
            else
            {
                this._isDayPressed = true;
                Mouse.Capture(this, CaptureMode.SubTree);

                b.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

                bool ctrl, shift;
                KeyboardHelper.GetMetaKeyState(out ctrl, out shift);

                DateTime selectedDate = (DateTime)b.DataContext;
                Debug.Assert(selectedDate != null);

                switch (this.Owner.SelectionMode)
                {
                    case CalendarSelectionMode.None:
                    {
                        break;
                    }

                    case CalendarSelectionMode.SingleDate:
                    {
                        this.Owner.DatePickerDisplayDateFlag = true;
                        if (!ctrl)
                        {
                            this.Owner.SelectedDate = selectedDate;
                        }
                        else
                        {
                            this.Owner.SelectedDates.Toggle(selectedDate);
                        }

                        break;
                    }

                    case CalendarSelectionMode.SingleRange:
                    {
                        this.Owner.SelectedDates.ClearInternal();

                        if (shift)
                        {
                            if (!this.Owner.HoverStart.HasValue)
                            {
                                this.Owner.HoverStart = this.Owner.HoverEnd = this.Owner.CurrentDate;
                            }
                        }
                        else
                        {
                            this.Owner.HoverStart = this.Owner.HoverEnd = selectedDate;
                        }

                        break;
                    }

                    case CalendarSelectionMode.MultipleRange:
                    {
                        if (!ctrl)
                        {
                            this.Owner.SelectedDates.ClearInternal();
                        }

                        if (shift)
                        {
                            if (!this.Owner.HoverStart.HasValue)
                            {
                                this.Owner.HoverStart = this.Owner.HoverEnd = this.Owner.CurrentDate;
                            }
                        }
                        else
                        {
                            this.Owner.HoverStart = this.Owner.HoverEnd = selectedDate;
                        }

                        break;
                    }
                }

                this.Owner.CurrentDate = selectedDate;
                this.Owner.UpdateCellItems();
            }
        }

        private void Cell_MouseEnter(object sender, MouseEventArgs e)
        {
            CalendarDayButton b = sender as CalendarDayButton;
            if (b == null)
            {
                return;
            }

            if (b.IsBlackedOut)
            {
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed && this._isDayPressed)
            {
                b.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

                if (this.Owner == null || !(b.DataContext is DateTime))
                {
                    return;
                }

                DateTime selectedDate = (DateTime)b.DataContext;

                switch (this.Owner.SelectionMode)
                {
                    case CalendarSelectionMode.SingleDate:
                    {
                        this.Owner.DatePickerDisplayDateFlag = true;
                        this.Owner.HoverStart = this.Owner.HoverEnd = null;
                        if (this.Owner.SelectedDates.Count == 0)
                        {
                            this.Owner.SelectedDates.Add(selectedDate);
                        }
                        else
                        {
                            this.Owner.SelectedDates[0] = selectedDate;
                        }

                        return;
                    }
                }

                this.Owner.HoverEnd = selectedDate;
                this.Owner.CurrentDate = selectedDate;
                this.Owner.UpdateCellItems();
            }
        }

        private void Cell_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CalendarDayButton b = sender as CalendarDayButton;
            if (b == null)
            {
                return;
            }

            if (this.Owner == null)
            {
                return;
            }

            if (!b.IsBlackedOut)
            {
                this.Owner.OnDayButtonMouseUp(e);
            }

            if (!(b.DataContext is DateTime))
            {
                return;
            }

            FinishSelection((DateTime)b.DataContext);
            e.Handled = true;
        }

        private void FinishSelection(DateTime selectedDate)
        {
            bool ctrl, shift;
            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);

            if (this.Owner.SelectionMode == CalendarSelectionMode.None || this.Owner.SelectionMode == CalendarSelectionMode.SingleDate)
            {
                this.Owner.OnDayClick(selectedDate);
                return;
            }

            if (this.Owner.HoverStart.HasValue)
            {
                switch (this.Owner.SelectionMode)
                {
                    case CalendarSelectionMode.SingleRange:
                    {
                        // Update SelectedDates
                        this.Owner.SelectedDates.ClearInternal();
                        EndDrag(ctrl, selectedDate);
                        break;
                    }

                    case CalendarSelectionMode.MultipleRange:
                    {
                        // add the selection (either single day or SingleRange day)
                        EndDrag(ctrl, selectedDate);
                        break;
                    }
                }
            }
            else
            {
                // If the day is blacked out but also a trailing day we should be able to switch months
                CalendarDayButton b = GetCalendarDayButton(selectedDate);
                if (b != null && b.IsInactive && b.IsBlackedOut)
                {
                    this.Owner.OnDayClick(selectedDate);
                }
            }
        }

        private void Month_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CalendarButton b = sender as CalendarButton;
            if (b != null)
            {
                this._isMonthPressed = true;
                Mouse.Capture(this, CaptureMode.SubTree);

                if (this.Owner != null)
                {
                    this.Owner.OnCalendarButtonPressed(b, false);
                }
            }
        }

        private void Month_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CalendarButton b = sender as CalendarButton;
            if (b != null && this.Owner != null)
            {
                this.Owner.OnCalendarButtonPressed(b, true);
            }
        }

        private void Month_MouseEnter(object sender, MouseEventArgs e)
        {
            CalendarButton b = sender as CalendarButton;
            if (b != null)
            {
                if (this._isMonthPressed && this.Owner != null)
                {
                    this.Owner.OnCalendarButtonPressed(b, false);
                }
            }
        }

        private void Month_Clicked(object sender, RoutedEventArgs e)
        {
            CalendarButton b = sender as CalendarButton;
            if (b != null)
            {
                this.Owner.OnCalendarButtonPressed(b, true);
            }
        }

        private void HeaderButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Owner != null)
            {
                if (this.Owner.DisplayMode == CalendarMode.Month)
                {
                    this.Owner.DisplayMode = CalendarMode.Year;
                }
                else
                {
                    Debug.Assert(this.Owner.DisplayMode == CalendarMode.Year);

                    this.Owner.DisplayMode = CalendarMode.Decade;
                }

                this.FocusDate(this.DisplayDate);
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.OnPreviousClick();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.OnNextClick();
            }
        }

        private void PopulateGrids()
        {
            if (_monthView != null)
            {
                if (_dayTitleTemplate != null)
                {
                    for (int i = 0; i < COLS; i++)
                    {
                        FrameworkElement titleCell = (FrameworkElement)this._dayTitleTemplate.LoadContent();
                        titleCell.SetValue(Grid.RowProperty, 0);
                        titleCell.SetValue(Grid.ColumnProperty, i);
                        this._monthView.Children.Add(titleCell);
                    }
                }

                for (int i = 1; i < ROWS; i++)
                {
                    for (int j = 0; j < COLS; j++)
                    {
                        CalendarDayButton dayCell = new CalendarDayButton();

                        dayCell.Owner = this.Owner;
                        dayCell.SetValue(Grid.RowProperty, i);
                        dayCell.SetValue(Grid.ColumnProperty, j);
                        dayCell.SetBinding(CalendarDayButton.StyleProperty, GetOwnerBinding("CalendarDayButtonStyle"));
                        dayCell.AddHandler(CalendarDayButton.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Cell_MouseLeftButtonDown), true);
                        dayCell.AddHandler(CalendarDayButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Cell_MouseLeftButtonUp), true);
                        dayCell.AddHandler(CalendarDayButton.MouseEnterEvent, new MouseEventHandler(Cell_MouseEnter), true);
                        dayCell.Click += new RoutedEventHandler(Cell_Clicked);
                        dayCell.AddHandler(PreviewKeyDownEvent, new RoutedEventHandler(CellOrMonth_PreviewKeyDown), true);

                        this._monthView.Children.Add(dayCell);
                    }
                }
            }

            if (_yearView != null)
            {
                CalendarButton monthCell;
                int count = 0;
                for (int i = 0; i < YEAR_ROWS; i++)
                {
                    for (int j = 0; j < YEAR_COLS; j++)
                    {
                        monthCell = new CalendarButton();

                        monthCell.Owner = this.Owner;
                        monthCell.SetValue(Grid.RowProperty, i);
                        monthCell.SetValue(Grid.ColumnProperty, j);
                        monthCell.SetBinding(CalendarButton.StyleProperty, GetOwnerBinding("CalendarButtonStyle"));
                        monthCell.AddHandler(CalendarButton.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Month_MouseLeftButtonDown), true);
                        monthCell.AddHandler(CalendarButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Month_MouseLeftButtonUp), true);
                        monthCell.AddHandler(CalendarButton.MouseEnterEvent, new MouseEventHandler(Month_MouseEnter), true);
                        monthCell.AddHandler(UIElement.PreviewKeyDownEvent, new RoutedEventHandler(CellOrMonth_PreviewKeyDown), true);
                        monthCell.Click += new RoutedEventHandler(Month_Clicked);
                        this._yearView.Children.Add(monthCell);
                        count++;
                    }
                }
            }
        }


        #region Month Mode Display

        private void SetMonthModeDayTitles()
        {
            if (_monthView != null)
            {
                string[] shortestDayNames = DateTimeHelper.GetDateFormat(DateTimeHelper.GetCulture(this)).ShortestDayNames;

                for (int childIndex = 0; childIndex < COLS; childIndex++)
                {
                    FrameworkElement daytitle = _monthView.Children[childIndex] as FrameworkElement;
                    
                    if (daytitle != null && shortestDayNames != null && shortestDayNames.Length > 0)
                    {
                        if (this.Owner != null)
                        {
                            daytitle.DataContext = shortestDayNames[(childIndex + (int)this.Owner.FirstDayOfWeek) % shortestDayNames.Length];
                        }
                        else
                        {
                            daytitle.DataContext = shortestDayNames[(childIndex + (int)DateTimeHelper.GetDateFormat(DateTimeHelper.GetCulture(this)).FirstDayOfWeek) % shortestDayNames.Length];
                        }
                    }
                }
            }
        }

        private void SetMonthModeCalendarDayButtons()
        {
            //DateTime firstDayOfMonth = DateTimeHelper.DiscardDayTime(DisplayDate);
            DateTime firstDayOfMonth = DateTimeHelper.GetFirstDayOfMonth(DisplayDate);
            int lastMonthToDisplay = GetNumberOfDisplayedDaysFromPreviousMonth(firstDayOfMonth);

            bool isMinMonth = DateTimeHelper.CompareYearMonth(firstDayOfMonth, _calendar.MinSupportedDateTime) <= 0;
            bool isMaxMonth = DateTimeHelper.CompareYearMonth(firstDayOfMonth, _calendar.MaxSupportedDateTime) >= 0;
            int daysInMonth = _calendar.GetDaysInMonth(firstDayOfMonth.Year, firstDayOfMonth.Month);
            CultureInfo culture = DateTimeHelper.GetCulture(this);

            int count = ROWS * COLS;
            for (int childIndex = COLS; childIndex < count; childIndex++)
            {
                CalendarDayButton childButton = _monthView.Children[childIndex] as CalendarDayButton;
                Debug.Assert(childButton != null);

                int dayOffset = childIndex - lastMonthToDisplay - COLS;
                if ((!isMinMonth || (dayOffset >= 0)) && (!isMaxMonth || (dayOffset < daysInMonth)))
                {
                    DateTime dateToAdd = _calendar.AddDays(firstDayOfMonth, dayOffset);
                    SetMonthModeDayButtonState(childButton, dateToAdd);
                    childButton.DataContext = dateToAdd;
                    childButton.SetContentInternal(DateTimeHelper.ToDayString(dateToAdd, culture));
                }
                else
                {
                    SetMonthModeDayButtonState(childButton, null);
                    childButton.DataContext = null;
                    childButton.SetContentInternal(DateTimeHelper.ToDayString(null, culture));
                }
            }
        }

        private void SetMonthModeDayButtonState(CalendarDayButton childButton, DateTime? dateToAdd)
        {
            if (this.Owner != null)
            {
                if (dateToAdd.HasValue)
                {
                    childButton.Visibility = Visibility.Visible;

                    // If the day is outside the DisplayDateStart/End boundary, do not show it
                    if (DateTimeHelper.CompareDays(dateToAdd.Value, this.Owner.DisplayDateStartInternal) < 0 || DateTimeHelper.CompareDays(dateToAdd.Value, this.Owner.DisplayDateEndInternal) > 0)
                    {
                        childButton.IsEnabled = false;
                        childButton.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        childButton.IsEnabled = true;

                        // SET IF THE DAY IS SELECTABLE OR NOT
                        childButton.SetValue(
                            CalendarDayButton.IsBlackedOutPropertyKey, 
                            this.Owner.BlackoutDates.Contains(dateToAdd.Value));

                        // SET IF THE DAY IS ACTIVE OR NOT: set if the day is a trailing day or not
                        childButton.SetValue(
                            CalendarDayButton.IsInactivePropertyKey, 
                            DateTimeHelper.CompareYearMonth(dateToAdd.Value, this.Owner.DisplayDateInternal) != 0);

                        // SET IF THE DAY IS TODAY OR NOT
                        if (DateTimeHelper.CompareDays(dateToAdd.Value, DateTime.Today) == 0)
                        {
                            childButton.SetValue(CalendarDayButton.IsTodayPropertyKey, true);
                            
                            // PersianCalendar.IsTodayHighlighted affects the final visual state for Today buttons 
                            // but childButton property change callbacks are no called in response to 
                            // PersianCalendar.IsTodayHighlighted changing so we must explicitly update the visual state
                            childButton.ChangeVisualState(true);
                        }
                        else
                        {
                            childButton.SetValue(CalendarDayButton.IsTodayPropertyKey, false);
                        }

                        // SET IF THE DAY IS SELECTED OR NOT
                        // Since we should be comparing the Date values not DateTime values, we can't use this.Owner.SelectedDates.Contains(dateToAdd) directly
                        bool isSelected = false;
                        foreach (DateTime item in this.Owner.SelectedDates)
                        {
                            isSelected |= (DateTimeHelper.CompareDays(dateToAdd.Value, item) == 0);                            
                        }

                        childButton.SetValue(CalendarDayButton.IsSelectedPropertyKey, isSelected);
                    }
                }
                else
                {
                    childButton.Visibility = Visibility.Hidden;
                    childButton.IsEnabled = false;
                    childButton.SetValue(CalendarDayButton.IsBlackedOutPropertyKey, false);
                    childButton.SetValue(CalendarDayButton.IsInactivePropertyKey, true);
                    childButton.SetValue(CalendarDayButton.IsTodayPropertyKey, false);
                    childButton.SetValue(CalendarDayButton.IsSelectedPropertyKey, false);
                }
            }
        }

        private void AddMonthModeHighlight()
        {
            var owner = this.Owner;
            if (owner == null)
            {
                return;
            }

            if (owner.HoverStart.HasValue && owner.HoverEnd.HasValue)
            {
                DateTime hStart = owner.HoverEnd.Value;
                DateTime hEnd = owner.HoverEnd.Value;

                int daysToHighlight = DateTimeHelper.CompareDays(owner.HoverEnd.Value, owner.HoverStart.Value);
                if (daysToHighlight < 0)
                {
                    hEnd = this.Owner.HoverStart.Value;
                }
                else
                {
                    hStart = this.Owner.HoverStart.Value;
                }

                int count = ROWS * COLS;

                for (int childIndex = COLS; childIndex < count; childIndex++)
                {
                    CalendarDayButton childButton = _monthView.Children[childIndex] as CalendarDayButton;
                    if (childButton.DataContext is DateTime)
                    {
                        DateTime date = (DateTime)childButton.DataContext;
                        childButton.SetValue(
                            CalendarDayButton.IsHighlightedPropertyKey,
                            (daysToHighlight != 0) && DateTimeHelper.InRange(date, hStart, hEnd));
                    }
                    else
                    {
                        childButton.SetValue(CalendarDayButton.IsHighlightedPropertyKey, false);
                    }
                }
            }
            else
            {
                int count = ROWS * COLS;

                for (int childIndex = COLS; childIndex < count; childIndex++)
                {
                    CalendarDayButton childButton = _monthView.Children[childIndex] as CalendarDayButton;
                    childButton.SetValue(CalendarDayButton.IsHighlightedPropertyKey, false);
                }
            }
        }

        private void SetMonthModeHeaderButton()
        {
            if (this._headerButton != null)
            {
                this._headerButton.Content = DateTimeHelper.ToYearMonthPatternString(DisplayDate, DateTimeHelper.GetCulture(this));

                if (this.Owner != null)
                {                    
                    this._headerButton.IsEnabled = true;
                }
            }
        }

        private void SetMonthModeNextButton()
        {
            if (this.Owner != null && _nextButton != null)
            {
                DateTime firstDayOfMonth = DateTimeHelper.DiscardDayTime(DisplayDate);

                // DisplayDate is equal to DateTime.MaxValue
                if (DateTimeHelper.CompareYearMonth(firstDayOfMonth, DateTime.MaxValue) == 0)
                {
                    _nextButton.IsEnabled = false;
                }
                else
                {
                    // Since we are sure DisplayDate is not equal to DateTime.MaxValue, 
                    // it is safe to use AddMonths  
                    DateTime firstDayOfNextMonth = _calendar.AddMonths(firstDayOfMonth, 1);
                    _nextButton.IsEnabled = (DateTimeHelper.CompareDays(this.Owner.DisplayDateEndInternal, firstDayOfNextMonth) > -1);
                }
            }
        }

        private void SetMonthModePreviousButton()
        {
            if (this.Owner != null && _previousButton != null)
            {
                DateTime firstDayOfMonth = DateTimeHelper.DiscardDayTime(DisplayDate);
                _previousButton.IsEnabled = (DateTimeHelper.CompareDays(this.Owner.DisplayDateStartInternal, firstDayOfMonth) < 0);
            }
        }

        #endregion

        #region Year Mode Display

        private void SetYearButtons(DateTime decade, DateTime decadeEnd)
        {
            int decadeYear = _calendar.GetYear(decade);
            int decadeEndYear = _calendar.GetYear(decadeEnd);
            int year;
            int count = -1;
            foreach (object child in _yearView.Children)
            {
                CalendarButton childButton = child as CalendarButton;
                Debug.Assert(childButton != null);
                year = decadeYear + count;

                if (year <= _calendar.MaxSupportedDateTime.Year && year >= _calendar.MinSupportedDateTime.Year)
                {
                    // There should be no time component. Time is 12:00 AM
                    DateTime day = _calendar.ToDateTime(year, 1, 1, 0, 0, 0, 0);
                    childButton.DataContext = day;
                    childButton.SetContentInternal(DateTimeHelper.ToYearString(day, DateTimeHelper.GetCulture(this)));
                    childButton.Visibility = Visibility.Visible;

                    if (this.Owner != null)
                    {
                        childButton.HasSelectedDays = (_calendar.GetYear(Owner.DisplayDate) == year);

                        if (year < this.Owner.DisplayDateStartInternal.Year || year > this.Owner.DisplayDateEndInternal.Year)
                        {
                            childButton.IsEnabled = false;
                            childButton.Opacity = 0;
                        }
                        else
                        {
                            childButton.IsEnabled = true;
                            childButton.Opacity = 1;
                        }
                    }

                    // SET IF THE YEAR IS INACTIVE OR NOT: set if the year is a trailing year or not
                    childButton.IsInactive = year < decadeYear || year > decadeEndYear;
                }
                else
                {
                    childButton.DataContext = null;
                    childButton.IsEnabled = false;
                    childButton.Opacity = 0;
                }

                count++;
            }
        }

        private void SetYearModeMonthButtons()
        {
            int count = 0;
            foreach (object child in _yearView.Children)
            {
                CalendarButton childButton = child as CalendarButton;
                Debug.Assert(childButton != null);
                
                // There should be no time component. Time is 12:00 AM
                int year = _calendar.GetYear(DisplayDate);
                int month = count + 1;
                DateTime day = _calendar.ToDateTime(year, month, 1, 0, 0, 0, 0);
                childButton.DataContext = day;
                childButton.SetContentInternal(DateTimeHelper.ToAbbreviatedMonthString(day, DateTimeHelper.GetCulture(this)));
                childButton.Visibility = Visibility.Visible;

                if (this.Owner != null)
                {
                    Debug.Assert(this.Owner.DisplayDateInternal != null);
                    childButton.HasSelectedDays = (DateTimeHelper.CompareYearMonth(day, this.Owner.DisplayDateInternal) == 0);

                    if (DateTimeHelper.CompareYearMonth(day, this.Owner.DisplayDateStartInternal) < 0 || DateTimeHelper.CompareYearMonth(day, this.Owner.DisplayDateEndInternal) > 0)
                    {
                        childButton.IsEnabled = false;
                        childButton.Opacity = 0;
                    }
                    else
                    {
                        childButton.IsEnabled = true;
                        childButton.Opacity = 1;
                    }
                }

                childButton.IsInactive = false;
                count++;
            }
        }

        private void SetYearModeHeaderButton()
        {
            if (this._headerButton != null)
            {
                this._headerButton.IsEnabled = true;
                this._headerButton.Content = DateTimeHelper.ToYearString(DisplayDate, DateTimeHelper.GetCulture(this));
            }
        }

        private void SetYearModeNextButton()
        {
            if (this.Owner != null && _nextButton != null)
            {
                _nextButton.IsEnabled = (this.Owner.DisplayDateEndInternal.Year != DisplayDate.Year);
            }
        }

        private void SetYearModePreviousButton()
        {
            if (this.Owner != null && _previousButton != null)
            {
                _previousButton.IsEnabled = (this.Owner.DisplayDateStartInternal.Year != DisplayDate.Year);
            }
        }

        #endregion Year Mode Display

        #region Decade Mode Display

        private void SetDecadeModeHeaderButton(DateTime decade)
        {
            if (this._headerButton != null)
            {
                this._headerButton.Content = DateTimeHelper.ToDecadeRangeString(decade, DateTimeHelper.GetCulture(this));
                this._headerButton.IsEnabled = false;
            }
        }

        private void SetDecadeModeNextButton(DateTime decadeEnd)
        {
            if (this.Owner != null && _nextButton != null)
            {
                int displayEndYear = _calendar.GetYear(this.Owner.DisplayDateEndInternal);
                int decadeEndYear = _calendar.GetYear(decadeEnd);
                _nextButton.IsEnabled = (displayEndYear > decadeEndYear);
            }
        }

        private void SetDecadeModePreviousButton(DateTime decade)
        {
            if (this.Owner != null && _previousButton != null)
            {
                int displayStartYear = _calendar.GetYear(this.Owner.DisplayDateStartInternal);
                int decadeYear = _calendar.GetYear(decade);
                _previousButton.IsEnabled = (decadeYear > displayStartYear);
            }
        }

        #endregion Decade Mode Display

        // How many days of the previous month need to be displayed
        private int GetNumberOfDisplayedDaysFromPreviousMonth(DateTime firstOfMonth)
        {
            DayOfWeek day = _calendar.GetDayOfWeek(firstOfMonth);
            int i;

            if (this.Owner != null)
            {
                i = ((day - this.Owner.FirstDayOfWeek + NUMBER_OF_DAYS_IN_WEEK) % NUMBER_OF_DAYS_IN_WEEK);
            }
            else
            {
                i = ((day - DateTimeHelper.GetDateFormat(DateTimeHelper.GetCulture(this)).FirstDayOfWeek + NUMBER_OF_DAYS_IN_WEEK) % NUMBER_OF_DAYS_IN_WEEK);
            }

            if (i == 0)
            {
                return NUMBER_OF_DAYS_IN_WEEK;
            }
            else
            {
                return i;
            }
        }

        /// <summary>
        /// Gets a binding to a property on the owning calendar
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private BindingBase GetOwnerBinding(string propertyName)
        {
            Binding result = new Binding(propertyName);
            result.Source = this.Owner;
            return result;
        }

        #endregion Private Methods
    }
}
