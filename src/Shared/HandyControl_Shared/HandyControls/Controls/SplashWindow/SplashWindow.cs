using System;
using System.ComponentModel;
using System.Threading;

namespace HandyControl.Controls
{
    public class SplashWindow : HandyControl.Controls.Window, ISplashWindow, INotifyPropertyChanged
    {
        public static SplashWindow Instance;

        private static ManualResetEvent ResetSplashCreated;
        private static Thread SplashThread;

        /// <summary>
        /// Initialize Splash Window
        /// </summary>
        /// <param name="SplashWindow">Create Splash Window and return it.</param>
        /// <returns>
        /// SplashWindow
        /// </returns>
        /// <example>
        /// <code>
        /// SplashWindow splash = new SplashWindow();
        /// return splash;
        /// </code>
        /// </example>
        public static void Init(Func<SplashWindow> SplashWindow)
        {
            ResetSplashCreated = new ManualResetEvent(false);

            SplashThread = new Thread(() => {
                var splash = SplashWindow();
                Instance = splash;
                splash.Show();

                ResetSplashCreated.Set();
                System.Windows.Threading.Dispatcher.Run();
            });
            SplashThread.SetApartmentState(ApartmentState.STA);
            SplashThread.IsBackground = true;
            SplashThread.Name = "Splash Screen";
            SplashThread.Start();

            ResetSplashCreated.WaitOne();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        private string _Message;
        public string Message
        {
            get { return _Message; }
            set
            {
                _Message = value;
                OnPropertyRaised("Message");
            }
        }

        /// <summary>
        /// Add Status Messages
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(string message)
        {
            Dispatcher.Invoke((Action)delegate ()
            {
                Message = message;
            });
        }

        /// <summary>
        /// Close Splash Window
        /// </summary>
        public void LoadComplete()
        {
            Dispatcher.InvokeShutdown();
        }
    }
}
