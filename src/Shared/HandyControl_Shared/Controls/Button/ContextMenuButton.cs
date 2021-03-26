using System.Windows.Controls;


namespace HandyControl.Controls
{
    /// <summary>
    ///     Button with context menu
    /// </summary>
    public class ContextMenuButton : Button
    {
        public ContextMenu Menu { get; set; }

        protected override void OnClick()
        {
            base.OnClick();
            if (Menu != null)
            {
                Menu.PlacementTarget = this;
                Menu.IsOpen = true;
            }
        }
    }
}
