using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HandyControl.Controls
{
    public class Navigation : TabControl
    {
        public static readonly DependencyProperty TabPanelVerticalAlignmentProperty = ElementBase.Property<Navigation, VerticalAlignment>(nameof(TabPanelVerticalAlignmentProperty), VerticalAlignment.Top);
        public static readonly DependencyProperty OffsetProperty = ElementBase.Property<Navigation, Thickness>(nameof(OffsetProperty), new Thickness(0));
        public static readonly DependencyProperty IconModeProperty = ElementBase.Property<Navigation, bool>(nameof(IconModeProperty), false);

        public static RoutedUICommand IconModeClickCommand = ElementBase.Command<Navigation>(nameof(IconModeClickCommand));

        public VerticalAlignment TabPanelVerticalAlignment { get { return (VerticalAlignment)GetValue(TabPanelVerticalAlignmentProperty); } set { SetValue(TabPanelVerticalAlignmentProperty, value); } }
        public Thickness Offset { get { return (Thickness)GetValue(OffsetProperty); } set { SetValue(OffsetProperty, value); } }
        public bool IconMode { get { return (bool)GetValue(IconModeProperty); } set { SetValue(IconModeProperty, value); GoToState(); } }

        void GoToState()
        {
            ElementBase.GoToState(this, IconMode ? "EnterIconMode" : "ExitIconMode");
        }

        void SelectionState()
        {
            if (IconMode)
            {
                ElementBase.GoToState(this, "SelectionStartIconMode");
                ElementBase.GoToState(this, "SelectionEndIconMode");
            }
            else
            {
                ElementBase.GoToState(this, "SelectionStart");
                ElementBase.GoToState(this, "SelectionEnd");
            }
        }

        public Navigation()
        {
            Loaded += delegate { GoToState(); ElementBase.GoToState(this, IconMode ? "SelectionLoadedIconMode" : "SelectionLoaded"); };
            SelectionChanged += delegate (object sender, SelectionChangedEventArgs e) { if (e.Source is Navigation) { SelectionState(); } };
            CommandBindings.Add(new CommandBinding(IconModeClickCommand, delegate { IconMode = !IconMode; GoToState(); }));
        }

        static Navigation()
        {
            ElementBase.DefaultStyle<Navigation>(DefaultStyleKeyProperty);
        }
    }
}
