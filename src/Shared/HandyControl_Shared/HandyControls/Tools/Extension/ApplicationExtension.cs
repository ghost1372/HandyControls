using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
namespace HandyControl.Tools.Extension
{
    public static class ApplicationExtension
    {
        /// <summary>
        /// Get the path of the executable file of the current application, including the name of the executable file.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The path of the executable file of the current application, including the name of the executable file</returns>
        public static string ExecutablePath(this Application value)
        {
            Uri uri = new Uri(Assembly.GetEntryAssembly().Location);
            if (uri.IsFile)
                return uri.LocalPath + Uri.UnescapeDataString(uri.Fragment);
            else
                return uri.ToString();
        }

        /// <summary>
        /// Close the application and start a new instance immediately
        /// </summary>
        /// <param name="value"></param>
        public static void Restart(this Application value)
        {
            string cmdLine = Environment.CommandLine;
            string cmdLineArgs0 = Environment.GetCommandLineArgs()[0];
            int i = cmdLine.IndexOf(' ', cmdLine.IndexOf(cmdLineArgs0) + cmdLineArgs0.Length);
            cmdLine = cmdLine.Remove(0, i + 1);

            ProcessStartInfo startInfo = Process.GetCurrentProcess().StartInfo;
            startInfo.FileName = value.ExecutablePath();
            startInfo.Arguments = cmdLine;
            value.Shutdown();
            Process.Start(startInfo);
        }
    }
}
