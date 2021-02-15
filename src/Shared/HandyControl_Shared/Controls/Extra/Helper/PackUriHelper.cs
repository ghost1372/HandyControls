using System;

namespace HandyControl.Controls
{
    public static class PackUriHelper
    {
        public static Uri GetAbsoluteUri(string AssemblyName, string path)
        {
            return new Uri($"pack://application:,,,/{AssemblyName};component/{path}");
        }

        internal static Uri GetAbsoluteUri(string Path)
        {
            return new Uri($"pack://application:,,,/HandyControl;component/{Path}");
        }
    }
}
