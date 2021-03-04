using HandyControl.Tools.Interop;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading;

namespace HandyControl.Tools
{
    public class InstanceHelper
    {
        internal static Mutex mutex;

        /// <summary>
        /// Check that only one instance of the program runs
        /// </summary>
        /// <param name="AssemblyName"></param>
        /// <returns></returns>
        public static bool IsSingleInstance(string AssemblyName = null)
        {
            if (string.IsNullOrEmpty(AssemblyName))
            {
                AssemblyName = Assembly.GetCallingAssembly().GetName().Name;
            }
            mutex = new Mutex(true, AssemblyName);
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                mutex.ReleaseMutex();
                return true;
            }
            else
            {
                BringWindowToFront();
                Environment.Exit(0);
                return false;
            }
        }

        public static void BringWindowToFront()
        {
            var currentProcess = Process.GetCurrentProcess();
            var processes = Process.GetProcessesByName(currentProcess.ProcessName);
            var process = processes.FirstOrDefault(p => p.Id != currentProcess.Id);
            if (process == null) return;
            InteropMethods.SetForegroundWindow(process.MainWindowHandle);
        }

        /// <summary>
        /// Check if Running Application runs with admin access or not
        /// </summary>
        public static void IsAdministrator()
        {
            if (!(new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                .IsInRole(WindowsBuiltInRole.Administrator))
                throw new UnauthorizedAccessException();
        }
    }
}
