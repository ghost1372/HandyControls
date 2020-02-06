using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Windows;
namespace $safeprojectname$.ViewModels
{
    public class ViewModelLocator
{
    public ViewModelLocator()
    {
        ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

        // Register ViewModels Here

        SimpleIoc.Default.Register<MainViewModel>();
        SimpleIoc.Default.Register<OverViewModel>();
    }

    public static ViewModelLocator Instance => new Lazy<ViewModelLocator>(() =>
        Application.Current.TryFindResource("Locator") as ViewModelLocator).Value;

    #region Vm

    public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
    public OverViewModel Over => ServiceLocator.Current.GetInstance<OverViewModel>();

    #endregion
}
}
