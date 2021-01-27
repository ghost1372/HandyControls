using HandyControl.Tools;
using System.Windows;
namespace $safeprojectname$
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var boot = new Bootstrapper();
            boot.Run();
        }

        internal void UpdateSkin(ApplicationTheme theme)
        {
            ThemeManager.Current.ApplicationTheme = theme;
        }
    }
}
