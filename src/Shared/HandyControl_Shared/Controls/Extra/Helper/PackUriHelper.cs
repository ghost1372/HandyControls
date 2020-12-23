using System;

namespace HandyControl.Controls
{
    internal static class PackUriHelper
    {
        public static Uri GetAbsoluteUri(string Namespace, string path)
        {
            return new Uri($"pack://application:,,,/{Namespace};component/{path}");
        }
    }
}
