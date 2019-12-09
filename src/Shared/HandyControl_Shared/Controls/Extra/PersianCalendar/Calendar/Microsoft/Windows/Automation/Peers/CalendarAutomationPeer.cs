//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using Microsoft.Windows.Controls;
using Microsoft.Windows.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using PersianCalendar = HandyControl.Controls.PersianCalendar;
using CalendarMode = Microsoft.Windows.Controls.CalendarMode;
using CalendarSelectionMode = Microsoft.Windows.Controls.CalendarSelectionMode;

namespace Microsoft.Windows.Automation.Peers
{
    /// <summary>
    /// AutomationPeer for Calendar Control
    /// </summary>
    internal sealed class CalendarAutomationPeer : FrameworkElementAutomationPeer, IGridProvider, IMultipleViewProvider, ISelectionProvider, ITableProvider
    {
        /// <summary>
        /// Initializes a new instance of the CalendarAutomationPeer class.
        /// </summary>
        /// <param name="owner">Owning Calendar</param>
        public CalendarAutomationPeer(HandyControl.Controls.PersianCalendar owner)
            : base(owner)
        {
        }

        #region Private Properties

        private HandyControl.Controls.PersianCalendar OwningCalendar => Owner as HandyControl.Controls.PersianCalendar;

        private Grid OwningGrid
        {
            get
            {
                if (OwningCalendar != null && OwningCalendar.MonthControl != null)
                {
                    if (OwningCalendar.DisplayMode == CalendarMode.Month)
                    {
                        return OwningCalendar.MonthControl.MonthView;
                    }
                    else
                    {
                        return OwningCalendar.MonthControl.YearView;
                    }
                }

                return null;
            }
        }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Gets the control pattern that is associated with the specified System.Windows.Automation.Peers.PatternInterface.
        /// </summary>
        /// <param name="patternInterface">A value from the System.Windows.Automation.Peers.PatternInterface enumeration.</param>
        /// <returns>The object that supports the specified pattern, or null if unsupported.</returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            switch (patternInterface)
            {
                case PatternInterface.Grid:
                case PatternInterface.Table:
                case PatternInterface.MultipleView:
                case PatternInterface.Selection:
                    {
                        if (OwningGrid != null)
                        {
                            return this;
                        }

                        break;
                    }

                default: break;
            }

            return base.GetPattern(patternInterface);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Gets the control type for the element that is associated with the UI Automation peer.
        /// </summary>
        /// <returns>The control type.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Calendar;
        }

        /// <summary>
        /// Called by GetClassName that gets a human readable name that, in addition to AutomationControlType, 
        /// differentiates the control represented by this AutomationPeer.
        /// </summary>
        /// <returns>The string that contains the name.</returns>
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        #endregion Protected Methods

        #region InternalMethods

        internal void RaiseSelectionEvents(SelectionChangedEventArgs e)
        {
            int numSelected = OwningCalendar.SelectedDates.Count;
            int numAdded = e.AddedItems.Count;

            if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected) && numSelected == 1 && numAdded == 1)
            {
                CalendarDayButton selectedButton = OwningCalendar.FindDayButtonFromDay((DateTime)e.AddedItems[0]);

                if (selectedButton != null)
                {
                    AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(selectedButton);

                    if (peer != null)
                    {
                        peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
                    }
                }
            }
            else
            {
                if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection))
                {
                    foreach (DateTime date in e.AddedItems)
                    {
                        CalendarDayButton selectedButton = OwningCalendar.FindDayButtonFromDay(date);

                        if (selectedButton != null)
                        {
                            AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(selectedButton);

                            if (peer != null)
                            {
                                peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementAddedToSelection);
                            }
                        }
                    }
                }

                if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
                {
                    foreach (DateTime date in e.RemovedItems)
                    {
                        CalendarDayButton removedButton = OwningCalendar.FindDayButtonFromDay(date);

                        if (removedButton != null)
                        {
                            AutomationPeer peer = FrameworkElementAutomationPeer.FromElement(removedButton);

                            if (peer != null)
                            {
                                peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
                            }
                        }
                    }
                }
            }
        }

        #endregion InternalMethods

        #region IGridProvider

        int IGridProvider.ColumnCount
        {
            get
            {
                if (OwningGrid != null)
                {
                    return OwningGrid.ColumnDefinitions.Count;
                }

                return 0;
            }
        }

        int IGridProvider.RowCount
        {
            get
            {
                if (OwningGrid != null)
                {
                    if (OwningCalendar.DisplayMode == CalendarMode.Month)
                    {
                        // In Month DisplayMode, since first row is DayTitles, we return the RowCount-1
                        return Math.Max(0, OwningGrid.RowDefinitions.Count - 1);
                    }
                    else
                    {
                        return OwningGrid.RowDefinitions.Count;
                    }
                }

                return 0;
            }
        }

        IRawElementProviderSimple IGridProvider.GetItem(int row, int column)
        {
            if (OwningCalendar.DisplayMode == CalendarMode.Month)
            {
                // In Month DisplayMode, since first row is DayTitles, we increment the row number by 1
                row++;
            }

            if (OwningGrid != null && row >= 0 && row < OwningGrid.RowDefinitions.Count && column >= 0 && column < OwningGrid.ColumnDefinitions.Count)
            {
                foreach (UIElement child in OwningGrid.Children)
                {
                    int childRow = (int)child.GetValue(Grid.RowProperty);
                    int childColumn = (int)child.GetValue(Grid.ColumnProperty);
                    if (childRow == row && childColumn == column)
                    {
                        AutomationPeer peer = CreatePeerForElement(child);
                        if (peer != null)
                        {
                            return ProviderFromPeer(peer);
                        }
                    }
                }
            }

            return null;
        }

        #endregion IGridProvider

        #region IMultipleViewProvider

        int IMultipleViewProvider.CurrentView => (int)OwningCalendar.DisplayMode;

        int[] IMultipleViewProvider.GetSupportedViews()
        {
            int[] supportedViews = new int[3];

            supportedViews[0] = (int)CalendarMode.Month;
            supportedViews[1] = (int)CalendarMode.Year;
            supportedViews[2] = (int)CalendarMode.Decade;

            return supportedViews;
        }

        string IMultipleViewProvider.GetViewName(int viewId)
        {
            switch (viewId)
            {
                case 0:
                    {
                        return SR.Get(SRID.CalendarAutomationPeer_MonthMode);
                    }

                case 1:
                    {
                        return SR.Get(SRID.CalendarAutomationPeer_YearMode);
                    }

                case 2:
                    {
                        return SR.Get(SRID.CalendarAutomationPeer_DecadeMode);
                    }
            }

            // TODO: update when Jolt 23302 is fixed
            // throw new ArgumentOutOfRangeException("viewId", Resource.Calendar_OnDisplayModePropertyChanged_InvalidValue);
            return string.Empty;
        }

        void IMultipleViewProvider.SetCurrentView(int viewId)
        {
            OwningCalendar.DisplayMode = (CalendarMode)viewId;
        }

        #endregion IMultipleViewProvider

        #region ISelectionProvider

        bool ISelectionProvider.CanSelectMultiple => OwningCalendar.SelectionMode == CalendarSelectionMode.SingleRange || OwningCalendar.SelectionMode == CalendarSelectionMode.MultipleRange;

        bool ISelectionProvider.IsSelectionRequired => false;

        IRawElementProviderSimple[] ISelectionProvider.GetSelection()
        {
            List<IRawElementProviderSimple> providers = new List<IRawElementProviderSimple>();

            if (OwningGrid != null)
            {
                if (OwningCalendar.DisplayMode == CalendarMode.Month && OwningCalendar.SelectedDates != null && OwningCalendar.SelectedDates.Count != 0)
                {
                    // return selected DayButtons
                    CalendarDayButton dayButton;

                    foreach (UIElement child in OwningGrid.Children)
                    {
                        int childRow = (int)child.GetValue(Grid.RowProperty);

                        if (childRow != 0)
                        {
                            dayButton = child as CalendarDayButton;

                            if (dayButton != null && dayButton.IsSelected)
                            {
                                AutomationPeer peer = CreatePeerForElement(dayButton);

                                if (peer != null)
                                {
                                    providers.Add(ProviderFromPeer(peer));
                                }
                            }
                        }
                    }
                }
                else
                {
                    // return the CalendarButton which has focus
                    CalendarButton calendarButton;

                    foreach (UIElement child in OwningGrid.Children)
                    {
                        calendarButton = child as CalendarButton;

                        if (calendarButton != null && calendarButton.IsFocused)
                        {
                            AutomationPeer peer = CreatePeerForElement(calendarButton);

                            if (peer != null)
                            {
                                providers.Add(ProviderFromPeer(peer));
                            }

                            break;
                        }
                    }
                }

                if (providers.Count > 0)
                {
                    return providers.ToArray();
                }
            }

            return null;
        }

        #endregion ISelectionProvider

        #region ITableProvider

        RowOrColumnMajor ITableProvider.RowOrColumnMajor => RowOrColumnMajor.RowMajor;

        IRawElementProviderSimple[] ITableProvider.GetColumnHeaders()
        {
            if (OwningCalendar.DisplayMode == CalendarMode.Month)
            {
                List<IRawElementProviderSimple> providers = new List<IRawElementProviderSimple>();

                foreach (UIElement child in OwningGrid.Children)
                {
                    int childRow = (int)child.GetValue(Grid.RowProperty);

                    if (childRow == 0)
                    {
                        AutomationPeer peer = CreatePeerForElement(child);

                        if (peer != null)
                        {
                            providers.Add(ProviderFromPeer(peer));
                        }
                    }
                }

                if (providers.Count > 0)
                {
                    return providers.ToArray();
                }
            }

            return null;
        }

        // If WeekNumber functionality is supported by Calendar in the future,
        // this method should return weeknumbers
        IRawElementProviderSimple[] ITableProvider.GetRowHeaders()
        {
            return null;
        }

        #endregion ITableProvider
    }
}
