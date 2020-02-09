using System;
using System.Windows;
using HandyControl.Data;
namespace $safeprojectname$
{
    public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConfigHelper.Instance.SetWindowDefaultStyle();
        }
        
    internal void UpdateSkin(SkinType skin)
    {
        Resources.MergedDictionaries.Clear();
        Resources.MergedDictionaries.Add(new ResourceDictionary
        {
            Source = new Uri($"pack://application:,,,/HandyControl;component/Themes/Skin{skin.ToString()}.xaml")
        });
        Resources.MergedDictionaries.Add(new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/HandyControl;component/Themes/Theme.xaml")
        });
    }
}
}
