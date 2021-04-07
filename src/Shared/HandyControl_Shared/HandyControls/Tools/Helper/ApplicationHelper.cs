using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Security.Principal;
using System.Threading;
using HandyControl.Tools.Interop;

namespace HandyControl.Tools
{
    public static partial class ApplicationHelper
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

        /// <summary>
        /// Bring MainWindow To Front
        /// </summary>
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
        /// <returns></returns>
        public static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public static bool IsConnectedToInternet()
        {
            return InteropMethods.InternetGetConnectedState(out int Desc, 0);
        }

        /// <summary>
        /// Get AbsoluteUri like pack://application:,,,/WpfApp;component/Style.xaml
        /// </summary>
        /// <param name="AssemblyName">Project Name</param>
        /// <param name="path">xaml file path</param>
        /// <returns>pack://application:,,,/{AssemblyName};component/{path}</returns>
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

