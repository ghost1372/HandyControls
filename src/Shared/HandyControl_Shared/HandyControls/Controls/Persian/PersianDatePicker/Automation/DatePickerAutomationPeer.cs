//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using HandyControl.Controls;

namespace Microsoft.Windows.Automation.Peers
{
    /// <summary>
    /// AutomationPeer for PersianDatePicker Control
    /// </summary>
    public sealed class DatePickerAutomationPeer : FrameworkElementAutomationPeer, IExpandCollapseProvider, IValueProvider
    {
        /// <summary>
        /// Initializes a new instance of the AutomationPeer for PersianDatePicker control.
        /// </summary>
        /// <param name="owner">PersianDatePicker</param>
        public DatePickerAutomationPeer(PersianDatePicker owner)
            : base(owner)
        {
        }

        #region Private Properties

        private PersianDatePicker OwningPersianDatePicker
        {
            get
            {
                return this.Owner as PersianDatePicker;
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
        /// Overrides the GetLocalizedControlTypeCore method for PersianDatePicker
        /// </summary>
        /// <returns></returns>
        protected override string GetLocalizedControlTypeCore()
        {
            return "date picker";
        }

        #endregion Protected Methods

        #region IExpandCollapseProvider

        ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState
        {
            get
            {
                if (this.OwningPersianDatePicker.IsDropDownOpen)
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
            this.OwningPersianDatePicker.IsDropDownOpen = false;
        }

        void IExpandCollapseProvider.Expand()
        {
            this.OwningPersianDatePicker.IsDropDownOpen = true;
        }

        #endregion IExpandCollapseProvider

        #region IValueProvider

        bool IValueProvider.IsReadOnly 
        { 
            get { return false; } 
        }

        string IValueProvider.Value 
        {
            get { return this.OwningPersianDatePicker.ToString(); } 
        }

        void IValueProvider.SetValue(string value)
        {
            this.OwningPersianDatePicker.Text = value;
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
