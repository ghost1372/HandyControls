using HandyControl.Tools;
namespace $safeprojectname$
{
    public partial class App
    {
        internal void UpdateSkin(ApplicationTheme theme)
        {
            ThemeManager.Current.ApplicationTheme = theme;
        }
    }
}
