using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace HandyControl.Controls
{
    public class InstanceHelper
    {
       
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        static Mutex mutex;

        /// <summary>
        /// check only one instance is running 
        /// example: IsRunOnlyOneInstance("8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F");
        /// </summary>
        /// <param name="MutexName">Random Strings (you can use GUID)</param>
        /// <param name="ShowErrorMessage"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        public static bool IsSingleInstance(bool ShowErrorMessage = true, string ErrorMessage = "Another instance of the app is running")
        {
            var guid = System.Runtime.InteropServices.Marshal.GetTypeLibGuidForAssembly(System.Reflection.Assembly.GetExecutingAssembly()).ToString();
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

        static void BringWindowToFront()
        {
            var currentProcess = Process.GetCurrentProcess();
            var processes = Process.GetProcessesByName(currentProcess.ProcessName);
            var process = processes.FirstOrDefault(p => p.Id != currentProcess.Id);
            if (process == null) return;

            SetForegroundWindow(process.MainWindowHandle);
        }
    }
}
