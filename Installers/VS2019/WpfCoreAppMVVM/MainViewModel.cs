using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
namespace $safeprojectname$.ViewModels
{
    public class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        Messenger.Default.Register<object>(this, MessageToken.LoadShowContent, obj =>
        {
            if (SubContent is IDisposable disposable)
            {
                disposable.Dispose();
            }
            SubContent = obj;
        });
    }
    private object _contentTitle;
    public object ContentTitle
    {
        get => _contentTitle;
        set => Set(ref _contentTitle, value);
    }

    private object _subContent;
    public object SubContent
    {
        get => _subContent;
        set => Set(ref _subContent, value);
    }

    public RelayCommand OpenOverviewCmd => new Lazy<RelayCommand>(() =>
        new RelayCommand(OpenOverview)).Value;

    private void OpenOverview()
    {
        ContentTitle = MessageToken.OverView;
        Messenger.Default.Send(AssemblyHelper.CreateInternalInstance($"View.{MessageToken.OverView}"), MessageToken.LoadShowContent);

        // If you want to pass a variable between views, Use one of the following codes
        //Messenger.Default.Send("Hello", MessageToken.SimpleMessage);
        //Messenger.Default.Send(new NotificationMessage("Hello"));
    }
}
}
