using HandyControl.Tools;
using System.Windows.Media;
namespace $safeprojectname$
{
    public partial class App
    {
        internal void UpdateTheme(ApplicationTheme theme)
        {
            if (ThemeManager.Current.ApplicationTheme != theme)
            {
                ThemeManager.Current.ApplicationTheme = theme;
            }
        }

        internal void UpdateAccent(Brush accent)
        {
            if (ThemeManager.Current.ActualAccentColor != accent)
            {
                ThemeManager.Current.ActualAccentColor = accent;
            }
        }
    }
}
