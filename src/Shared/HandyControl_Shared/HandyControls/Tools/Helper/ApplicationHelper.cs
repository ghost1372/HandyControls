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
    public static class ApplicationHelper
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

        public static bool IsConnectedToInternet()
        {
            return InteropMethods.InternetGetConnectedState(out int Desc, 0);
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
        /// Register Context Menu in Windows Directory 
        /// </summary>
        /// <param name="ContextMenuName"></param>
        /// <param name="Command"></param>
        /// <param name="IconFilePath">Icon Should be in *.ico format</param>
        public static void RegisterToWindowsDirectoryContextMenu(string ContextMenuName, string Command, string IconFilePath = null)
        {
            string _DirectoryShell = $@"SOFTWARE\Classes\directory\shell\{ContextMenuName}\command\";
            string _Icon = $@"SOFTWARE\Classes\directory\shell\{ContextMenuName}\";
            RegistryHelper.AddOrUpdateKey("", _DirectoryShell, Command);
            if (!string.IsNullOrEmpty(IconFilePath))
            {
                RegistryHelper.AddOrUpdateKey("Icon", _Icon, IconFilePath);
            }
        }

        /// <summary>
        /// UnRegister Context Menu from Windows Directory
        /// </summary>
        /// <param name="ContextMenuName"></param>
        public static bool UnRegisterFromWindowsDirectoryContextMenu(string ContextMenuName)
        {
            string _RemovePath = $@"SOFTWARE\Classes\directory\shell\";
            return RegistryHelper.DeleteSubKeyTree(ContextMenuName, _RemovePath);
        }

        /// <summary>
        /// Register Context Menu in Windows File 
        /// </summary>
        /// <param name="ContextMenuName"></param>
        /// <param name="Command"></param>
        /// <param name="IconFilePath">Icon Should be in *.ico format</param>
        public static void RegisterToWindowsFileContextMenu(string ContextMenuName,string Command, string IconFilePath = null)
        {
            string _FileShell = $@"SOFTWARE\Classes\*\shell\{ContextMenuName}\command\";
            string _Icon = $@"SOFTWARE\Classes\*\shell\{ContextMenuName}\";

            RegistryHelper.AddOrUpdateKey("", _FileShell, Command);
            if (!string.IsNullOrEmpty(IconFilePath))
            {
                RegistryHelper.AddOrUpdateKey("Icon", _Icon, IconFilePath);
            }
        }

        /// <summary>
        /// UnRegister Context Menu from Windows File
        /// </summary>
        /// <param name="ContextMenuName"></param>
        public static bool UnRegisterFromWindowsFileContextMenu(string ContextMenuName)
        {
            var _RemovePath = @"SOFTWARE\Classes\*\shell\";
            return RegistryHelper.DeleteSubKeyTree(ContextMenuName, _RemovePath);
        }

        /// <summary>
        /// Register Context Menu in Windows Background 
        /// </summary>
        /// <param name="ContextMenuName"></param>
        /// <param name="Command"></param>
        /// <param name="IconFilePath">Icon Should be in *.ico format</param>
        public static void RegisterToWindowsBackgroundContextMenu(string ContextMenuName, string Command, string IconFilePath = null)
        {
            string _DirectoryShell = $@"SOFTWARE\Classes\directory\background\shell\{ContextMenuName}\command\";
            string _Icon = $@"SOFTWARE\Classes\directory\background\shell\{ContextMenuName}\";

            RegistryHelper.AddOrUpdateKey("", _DirectoryShell, Command);
            if (!string.IsNullOrEmpty(IconFilePath))
            {
                RegistryHelper.AddOrUpdateKey("Icon", _Icon, IconFilePath);
            }
        }

        /// <summary>
        /// UnRegister Context Menu from Windows Background
        /// </summary>
        /// <param name="ContextMenuName"></param>
        public static bool UnRegisterFromWindowsBackgroundContextMenu(string ContextMenuName)
        {
            string _RemovePath = $@"SOFTWARE\Classes\directory\background\shell\";
            return RegistryHelper.DeleteSubKeyTree(ContextMenuName, _RemovePath);
        }
    }
}

