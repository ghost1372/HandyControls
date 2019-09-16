//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using Microsoft.Windows.Controls;
using Microsoft.Windows.Controls.Primitives;
using System;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;
using PersianCalendar = HandyControl.Controls.PersianCalendar;
using CalendarMode = Microsoft.Windows.Controls.CalendarMode;

namespace Microsoft.Windows.Automation.Peers
{
    /// <summary>
    /// AutomationPeer for CalendarButton
    /// </summary>
    public sealed class CalendarButtonAutomationPeer : ButtonAutomationPeer, IGridItemProvider, ISelectionItemProvider
    {
        /// <summary>
        /// Initializes a new instance of the CalendarButtonAutomationPeer class.
        /// </summary>
        /// <param name="owner">Owning CalendarButton</param>
        public CalendarButtonAutomationPeer(CalendarButton owner)
            : base(owner)
        {
        }

        #region Private Properties

        private HandyControl.Controls.PersianCalendar OwningCalendar => OwningCalendarButton.Owner;

        private IRawElementProviderSimple OwningCalendarAutomationPeer
        {
            get
            {
                if (OwningCalendar != null)
                {
                    AutomationPeer peer = CreatePeerForElement(OwningCalendar);

                    if (peer != null)
                    {
                        return ProviderFromPeer(peer);
                    }
                }

                return null;
            }
        }

        private CalendarButton OwningCalendarButton => Owner as CalendarButton;

        private DateTime? Date
        {
            get
            {
                if (OwningCalendarButton != null && OwningCalendarButton.DataContext is DateTime)
                {
                    return (DateTime?)OwningCalendarButton.DataContext;
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
                    {
                        if (OwningCalendar != null && OwningCalendar.MonthControl != null && OwningCalendarButton != null)
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

        #endregion Public methods

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
        /// Overrides the GetLocalizedControlTypeCore method for CalendarButtonAutomationPeer
        /// </summary>
        /// <returns></returns>
        protected override string GetLocalizedControlTypeCore()
        {
            return SR.Get(SRID.CalendarAutomationPeer_CalendarButtonLocalizedControlType);
        }

        /// <summary>
        /// Overrides the GetHelpTextCore method for CalendarButtonAutomationPeer
        /// </summary>
        /// <returns></returns>
        protected override string GetHelpTextCore()
        {
            DateTime? date = Date;
            return date.HasValue ? DateTimeHelper.ToLongDateString(date, DateTimeHelper.GetCulture(OwningCalendarButton)) : base.GetHelpTextCore();
        }

        /// <summary>
        /// Overrides the GetNameCore method for CalendarButtonAutomationPeer
        /// </summary>
        /// <returns></returns>
        protected override string GetNameCore()
        {
            DateTime? date = Date;
            if (date.HasValue)
            {
                if (OwningCalendar.DisplayMode == CalendarMode.Decade)
                {
                    return DateTimeHelper.ToYearString(date, DateTimeHelper.GetCulture(OwningCalendarButton));
                }
                else
                {
                    return DateTimeHelper.ToYearMonthPatternString(date, DateTimeHelper.GetCulture(OwningCalendarButton));
                }
            }
            else
            {
                return base.GetNameCore();
            }
        }

        #endregion Protected Methods

        #region IGridItemProvider

        int IGridItemProvider.Column => (int)OwningCalendarButton.GetValue(Grid.ColumnProperty);

        int IGridItemProvider.ColumnSpan => (int)OwningCalendarButton.GetValue(Grid.ColumnSpanProperty);

        IRawElementProviderSimple IGridItemProvider.ContainingGrid => OwningCalendarAutomationPeer;

        int IGridItemProvider.Row => (int)OwningCalendarButton.GetValue(Grid.RowSpanProperty);

        int IGridItemProvider.RowSpan => 1;

        #endregion IGridItemProvider

        #region ISelectionItemProvider

        bool ISelectionItemProvider.IsSelected => OwningCalendarButton.IsFocused;

        IRawElementProviderSimple ISelectionItemProvider.SelectionContainer => OwningCalendarAutomationPeer;

        void ISelectionItemProvider.AddToSelection()
        {
            return;
        }

        void ISelectionItemProvider.RemoveFromSelection()
        {
            return;
        }

        void ISelectionItemProvider.Select()
        {
            if (OwningCalendarButton.IsEnabled)
            {
                OwningCalendarButton.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }
            else
            {
                throw new ElementNotEnabledException();
            }
        }

        #endregion ISelectionItemProvider
    }
}
