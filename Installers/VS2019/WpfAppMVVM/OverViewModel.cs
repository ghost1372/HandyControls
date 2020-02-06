using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;

namespace $safeprojectname$.ViewModels
{
    public class OverViewModel : ViewModelBase
    {
        public OverViewModel()
        {
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
