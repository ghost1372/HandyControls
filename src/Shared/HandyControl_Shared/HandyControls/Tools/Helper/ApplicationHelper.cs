using System;
using System.Diagnostics;
using System.Globalization;
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

        /// <summary>
        /// Creates a Color from a XAML color string.
        /// </summary>
        /// <param name="colorString"></param>
        /// <returns></returns>
        public static Color GetColorFromString(string colorString)
        {
            if (string.IsNullOrEmpty(colorString))
            {
                throw new ArgumentException(nameof(colorString));
            }

            if (colorString[0] == '#')
            {
                switch (colorString.Length)
                {
                    case 9:
                        {
                            var cuint = Convert.ToUInt32(colorString.Substring(1), 16);
                            var a = (byte) (cuint >> 24);
                            var r = (byte) ((cuint >> 16) & 0xff);
                            var g = (byte) ((cuint >> 8) & 0xff);
                            var b = (byte) (cuint & 0xff);

                            return Color.FromArgb(a, r, g, b);
                        }

                    case 7:
                        {
                            var cuint = Convert.ToUInt32(colorString.Substring(1), 16);
                            var r = (byte) ((cuint >> 16) & 0xff);
                            var g = (byte) ((cuint >> 8) & 0xff);
                            var b = (byte) (cuint & 0xff);

                            return Color.FromArgb(255, r, g, b);
                        }

                    case 5:
                        {
                            var cuint = Convert.ToUInt16(colorString.Substring(1), 16);
                            var a = (byte) (cuint >> 12);
                            var r = (byte) ((cuint >> 8) & 0xf);
                            var g = (byte) ((cuint >> 4) & 0xf);
                            var b = (byte) (cuint & 0xf);
                            a = (byte) (a << 4 | a);
                            r = (byte) (r << 4 | r);
                            g = (byte) (g << 4 | g);
                            b = (byte) (b << 4 | b);

                            return Color.FromArgb(a, r, g, b);
                        }

                    case 4:
                        {
                            var cuint = Convert.ToUInt16(colorString.Substring(1), 16);
                            var r = (byte) ((cuint >> 8) & 0xf);
                            var g = (byte) ((cuint >> 4) & 0xf);
                            var b = (byte) (cuint & 0xf);
                            r = (byte) (r << 4 | r);
                            g = (byte) (g << 4 | g);
                            b = (byte) (b << 4 | b);

                            return Color.FromArgb(255, r, g, b);
                        }

                    default:
                        throw new FormatException(string.Format("The {0} string passed in the colorString argument is not a recognized Color format.", colorString));
                }
            }

            if (colorString.Length > 3 && colorString[0] == 's' && colorString[1] == 'c' && colorString[2] == '#')
            {
                var values = colorString.Split(',');

                if (values.Length == 4)
                {
                    var scA = double.Parse(values[0].Substring(3), CultureInfo.InvariantCulture);
                    var scR = double.Parse(values[1], CultureInfo.InvariantCulture);
                    var scG = double.Parse(values[2], CultureInfo.InvariantCulture);
                    var scB = double.Parse(values[3], CultureInfo.InvariantCulture);

                    return Color.FromArgb((byte) (scA * 255), (byte) (scR * 255), (byte) (scG * 255), (byte) (scB * 255));
                }

                if (values.Length == 3)
                {
                    var scR = double.Parse(values[0].Substring(3), CultureInfo.InvariantCulture);
                    var scG = double.Parse(values[1], CultureInfo.InvariantCulture);
                    var scB = double.Parse(values[2], CultureInfo.InvariantCulture);

                    return Color.FromArgb(255, (byte) (scR * 255), (byte) (scG * 255), (byte) (scB * 255));
                }

                throw new FormatException(string.Format("The {0} string passed in the colorString argument is not a recognized Color format (sc#[scA,]scR,scG,scB).", colorString));
            }

            var prop = typeof(Colors).GetTypeInfo().GetDeclaredProperty(colorString);

            if (prop != null)
            {
                return (Color) prop.GetValue(null);
            }

            throw new FormatException(string.Format("The {0} string passed in the colorString argument is not a recognized Color.", colorString));
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
            else if (brush.GetType() == typeof(SolidColorBrush))
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
        /// Get Hex Code from Color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string GetHexFromColor(Color color)
        {
            return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        /// <summary>
        /// Get Hex Code from Brush
        /// </summary>
        /// <param name="brush"></param>
        /// <returns></returns>
        public static string GetHexFromBrush(Brush brush)
        {
            var color = GetColorFromBrush(brush);
            return GetHexFromColor(color);
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

