//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using HandyControl.Controls;
using CalendarButtonAutomationPeer = Microsoft.Windows.Automation.Peers.CalendarButtonAutomationPeer;

namespace Microsoft.Windows.Controls.Primitives
{
    /// <summary>
    /// Represents a button control used in PersianCalendar Control, which reacts to the Click event.
    /// </summary>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateUnselected, GroupName = VisualStates.GroupSelection)]
    [TemplateVisualState(Name = VisualStates.StateSelected, GroupName = VisualStates.GroupSelection)]
    [TemplateVisualState(Name = VisualStates.StateCalendarButtonUnfocused, GroupName = VisualStates.GroupCalendarButtonFocus)]
    [TemplateVisualState(Name = VisualStates.StateCalendarButtonFocused, GroupName = VisualStates.GroupCalendarButtonFocus)]
    [TemplateVisualState(Name = VisualStates.StateInactive, GroupName = VisualStates.GroupActive)]
    [TemplateVisualState(Name = VisualStates.StateActive, GroupName = VisualStates.GroupActive)]
    public sealed class CalendarButton : Button
    {
        #region Data

        private bool _shouldCoerceContent;
        private object _coercedContent;

        #endregion Data

        /// <summary>
        /// Static constructor
        /// </summary>
        static CalendarButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarButton), new FrameworkPropertyMetadata(typeof(CalendarButton)));
            ContentProperty.OverrideMetadata(typeof(CalendarButton), new FrameworkPropertyMetadata(null, OnCoerceContent));
        }

        /// <summary>
        /// Represents the CalendarButton that is used in PersianCalendar Control.
        /// </summary>
        public CalendarButton()
            : base()
        {
            // Attach the necessary events to their virtual counterparts
            Loaded += delegate { ChangeVisualState(false); };
        }

        #region Public Properties

        #region HasSelectedDays

        internal static readonly DependencyPropertyKey HasSelectedDaysPropertyKey = DependencyProperty.RegisterReadOnly(
            "HasSelectedDays",
            typeof(bool),
            typeof(CalendarButton),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnVisualStatePropertyChanged)));

        /// <summary>
        /// Dependency property field for HasSelectedDays property
        /// </summary>
        public static readonly DependencyProperty HasSelectedDaysProperty = HasSelectedDaysPropertyKey.DependencyProperty;

        /// <summary>
        /// True if the CalendarButton represents a date range containing the display date
        /// </summary>
        public bool HasSelectedDays
        {
            get { return (bool)GetValue(HasSelectedDaysProperty); }
            internal set { SetValue(HasSelectedDaysPropertyKey, value); }
        }

        #endregion HasSelectedDays

        #region IsInactive

        internal static readonly DependencyPropertyKey IsInactivePropertyKey = DependencyProperty.RegisterReadOnly(
            "IsInactive",
            typeof(bool),
            typeof(CalendarButton),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnVisualStatePropertyChanged)));

        /// <summary>
        /// Dependency property field for IsInactive property
        /// </summary>
        public static readonly DependencyProperty IsInactiveProperty = IsInactivePropertyKey.DependencyProperty;

        /// <summary>
        /// True if the CalendarButton represents
        ///     a month that falls outside the current year 
        ///     or 
        ///     a year that falls outside the current decade
        /// </summary>
        public bool IsInactive
        {
            get { return (bool)GetValue(IsInactiveProperty); }
            internal set { SetValue(IsInactivePropertyKey, value); }
        }

        #endregion IsInactive

        #endregion Public Properties

        #region Internal Properties

        internal PersianCalendar Owner
        {
            get;
            set;
        }

        #endregion Internal Properties

        #region Public Methods

        /// <summary>
        /// Apply a template to the button.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Sync the logical and visual states of the control
            ChangeVisualState(false);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Creates the automation peer for the DayButton.
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CalendarButtonAutomationPeer(this);
        }

        protected override void OnGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            ChangeVisualState(true);
            base.OnGotKeyboardFocus(e);
        }

        protected override void OnLostKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            ChangeVisualState(true);
            base.OnLostKeyboardFocus(e);
        }

        #endregion Protected Methods

        #region Internal Methods

        internal void SetContentInternal(string value)
        {
            if (BindingOperations.GetBindingExpressionBase(this, ContentControl.ContentProperty) != null)
            {
                Content = value;
            }
            else
            {
                this._shouldCoerceContent = true;
                this._coercedContent = value;
                this.CoerceValue(ContentControl.ContentProperty);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Change to the correct visual state for the button.
        /// </summary>
        /// <param name="useTransitions">
        /// true to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        private void ChangeVisualState(bool useTransitions)
        {
            // Update the SelectionStates group
            if (HasSelectedDays)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateSelected, VisualStates.StateUnselected);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateUnselected);
            }

            // Update the ActiveStates group
            if (IsInactive)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateInactive);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateActive, VisualStates.StateInactive);
            }

            // Update the FocusStates group
            if (IsKeyboardFocused)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateCalendarButtonFocused, VisualStates.StateCalendarButtonUnfocused);
            }
            else
            {
                VisualStateManager.GoToState(this, VisualStates.StateCalendarButtonUnfocused, useTransitions);
            }
        }

        /// <summary>
        /// Common PropertyChangedCallback for dependency properties that trigger visual state changes
        /// </summary>
        /// <param name="dObject"></param>
        /// <param name="e"></param>
        private static void OnVisualStatePropertyChanged(DependencyObject dObject, DependencyPropertyChangedEventArgs e)
        {
            CalendarButton button = dObject as CalendarButton;
            if (button != null && !object.Equals(e.OldValue, e.NewValue))
            {
                button.ChangeVisualState(true);
            }
        }

        private static object OnCoerceContent(DependencyObject sender, object baseValue)
        {
            CalendarButton button = (CalendarButton)sender;
            if (button._shouldCoerceContent)
            {
                button._shouldCoerceContent = false;
                return button._coercedContent;
            }

            return baseValue;
        }

        #endregion Private Methods
    }
}
