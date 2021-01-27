using System;

namespace HandyControl.Controls
{
    public static class PackUriHelper
    {
        public static Uri GetAbsoluteUri(string Namespace, string path)
        {
            return new Uri($"pack://application:,,,/{Namespace};component/{path}");
        }

        internal static Uri GetAbsoluteUri(string Path)
        {
            return new Uri($"pack://application:,,,/HandyControl;component/{Path}");
        }
    }
}
