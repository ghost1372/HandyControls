using System;
using System.Reflection;

namespace HandyControl.Tools
{
    public class AssemblyHelper
    {
        public static string GetCallingAssemblyName()
        {
            return Assembly.GetCallingAssembly().GetName().Name;
        }

        public static Version GetCallingAssemblyVersion()
        {
            return Assembly.GetCallingAssembly().GetName().Version;
        }

        public static string GetExecutingAssemblyName()
        {
            return Assembly.GetExecutingAssembly().GetName().Name;
        }

        public static Version GetExecutingAssemblyVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }
       
        public static string GetEntryAssemblyName()
        {
            return Assembly.GetEntryAssembly().GetName().Name;
        }

        public static Version GetEntryAssemblyVersion()
        {
            return Assembly.GetEntryAssembly().GetName().Version;
        }
    }
}
