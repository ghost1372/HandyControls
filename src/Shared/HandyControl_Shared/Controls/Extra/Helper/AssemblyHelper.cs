using System.Reflection;

namespace HandyControl.Controls
{
    public class AssemblyHelper
    {
        public static Assembly GetCallingAssembly()
        {
            return Assembly.GetCallingAssembly();
        }
        public static string GetCallingAssemblyName()
        {
            return GetCallingAssembly().GetName().Name;
        }
        public static Assembly GetExecutingAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }

        public static string GetExecutingAssemblyName()
        {
            return GetExecutingAssembly().GetName().Name;
        }
        public static Assembly GetEntryAssembly()
        {
            return Assembly.GetEntryAssembly();
        }

        public static string GetEntryAssemblyName()
        {
            return GetEntryAssembly().GetName().Name;
        }
    }
}
