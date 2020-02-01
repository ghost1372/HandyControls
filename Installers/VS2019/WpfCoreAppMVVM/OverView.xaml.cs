using GalaSoft.MvvmLight.Messaging;
using System.Windows;
namespace $safeprojectname$.View
{
    /// <summary>
    /// Interaction logic for OverView.xaml
    /// </summary>
    public partial class OverView
{
    public OverView()
    {
        InitializeComponent();

        //Get passed variable, Use code according to the method used
        //Messenger.Default.Register<string>(this, MessageToken.SimpleMessage, GetMessage);
        //Messenger.Default.Register<NotificationMessage>(this, GetMessageX);
    }

    //private void GetMessageX(NotificationMessage obj)
    //{
    //    MessageBox.Show(obj.Notification);
    //}

    //private void GetMessage(string simpleMessage)
    //{
    //    MessageBox.Show(simpleMessage);
    //}
}
}
