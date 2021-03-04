//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Windows.Controls.Primitives
{
    /// <summary>
    /// DatePickerTextBox is a specialized form of TextBox which displays custom visuals when its contents are empty
    /// </summary>
    [TemplatePart(Name = DatePickerTextBox.ElementContentName, Type = typeof(ContentControl))]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnwatermarked, GroupName = VisualStates.GroupWatermark)]
    [TemplateVisualState(Name = VisualStates.StateWatermarked, GroupName = VisualStates.GroupWatermark)]
    public sealed partial class DatePickerTextBox : TextBox
    {
        #region Constants
        private const string ElementContentName = "Watermark";

        #endregion

        #region Data

        private ContentControl elementContent;
        private bool isHovered;

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor
        /// </summary>
        static DatePickerTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DatePickerTextBox), new FrameworkPropertyMetadata(typeof(DatePickerTextBox)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatePickerTextBox"/> class.
        /// </summary>
        public DatePickerTextBox()
        {
            this.Watermark = "<Enter text here>";
            this.Loaded += OnLoaded;
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnDatePickerTextBoxIsEnabledChanged);
        }
        #endregion

        #region Public Properties

        #region Watermark
        /// <summary>
        /// Watermark dependency property
        /// </summary>
        internal static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(
            "Watermark", typeof(object), typeof(DatePickerTextBox), new PropertyMetadata(OnWatermarkPropertyChanged));

        /// <summary>
        /// Watermark content
        /// </summary>
        /// <value>The watermark.</value>
        internal object Watermark
        {
            get { return (object)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        #endregion

        #endregion Public Properties

        #region Protected

        /// <summary>
        /// Called when template is applied to the control.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            elementContent = ExtractTemplatePart<ContentControl>(ElementContentName);

            OnWatermarkChanged();

            ChangeVisualState(false);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (IsEnabled)
            {
                if (!string.IsNullOrEmpty(this.Text))
                {
                    Select(0, this.Text.Length);
                }

                ChangeVisualState(true);
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            ChangeVisualState(true);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            this.isHovered = true;

            if (!IsFocused)
            {
                ChangeVisualState(true);
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            isHovered = false;

            if (!IsFocused)
            {
                ChangeVisualState(true);
            }
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            ChangeVisualState(true);
        }

        #endregion Protected

        #region Private

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ApplyTemplate();
            ChangeVisualState(false);
        }

        /// <summary>
        /// Change to the correct visual state for the textbox.
        /// </summary>
        /// <param name="useTransitions">
        /// true to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        private void ChangeVisualState(bool useTransitions)
        {
            // Update the CommonStates group
            if (!IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateDisabled, VisualStates.StateNormal);
            }
            else if (isHovered)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateMouseOver, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateNormal);
            }

            // Update the FocusStates group
            if (IsFocused && IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateFocused, VisualStates.StateUnfocused);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateUnfocused);
            }

            // Update the WatermarkStates group
            if (this.Watermark != null && string.IsNullOrEmpty(this.Text))
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateWatermarked, VisualStates.StateUnwatermarked);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateUnwatermarked);
            }
        }

        private T ExtractTemplatePart<T>(string partName) where T : DependencyObject
        {
            DependencyObject obj = GetTemplateChild(partName);
            return ExtractTemplatePart<T>(partName, obj);
        }

        private static T ExtractTemplatePart<T>(string partName, DependencyObject obj) where T : DependencyObject
        {
            Debug.Assert(
                obj == null || typeof(T).IsInstanceOfType(obj),
                string.Format(CultureInfo.InvariantCulture, "The template part {0} is not an instance of {1}.", partName, typeof(T).Name));
            return obj as T;
        }

        /// <summary>
        /// Called when the IsEnabled property changes.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Property changed args</param>
        private void OnDatePickerTextBoxIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Debug.Assert(e.NewValue is bool);
            bool isEnabled = (bool)e.NewValue;

            IsReadOnly = !isEnabled;

            ChangeVisualState(true);
        }

        private void OnWatermarkChanged()
        {
            if (elementContent != null)
            {
                Control watermarkControl = this.Watermark as Control;
                if (watermarkControl != null)
                {
                    watermarkControl.IsTabStop = false;
                    watermarkControl.IsHitTestVisible = false;
                }
            }
        }

        /// <summary>
        /// Called when watermark property is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWatermarkPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            DatePickerTextBox datePickerTextBox = sender as DatePickerTextBox;
            Debug.Assert(datePickerTextBox != null, "The source is not an instance of a DatePickerTextBox!");
            datePickerTextBox.OnWatermarkChanged();
            datePickerTextBox.ChangeVisualState(true);
        }

        #endregion Private
    }
}

