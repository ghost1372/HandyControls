//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using HandyControl.Controls;
using Microsoft.Windows.Controls;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace Microsoft.Windows.Automation.Peers
{
    /// <summary>
    /// AutomationPeer for DatePicker Control
    /// </summary>
    public sealed class DatePickerAutomationPeer : FrameworkElementAutomationPeer, IExpandCollapseProvider, IValueProvider
    {
        /// <summary>
        /// Initializes a new instance of the AutomationPeer for DatePicker control.
        /// </summary>
        /// <param name="owner">DatePicker</param>
        public DatePickerAutomationPeer(PersianDatePicker owner)
            : base(owner)
        {
        }

        #region Private Properties

        private PersianDatePicker OwningDatePicker => Owner as PersianDatePicker;

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Gets the control pattern that is associated with the specified System.Windows.Automation.Peers.PatternInterface.
        /// </summary>
        /// <param name="patternInterface">A value from the System.Windows.Automation.Peers.PatternInterface enumeration.</param>
        /// <returns>The object that supports the specified pattern, or null if unsupported.</returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.ExpandCollapse || patternInterface == PatternInterface.Value)
            {
                return this;
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
            return AutomationControlType.Custom;
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
        /// Overrides the GetLocalizedControlTypeCore method for DatePicker
        /// </summary>
        /// <returns></returns>
        protected override string GetLocalizedControlTypeCore()
        {
            return SR.Get(SRID.DatePickerAutomationPeer_LocalizedControlType);
        }

        #endregion Protected Methods

        #region IExpandCollapseProvider

        ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState
        {
            get
            {
                if (OwningDatePicker.IsDropDownOpen)
                {
                    return ExpandCollapseState.Expanded;
                }
                else
                {
                    return ExpandCollapseState.Collapsed;
                }
            }
        }

        void IExpandCollapseProvider.Collapse()
        {
            OwningDatePicker.IsDropDownOpen = false;
        }

        void IExpandCollapseProvider.Expand()
        {
            OwningDatePicker.IsDropDownOpen = true;
        }

        #endregion IExpandCollapseProvider

        #region IValueProvider

        bool IValueProvider.IsReadOnly => false;

        string IValueProvider.Value => OwningDatePicker.ToString();

        void IValueProvider.SetValue(string value)
        {
            OwningDatePicker.Text = value;
        }

        #endregion IValueProvider

        #region Internal Methods
        // Never inline, as we don't want to unnecessarily link the automation DLL
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseValuePropertyChangedEvent(string oldValue, string newValue)
        {
            if (oldValue != newValue)
            {
                RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty, oldValue, newValue);
            }
        }
        #endregion
    }
}
