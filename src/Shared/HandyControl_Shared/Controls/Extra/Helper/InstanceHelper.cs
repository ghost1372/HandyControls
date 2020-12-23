using HandyControl.Tools.Interop;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading;

namespace HandyControl.Controls
{
    public class InstanceHelper
    {
        static Mutex mutex;

        /// <summary>
        /// check only one instance is running 
        /// </summary>
        /// <param name="ShowErrorMessage"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        public static bool IsSingleInstance(bool ShowErrorMessage = true, string ErrorMessage = "Another instance of the app is running")
        {
            var guid = CryptographyHelper.GenerateMD5(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            mutex = new Mutex(true, "{" + $"{guid}" + "}");
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                mutex.ReleaseMutex();
                return true;
            }
            else
            {
                if (ShowErrorMessage)
                {
                    MessageBox.Error(ErrorMessage);
                }
                BringWindowToFront();
                Environment.Exit(0);
                return false;
            }
        }

        private static void BringWindowToFront()
        {
            var currentProcess = Process.GetCurrentProcess();
            var processes = Process.GetProcessesByName(currentProcess.ProcessName);
            var process = processes.FirstOrDefault(p => p.Id != currentProcess.Id);
            if (process == null) return;
            InteropMethods.SetForegroundWindow(process.MainWindowHandle);
        }

        public static void IsAdministrator()
        {
            if (!(new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                .IsInRole(WindowsBuiltInRole.Administrator))
                throw new UnauthorizedAccessException();
        }
    }
}
