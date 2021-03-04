using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using HandyControl.Tools.Interop;

namespace HandyControl.Tools
{
    public class ApplicationHelper
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

        [DllImport(InteropValues.ExternDll.WinInet)]
        private static extern bool InternetGetConnectedState(out int Description, int ReservedValue);
        public static bool IsConnectedToInternet()
        {
            return InternetGetConnectedState(out int Desc, 0);
        }

        public static Uri GetAbsoluteUri(string AssemblyName, string path)
        {
            return new Uri($"pack://application:,,,/{AssemblyName};component/{path}");
        }

        internal static Uri GetAbsoluteUri(string Path)
        {
            return new Uri($"pack://application:,,,/HandyControl;component/{Path}");
        }

#if !NET40
        /// <summary>
        /// Faster application execution at startup by caching
        /// </summary>
        /// <param name="CachePath">Cache Path</param>
        public static void StartProfileOptimization(string CachePath = null)
        {
            if (string.IsNullOrEmpty(CachePath))
            {
                CachePath = $"{AppDomain.CurrentDomain.BaseDirectory}Cache";
            }
            if (!Directory.Exists(CachePath))
            {
                Directory.CreateDirectory(CachePath);
            }
            ProfileOptimization.SetProfileRoot(CachePath);
            ProfileOptimization.StartProfile("Profile");
        }
#endif
    }
}

