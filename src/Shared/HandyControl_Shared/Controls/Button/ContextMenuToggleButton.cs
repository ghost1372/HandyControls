using System.Windows.Controls;
using System.Windows.Controls.Primitives;


namespace HandyControl.Controls
{
    /// <summary>
    ///     Toggle button with context menu
    /// </summary>
    public class ContextMenuToggleButton : ToggleButton
    {
        public ContextMenu Menu { get; set; }

        protected override void OnClick()
        {
            base.OnClick();
            if (Menu != null)
            {
                if (IsChecked == true)
                {
                    Menu.PlacementTarget = this;
                    Menu.IsOpen = true;
                }
                else
                {
                    Menu.IsOpen = false;
                }
            }
        }
    }
}
