using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using HandyControl.Data;
using HandyControl.Interactivity;

namespace HandyControl.Controls
{
    [TemplatePart(Name = ElementRoot, Type = typeof(Grid))]
    [TemplatePart(Name = ElementTextBox, Type = typeof(WatermarkTextBox))]
    [TemplatePart(Name = ElementButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementPopup, Type = typeof(Popup))]
    public class ColorPickerBox : Control, IDataInput
    {
        #region Constants

        private const string ElementRoot = "PART_Root";

        private const string ElementTextBox = "PART_TextBox";

        private const string ElementButton = "PART_Button";

        private const string ElementPopup = "PART_Popup";

        #endregion Constants

        #region Data

        private ColorPicker _colorPicker;

        private string _defaultText;

        private ButtonBase _dropDownButton;

        private Popup _popup;

        private bool _disablePopupReopen;

        private WatermarkTextBox _textBox;

        private IDictionary<DependencyProperty, bool> _isHandlerSuspended;

        private Brush _originalSelectedBrush;

        #endregion Data

        #region Public Events

        public static readonly RoutedEvent SelectedBrushChangedEvent =
            EventManager.RegisterRoutedEvent("SelectedColorChanged", RoutingStrategy.Direct,
                typeof(EventHandler<FunctionEventArgs<Brush>>), typeof(ColorPickerBox));

        public event EventHandler<FunctionEventArgs<Brush>> SelectedBrushChanged
        {
            add => AddHandler(SelectedBrushChangedEvent, value);
            remove => RemoveHandler(SelectedBrushChangedEvent, value);
        }

        public event RoutedEventHandler PickerClosed;

        public event RoutedEventHandler PickerOpened;

        #endregion Public Events

        static ColorPickerBox()
        {
            EventManager.RegisterClassHandler(typeof(ColorPickerBox), GotFocusEvent, new RoutedEventHandler(OnGotFocus));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(ColorPickerBox), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(ColorPickerBox), new FrameworkPropertyMetadata(ValueBoxes.FalseBox));
        }
        public ColorPickerBox()
        {
            InitColorPicker();
            CommandBindings.Add(new CommandBinding(ControlCommands.Clear, (s, e) =>
            {
                SetCurrentValue(SelectedBrushProperty, null);
                SetCurrentValue(TextProperty, "");
                _textBox.Text = string.Empty;
            }));
        }

        #region public properties
        public static readonly DependencyProperty ColorPickerStyleProperty = DependencyProperty.Register(
        nameof(ColorPickerStyle), typeof(Style), typeof(ColorPickerBox), new PropertyMetadata(default(Style)));

        public Style ColorPickerStyle
        {
            get => (Style) GetValue(ColorPickerStyleProperty);
            set => SetValue(ColorPickerStyleProperty, value);
        }

        //public static readonly DependencyProperty DisplayBrushProperty = DependencyProperty.Register(
        //nameof(DisplayBrush), typeof(SolidColorBrush), typeof(ColorPickerBox), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, CoerceDisplayBrush));

        //private static object CoerceDisplayBrush(DependencyObject d, object value)
        //{
        //    var cpb = (ColorPickerBox) d;
        //        cpb._colorPicker.SelectedBrush = (SolidColorBrush)value;


        //    return cpb._colorPicker.SelectedBrush;
        //}

        //public SolidColorBrush DisplayBrush
        //{
        //    get => (SolidColorBrush) GetValue(DisplayBrushProperty);
        //    set => SetValue(DisplayBrushProperty, value);
        //}

        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(
       nameof(IsDropDownOpen), typeof(bool), typeof(ColorPickerBox), new FrameworkPropertyMetadata(ValueBoxes.FalseBox, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsDropDownOpenChanged, OnCoerceIsDropDownOpen));

        private static object OnCoerceIsDropDownOpen(DependencyObject d, object baseValue) =>
            d is ColorPickerBox
            {
                IsEnabled: false
            }
                ? false
                : baseValue;

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as ColorPickerBox;

            var newValue = (bool) e.NewValue;
            if (dp?._popup != null && dp._popup.IsOpen != newValue)
            {
                dp._popup.IsOpen = newValue;
                if (newValue)
                {
                    dp._originalSelectedBrush = dp.SelectedBrush;

                    dp.Dispatcher.BeginInvoke(DispatcherPriority.Input, (Action) delegate
                    {
                        dp._colorPicker.Focus();
                    });
                }
            }
        }

        public bool IsDropDownOpen
        {
            get => (bool) GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, ValueBoxes.BooleanBox(value));
        }

        public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register(
        nameof(SelectedBrush), typeof(Brush), typeof(ColorPickerBox), new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedBrushChanged, CoerceSelectedBrush));


        private static object CoerceSelectedBrush(DependencyObject d, object basevalue)
        {
            return basevalue is Brush ? basevalue : Brushes.White;
        }


        private static void OnSelectedBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ColorPickerBox dp) return;

            if (dp.SelectedBrush != null)
            {
                var brush = dp.SelectedBrush.ToString();
                dp.SetTextInternal(brush);
            }

            dp.RaiseEvent(new FunctionEventArgs<Brush>(SelectedBrushChangedEvent, dp)
            {
                Info = dp.SelectedBrush
            });
        }

        public Brush SelectedBrush
        {
            get => (Brush) GetValue(SelectedBrushProperty);
            set => SetValue(SelectedBrushProperty, value);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text), typeof(string), typeof(ColorPickerBox), new FrameworkPropertyMetadata(string.Empty, OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPickerBox dp && !dp.IsHandlerSuspended(TextProperty))
            {
                if (e.NewValue is string newValue)
                {
                    if (dp._textBox != null)
                    {
                        dp._textBox.Text = newValue;
                    }
                    else
                    {
                        dp._defaultText = newValue;
                    }

                    dp.SetSelectedBrush();
                }
                else
                {
                    dp.SetValueNoCallback(SelectedBrushProperty, null);
                }
            }
        }

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        /// <summary>
        /// Sets the local Text property without breaking bindings
        /// </summary>
        /// <param name="value"></param>
        private void SetTextInternal(string value)
        {
            SetCurrentValue(TextProperty, value);
        }

        public Func<string, OperationResult<bool>> VerifyFunc { get; set; }

        public static readonly DependencyProperty IsErrorProperty = DependencyProperty.Register(
            nameof(IsError), typeof(bool), typeof(ColorPickerBox), new PropertyMetadata(ValueBoxes.FalseBox));

        public bool IsError
        {
            get => (bool) GetValue(IsErrorProperty);
            set => SetValue(IsErrorProperty, ValueBoxes.BooleanBox(value));
        }

        public static readonly DependencyProperty ErrorStrProperty = DependencyProperty.Register(
            nameof(ErrorStr), typeof(string), typeof(ColorPickerBox), new PropertyMetadata(default(string)));

        public string ErrorStr
        {
            get => (string) GetValue(ErrorStrProperty);
            set => SetValue(ErrorStrProperty, value);
        }

        public static readonly DependencyProperty TextTypeProperty = DependencyProperty.Register(
            nameof(TextType), typeof(TextType), typeof(ColorPickerBox), new PropertyMetadata(default(TextType)));

        public TextType TextType
        {
            get => (TextType) GetValue(TextTypeProperty);
            set => SetValue(TextTypeProperty, value);
        }

        public static readonly DependencyProperty ShowClearButtonProperty = DependencyProperty.Register(
            nameof(ShowClearButton), typeof(bool), typeof(ColorPickerBox), new PropertyMetadata(ValueBoxes.FalseBox));

        public bool ShowClearButton
        {
            get => (bool) GetValue(ShowClearButtonProperty);
            set => SetValue(ShowClearButtonProperty, ValueBoxes.BooleanBox(value));
        }

        public static readonly DependencyProperty SelectionBrushProperty =
            TextBoxBase.SelectionBrushProperty.AddOwner(typeof(ColorPickerBox));

        public Brush SelectionBrush
        {
            get => (Brush) GetValue(SelectionBrushProperty);
            set => SetValue(SelectionBrushProperty, value);
        }

#if !(NET40 || NET45 || NET451 || NET452 || NET46 || NET461 || NET462 || NET47 || NET471 || NET472)

        public static readonly DependencyProperty SelectionTextBrushProperty =
            TextBoxBase.SelectionTextBrushProperty.AddOwner(typeof(ColorPickerBox));

        public Brush SelectionTextBrush
        {
            get => (Brush) GetValue(SelectionTextBrushProperty);
            set => SetValue(SelectionTextBrushProperty, value);
        }

#endif

        public static readonly DependencyProperty SelectionOpacityProperty =
            TextBoxBase.SelectionOpacityProperty.AddOwner(typeof(ColorPickerBox));

        public double SelectionOpacity
        {
            get => (double) GetValue(SelectionOpacityProperty);
            set => SetValue(SelectionOpacityProperty, value);
        }

        public static readonly DependencyProperty CaretBrushProperty =
            TextBoxBase.CaretBrushProperty.AddOwner(typeof(ColorPickerBox));

        public Brush CaretBrush
        {
            get => (Brush) GetValue(CaretBrushProperty);
            set => SetValue(CaretBrushProperty, value);
        }
        #endregion public properties

        #region Public Methods

        public override void OnApplyTemplate()
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            if (_popup != null)
            {
                _popup.PreviewMouseLeftButtonDown -= PopupPreviewMouseLeftButtonDown;
                _popup.Opened -= PopupOpened;
                _popup.Closed -= PopupClosed;
                _popup.Child = null;
            }

            if (_dropDownButton != null)
            {
                _dropDownButton.Click -= DropDownButton_Click;
                _dropDownButton.MouseLeave -= DropDownButton_MouseLeave;
            }

            if (_textBox != null)
            {
                _textBox.KeyDown -= TextBox_KeyDown;
                _textBox.TextChanged -= TextBox_TextChanged;
                _textBox.LostFocus -= TextBox_LostFocus;
            }

            base.OnApplyTemplate();

            _popup = GetTemplateChild(ElementPopup) as Popup;
            _dropDownButton = GetTemplateChild(ElementButton) as Button;
            _textBox = GetTemplateChild(ElementTextBox) as WatermarkTextBox;

            CheckNull();

            _popup.PreviewMouseLeftButtonDown += PopupPreviewMouseLeftButtonDown;
            _popup.Opened += PopupOpened;
            _popup.Closed += PopupClosed;
            _popup.Child = _colorPicker;

            _dropDownButton.Click += DropDownButton_Click;
            _dropDownButton.MouseLeave += DropDownButton_MouseLeave;

            var selectedBrush = SelectedBrush;

            if (_textBox != null)
            {
                if (selectedBrush == null)
                {
                    _textBox.Text = Brushes.Transparent.ToString();
                }

                _textBox.SetBinding(SelectionBrushProperty, new Binding(SelectionBrushProperty.Name) { Source = this });
#if !(NET40 || NET45 || NET451 || NET452 || NET46 || NET461 || NET462 || NET47 || NET471 || NET472)
                _textBox.SetBinding(SelectionTextBrushProperty, new Binding(SelectionTextBrushProperty.Name) { Source = this });
#endif
                _textBox.SetBinding(SelectionOpacityProperty, new Binding(SelectionOpacityProperty.Name) { Source = this });
                _textBox.SetBinding(CaretBrushProperty, new Binding(CaretBrushProperty.Name) { Source = this });

                _textBox.KeyDown += TextBox_KeyDown;
                _textBox.TextChanged += TextBox_TextChanged;
                _textBox.LostFocus += TextBox_LostFocus;

                if (selectedBrush == null)
                {
                    if (!string.IsNullOrEmpty(_defaultText))
                    {
                        _textBox.Text = _defaultText;
                        SetSelectedBrush();
                    }
                }
                else
                {
                    _textBox.Text = selectedBrush.ToString();
                }
            }

            if (selectedBrush is null)
            {
                _originalSelectedBrush ??= Brushes.Transparent;
                //SetCurrentValue(DisplayBrushProperty, _originalSelectedBrush);
            }
            else
            {
                //SetCurrentValue(DisplayBrushProperty, selectedBrush);
            }
        }

        public virtual bool VerifyData()
        {
            OperationResult<bool> result;

            if (VerifyFunc != null)
            {
                result = VerifyFunc.Invoke(Text);
            }
            else
            {
                if (!string.IsNullOrEmpty(Text))
                {
                    result = OperationResult.Success();
                }
                else if (InfoElement.GetNecessary(this))
                {
                    result = OperationResult.Failed(Properties.Langs.Lang.IsNecessary);
                }
                else
                {
                    result = OperationResult.Success();
                }
            }

            var isError = !result.Data;
            if (isError)
            {
                SetCurrentValue(IsErrorProperty, ValueBoxes.TrueBox);
                SetCurrentValue(ErrorStrProperty, result.Message);
            }
            else
            {
                isError = Validation.GetHasError(this);
                if (isError)
                {
                    SetCurrentValue(ErrorStrProperty, Validation.GetErrors(this)[0].ErrorContent?.ToString());
                }
                else
                {
                    SetCurrentValue(IsErrorProperty, ValueBoxes.FalseBox);
                    SetCurrentValue(ErrorStrProperty, default(string));
                }
            }
            return !isError;
        }

        public override string ToString() => SelectedBrush?.ToString() ?? string.Empty;

        #endregion

        #region Protected Methods

        protected virtual void OnPickerClosed(RoutedEventArgs e)
        {
            var handler = PickerClosed;
            handler?.Invoke(this, e);
        }

        protected virtual void OnPickerOpened(RoutedEventArgs e)
        {
            var handler = PickerOpened;
            handler?.Invoke(this, e);
        }

        #endregion Protected Methods

        #region Private Methods

        private void CheckNull()
        {
            if (_dropDownButton == null || _popup == null || _textBox == null)
                throw new Exception();
        }

        private void InitColorPicker()
        {
            _colorPicker = new ColorPicker()
            {
                ShowConfirmButton = false
            };
            _colorPicker.SelectedColorChanged += ColorPicker_SelectedBrushChanged;
            if (_colorPicker.ShowConfirmButton)
                _colorPicker.Confirmed += ColorPicker_Confirmed;
        }

        private void ColorPicker_Confirmed(object sender, FunctionEventArgs<Color> e) => TogglePopup();

        private void ColorPicker_SelectedBrushChanged(object sender, FunctionEventArgs<Color> e) => SelectedBrush = new SolidColorBrush(e.Info);


        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SetSelectedBrush();
        }

        private void SetIsHandlerSuspended(DependencyProperty property, bool value)
        {
            if (value)
            {
                _isHandlerSuspended ??= new Dictionary<DependencyProperty, bool>(2);
                _isHandlerSuspended[property] = true;
            }
            else
            {
                _isHandlerSuspended?.Remove(property);
            }
        }

        private void SetValueNoCallback(DependencyProperty property, object value)
        {
            SetIsHandlerSuspended(property, true);
            try
            {
                SetCurrentValue(property, value);
            }
            finally
            {
                SetIsHandlerSuspended(property, false);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetValueNoCallback(TextProperty, _textBox.Text);
            VerifyData();
        }

        private bool ProcessColorPickerKey(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.System:
                    {
                        switch (e.SystemKey)
                        {
                            case Key.Down:
                                {
                                    if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                                    {
                                        TogglePopup();
                                        return true;
                                    }

                                    break;
                                }
                        }

                        break;
                    }

                case Key.Enter:
                    {
                        SetSelectedBrush();
                        return true;
                    }
            }

            return false;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = ProcessColorPickerKey(e) || e.Handled;
        }

        private void DropDownButton_MouseLeave(object sender, MouseEventArgs e)
        {
            _disablePopupReopen = false;
        }

        private bool IsHandlerSuspended(DependencyProperty property)
        {
            return _isHandlerSuspended != null && _isHandlerSuspended.ContainsKey(property);
        }

        private void PopupPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Popup { StaysOpen: false })
            {
                if (_dropDownButton?.InputHitTest(e.GetPosition(_dropDownButton)) != null)
                {
                    _disablePopupReopen = true;
                }
            }
        }

        private void PopupOpened(object sender, EventArgs e)
        {
            if (!IsDropDownOpen)
            {
                SetCurrentValue(IsDropDownOpenProperty, ValueBoxes.TrueBox);
            }

            _colorPicker?.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

            OnPickerOpened(new RoutedEventArgs());
        }

        private void PopupClosed(object sender, EventArgs e)
        {
            if (IsDropDownOpen)
            {
                SetCurrentValue(IsDropDownOpenProperty, ValueBoxes.FalseBox);
            }

            if (_colorPicker.IsKeyboardFocusWithin)
            {
                MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }

            OnPickerClosed(new RoutedEventArgs());
        }

        private void DropDownButton_Click(object sender, RoutedEventArgs e) => TogglePopup();

        private void TogglePopup()
        {
            if (IsDropDownOpen)
            {
                SetCurrentValue(IsDropDownOpenProperty, ValueBoxes.FalseBox);
            }
            else
            {
                if (_disablePopupReopen)
                {
                    _disablePopupReopen = false;
                }
                else
                {
                    SetSelectedBrush();
                    SetCurrentValue(IsDropDownOpenProperty, ValueBoxes.TrueBox);
                }
            }
        }

        private void SafeSetText(string s)
        {
            if (string.Compare(Text, s, StringComparison.Ordinal) != 0)
            {
                SetCurrentValue(TextProperty, s);
            }
        }

        private SolidColorBrush ParseText(string text)
        {
            try
            {
                var color = (Color) ColorConverter.ConvertFromString(text);
                if (color != null)
                {
                    var brush = new SolidColorBrush(color);
                    return brush;
                }
            }
            catch
            {
                // ignored
            }

            return null;
        }

        private Brush SetTextBoxValue(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                SafeSetText(s);
                return SelectedBrush;
            }

            var d = ParseText(s);

            if (d is SolidColorBrush colorBrush)
            {
                SafeSetText(colorBrush.ToString());
                return colorBrush;
            }

            if (SelectedBrush != null)
            {
                var newtext = SelectedBrush.ToString();
                SafeSetText(newtext);
                return SelectedBrush;
            }
            //SafeSetText(DateTimeToString(DisplayDateTime));
            return null;
        }

        private void SetSelectedBrush()
        {
            if (_textBox != null)
            {
                if (!string.IsNullOrEmpty(_textBox.Text))
                {
                    var s = _textBox.Text;

                    if (SelectedBrush != null)
                    {
                        //if (SelectedBrush != DisplayBrush)
                        //{
                        //    SetCurrentValue(DisplayBrushProperty, SelectedBrush);
                        //}

                        var selectedBrush = SelectedBrush.ToString();

                        if (string.Compare(selectedBrush, s, StringComparison.Ordinal) == 0)
                        {
                            return;
                        }
                    }

                    var d = SetTextBoxValue(s);
                    if (!SelectedBrush.Equals(d))
                    {
                        SetCurrentValue(SelectedBrushProperty, d);
                        //SetCurrentValue(DisplayBrushProperty, d);
                    }
                }
                else
                {
                    if (SelectedBrush != null)
                    {
                        SetCurrentValue(SelectedBrushProperty, null);
                    }
                }
            }
            else
            {
                var d = SetTextBoxValue(_defaultText);
                if (!SelectedBrush.Equals(d))
                {
                    SetCurrentValue(SelectedBrushProperty, d);
                }
            }
        }

        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            var picker = (ColorPickerBox) sender;
            if (!e.Handled && picker._textBox != null)
            {
                if (Equals(e.OriginalSource, picker))
                {
                    picker._textBox.Focus();
                    e.Handled = true;
                }
                else if (Equals(e.OriginalSource, picker._textBox))
                {
                    picker._textBox.SelectAll();
                    e.Handled = true;
                }
            }
        }

        #endregion
    }
}
