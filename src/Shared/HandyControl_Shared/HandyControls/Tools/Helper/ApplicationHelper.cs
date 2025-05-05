using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Security.Principal;
using System.Threading;
using HandyControl.Tools.Interop;

namespace HandyControl.Tools;

public static partial class ApplicationHelper
{
    private static readonly int MAX_PATH = 255;
    private static Mutex mutex;

    /// <summary>
    /// Check that only one instance of the program runs
    /// </summary>
    /// <param name="AssemblyName"></param>
    /// <returns></returns>
    public static bool IsSingleInstance(string AssemblyName = null)
    {
        if (string.IsNullOrEmpty(AssemblyName))
        {
            AssemblyName = Path.GetFileNameWithoutExtension(GetExecutablePathNative());
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

    /// <summary>
    /// Close the application and start a new instance immediately
    /// </summary>
    /// <param name="restartAsAdministrator"></param>
    public static void Restart(bool restartAsAdministrator = false)
    {
        ProcessStartInfo processStartInfo = new ProcessStartInfo(GetExecutablePathNative());

        if (restartAsAdministrator)
        {
            processStartInfo.UseShellExecute = true;
            processStartInfo.Verb = "runas";
        }

        Process.Start(processStartInfo);
        Environment.Exit(0);
    }

    /// <summary>
    /// Assembly Based Method to Get the path of the executable file, this method work in .Net Framework and .Net Core in Portable and Publish Mode
    /// </summary>
    /// <returns></returns>
    public static string GetExecutablePath()
    {
#if NET5_0
        return GetExecutablePathNative();
#else
        var path = Assembly.GetEntryAssembly().Location;
        if (Path.GetExtension(path).Equals(".dll"))
        {
            path = path.Replace(".dll", ".exe");
        }
        return path;
#endif
    }

    /// <summary>
    /// Native Method to Get the path of the executable file, this method work in .Net Framework and .Net Core in Portable and Publish Mode
    /// </summary>
    /// <returns></returns>
    public static string GetExecutablePathNative()
    {
        var sb = new System.Text.StringBuilder(MAX_PATH);
        InteropMethods.GetModuleFileName(IntPtr.Zero, sb, MAX_PATH);
        return sb.ToString();
    }

    /// <summary>
    /// Retrieves an enumeration value from a string representation. It requires the generic type to be an enum.
    /// </summary>
    /// <typeparam name="TEnum">The generic type must be an enumeration type to convert the string into its corresponding enum value.</typeparam>
    /// <param name="text">The string representation of the enumeration value to be converted.</param>
    /// <returns>The corresponding enumeration value of the specified type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the generic type parameter is not an enumeration.</exception>
    public static TEnum GetEnum<TEnum>(string text) where TEnum : struct
    {
        return !typeof(TEnum).GetTypeInfo().IsEnum
            ? throw new InvalidOperationException("Generic parameter 'TEnum' must be an enum.")
            : (TEnum) Enum.Parse(typeof(TEnum), text);
    }
}

