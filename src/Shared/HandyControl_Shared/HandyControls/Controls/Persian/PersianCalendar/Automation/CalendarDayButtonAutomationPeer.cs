//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using HandyControl.Controls;
using Microsoft.Windows.Controls;
using Microsoft.Windows.Controls.Primitives;
using CalendarSelectionMode = Microsoft.Windows.Controls.CalendarSelectionMode;

namespace Microsoft.Windows.Automation.Peers
{
    /// <summary>
    /// AutomationPeer for CalendarDayButton
    /// </summary>
    public sealed class CalendarDayButtonAutomationPeer : ButtonAutomationPeer, IGridItemProvider, ISelectionItemProvider, ITableItemProvider
    {
        /// <summary>
        /// Initializes a new instance of the CalendarDayButtonAutomationPeer class.
        /// </summary>
        /// <param name="owner">Owning CalendarDayButton</param>
        public CalendarDayButtonAutomationPeer(CalendarDayButton owner)
            : base(owner)
        {
        }

        #region Private Properties

        private PersianCalendar OwningPersianCalendar
        {
            get
            {
                return this.OwningCalendarDayButton.Owner;
            }
        }

        private IRawElementProviderSimple OwningCalendarAutomationPeer
        {
            get
            {
                if (this.OwningPersianCalendar != null)
                {
                    AutomationPeer peer = CreatePeerForElement(this.OwningPersianCalendar);

                    if (peer != null)
                    {
                        return ProviderFromPeer(peer);
                    }
                }

                return null;
            }
        }

        private CalendarDayButton OwningCalendarDayButton
        {
            get
            {
                return this.Owner as CalendarDayButton;
            }
        }

        private DateTime? Date
        {
            get
            {
                if (this.OwningCalendarDayButton != null && this.OwningCalendarDayButton.DataContext is DateTime)
                {
                    return (DateTime?)this.OwningCalendarDayButton.DataContext;
                }
                else
                {
                    return null;
                }
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
            object result = null;

            switch (patternInterface)
            {
                case PatternInterface.SelectionItem:
                case PatternInterface.GridItem:
                case PatternInterface.TableItem:
                {
                    if (this.OwningPersianCalendar != null && this.OwningCalendarDayButton != null)
                    {
                        result = this;
                    }
                    else
                    {
                        result = base.GetPattern(patternInterface);
                    }

                    break;
                }

                default:
                {
                    result = base.GetPattern(patternInterface);
                    break;
                }
            }

            return result;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Gets the control type for the element that is associated with the UI Automation peer.
        /// </summary>
        /// <returns>The control type.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Button;
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

        /// <summary>
        /// Overrides the GetHelpTextCore method for CalendarDayButtonAutomationPeer
        /// </summary>
        /// <returns></returns>
        protected override string GetHelpTextCore()
        {
            DateTime? date = this.Date;
            if (date.HasValue)
            {
                string dateString = DateTimeHelper.ToLongDateString(Date, DateTimeHelper.GetCulture(this.OwningCalendarDayButton));

                if (this.OwningCalendarDayButton.IsBlackedOut)
                {
                    return string.Format(DateTimeHelper.GetCurrentDateFormat(), "Blackout Day - {0}", dateString);
                }
                else
                {
                    return dateString;
                }
            }
            else
            {
                return base.GetHelpTextCore();
            }
        }

        /// <summary>
        /// Overrides the GetLocalizedControlTypeCore method for CalendarDayButtonAutomationPeer
        /// </summary>
        /// <returns></returns>
        protected override string GetLocalizedControlTypeCore()
        {
            return "Day button";
        }

        /// <summary>
        /// Overrides the GetNameCore method for CalendarDayButtonAutomationPeer
        /// </summary>
        /// <returns></returns>
        protected override string GetNameCore()
        {
            DateTime? date = this.Date;
            return date.HasValue ? DateTimeHelper.ToLongDateString(Date, DateTimeHelper.GetCulture(this.OwningCalendarDayButton)) : base.GetNameCore();
        }

        #endregion Protected Methods

        #region IGridItemProvider

        /// <summary>
        /// Grid item column.
        /// </summary>
        int IGridItemProvider.Column
        {
            get
            {
                return (int)this.OwningCalendarDayButton.GetValue(Grid.ColumnProperty);
            }
        }

        /// <summary>
        /// Grid item column span.
        /// </summary>
        int IGridItemProvider.ColumnSpan
        {
            get { return (int)this.OwningCalendarDayButton.GetValue(Grid.ColumnSpanProperty); }
        }

        /// <summary>
        /// Grid item's containing grid.
        /// </summary>
        IRawElementProviderSimple IGridItemProvider.ContainingGrid
        {
            get
            {
                return this.OwningCalendarAutomationPeer;
            }
        }

        /// <summary>
        /// Grid item row.
        /// </summary>
        int IGridItemProvider.Row
        {
            get
            {                
                Debug.Assert((int)this.OwningCalendarDayButton.GetValue(Grid.RowProperty) > 0);

                // we decrement the Row value by one since the first row is composed of DayTitles
                return (int)this.OwningCalendarDayButton.GetValue(Grid.RowProperty) - 1;
            }
        }

        /// <summary>
        /// Grid item row span.
        /// </summary>
        int IGridItemProvider.RowSpan
        {
            get
            {
                return (int)this.OwningCalendarDayButton.GetValue(Grid.RowSpanProperty);
            }
        }

        #endregion IGridItemProvider

        #region ISelectionItemProvider

        /// <summary>
        /// True if the owning CalendarDayButton is selected.
        /// </summary>
        bool ISelectionItemProvider.IsSelected 
        { 
            get 
            { 
                return this.OwningCalendarDayButton.IsSelected; 
            } 
        }

        /// <summary>
        /// Selection items selection container.
        /// </summary>
        IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
        {
            get
            {
                return this.OwningCalendarAutomationPeer;
            }
        }

        /// <summary>
        /// Adds selection item to selection.
        /// </summary>
        void ISelectionItemProvider.AddToSelection()
        {
            // Return if the item is already selected or a day is already selected in the SingleSelectionMode
            if (((ISelectionItemProvider)this).IsSelected)
            {
                return;
            }

            if (EnsureSelection() && this.OwningCalendarDayButton.DataContext is DateTime)
            {
                if (this.OwningPersianCalendar.SelectionMode == CalendarSelectionMode.SingleDate)
                {
                    this.OwningPersianCalendar.SelectedDate = (DateTime)this.OwningCalendarDayButton.DataContext;
                }
                else
                {
                    this.OwningPersianCalendar.SelectedDates.Add((DateTime)this.OwningCalendarDayButton.DataContext);
                }
            }
        }

        /// <summary>
        /// Removes selection item from selection.
        /// </summary>
        void ISelectionItemProvider.RemoveFromSelection()
        {
            // Return if the item is not already selected.
            if (!((ISelectionItemProvider)this).IsSelected)
            {
                return;
            }

            if (this.OwningCalendarDayButton.DataContext is DateTime)
            {
                this.OwningPersianCalendar.SelectedDates.Remove((DateTime)this.OwningCalendarDayButton.DataContext);
            }
        }

        /// <summary>
        /// Selects this item.
        /// </summary>
        void ISelectionItemProvider.Select()
        {
            if (EnsureSelection())
            {
                this.OwningPersianCalendar.SelectedDates.Clear();

                if (this.OwningCalendarDayButton.DataContext is DateTime)
                {
                    this.OwningPersianCalendar.SelectedDates.Add((DateTime)this.OwningCalendarDayButton.DataContext);
                }
            }
        }

        #endregion ISelectionItemProvider

        #region ITableItemProvider

        /// <summary>
        /// Gets the table item's column headers.
        /// </summary>
        /// <returns>The table item's column headers</returns>
        IRawElementProviderSimple[] ITableItemProvider.GetColumnHeaderItems()
        {
            if (this.OwningPersianCalendar != null && this.OwningCalendarAutomationPeer != null)
            {
                IRawElementProviderSimple[] headers = ((ITableProvider)CreatePeerForElement(this.OwningPersianCalendar)).GetColumnHeaders();

                if (headers != null)
                {
                    int column = ((IGridItemProvider)this).Column;
                    return new IRawElementProviderSimple[] { headers[column] };
                }
            }

            return null;
        }

        /// <summary>
        /// Get's the table item's row headers.
        /// </summary>
        /// <returns>The table item's row headers</returns>
        IRawElementProviderSimple[] ITableItemProvider.GetRowHeaderItems()
        {
            return null;
        }

        #endregion ITableItemProvider

        #region Private Methods

        private bool EnsureSelection()
        {
            if (!this.OwningCalendarDayButton.IsEnabled)
            {
                throw new ElementNotEnabledException();
            }

            // If the day is a blackout day or the SelectionMode is None, selection is not allowed
            if (this.OwningCalendarDayButton.IsBlackedOut ||
                this.OwningPersianCalendar.SelectionMode == CalendarSelectionMode.None)
            {
                return false;
            }

            return true;
        }

        #endregion Private Methods
    }
}
