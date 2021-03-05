using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Windows.Media;
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
        /// <returns></returns>
        public static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
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
        /// <summary>
        /// Get Color from LinearGradientBrush, SolidColorBrush and Brush
        /// </summary>
        /// <param name="brush"></param>
        /// <returns></returns>
        public static Color GetColorFromBrush(Brush brush)
        {
            if (brush.GetType() == typeof(LinearGradientBrush))
            {
                var linearBrush = (LinearGradientBrush) brush;
                return new SolidColorBrush(linearBrush.GradientStops[1].Color).Color;
            }
            else if(brush.GetType() == typeof(SolidColorBrush))
            {
                var solidBrush = (SolidColorBrush) brush;
                return Color.FromArgb(solidBrush.Color.A, solidBrush.Color.R, solidBrush.Color.G, solidBrush.Color.B);
            }
            else
            {
                return ((SolidColorBrush) brush).Color;
            }
        }

        /// <summary>
        /// Register Context Menu in Windows Directory Shell 
        /// </summary>
        /// <param name="ContextMenuName"></param>
        /// <param name="Command"></param>
        public static void RegisterToWindowsDirectoryShellContextMenu(string ContextMenuName, string Command)
        {
            string _DirectoryShell = $@"SOFTWARE\Classes\directory\shell\{ContextMenuName}\command\";
            RegistryHelper.AddOrUpdateKey("", _DirectoryShell, Command);
        }

        /// <summary>
        /// UnRegister Context Menu from Windows Directory Shell
        /// </summary>
        /// <param name="ContextMenuName"></param>
        public static void UnRegisterFromWindowsDirectoryShellContextMenu(string ContextMenuName)
        {
            string _RemovePath = $@"SOFTWARE\Classes\directory\shell\";
            RegistryHelper.DeleteSubKeyTree(ContextMenuName, _RemovePath);
        }

        /// <summary>
        /// Register Context Menu in Windows File Shell 
        /// </summary>
        /// <param name="ContextMenuName"></param>
        /// <param name="Command"></param>
        public static void RegisterToWindowsFileShellContextMenu(string ContextMenuName,string Command)
        {
            string _FileShell = $@"SOFTWARE\Classes\*\shell\{ContextMenuName}\command\";
            RegistryHelper.AddOrUpdateKey("", _FileShell, Command);
        }

        /// <summary>
        /// UnRegister Context Menu from Windows File Shell
        /// </summary>
        /// <param name="ContextMenuName"></param>
        public static void UnRegisterFromWindowsFileShellContextMenu(string ContextMenuName)
        {
            var _RemovePath = @"SOFTWARE\Classes\*\shell\";
            RegistryHelper.DeleteSubKeyTree(ContextMenuName, _RemovePath);
        }

    }
}

