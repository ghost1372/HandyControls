using System;
using System.Windows;
using HandyControl.Data;
namespace $safeprojectname$
{
    public partial class App
    {
        internal void UpdateSkin(SkinType skin)
        {
            ResourceHelper.GetTheme("HandyTheme", Resources).Skin = skin;
            Current.MainWindow?.OnApplyTemplate();
        }
}
}
