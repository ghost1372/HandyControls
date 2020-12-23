using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using HandyControl.Tools.Interop;

namespace HandyControl.Controls
{
    internal static class OSVersionHelper
    {
        private static readonly Version _osVersion = GetOSVersion();

        internal static bool IsWindowsNT { get; } = Environment.OSVersion.Platform == PlatformID.Win32NT;

        internal static bool IsWindows8OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(6, 2);

        internal static bool IsWindows10OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0);

        public static Version GetOSVersion()
        {
            var osv = new InteropValues.RTL_OSVERSIONINFOEX();
            osv.dwOSVersionInfoSize = (uint) Marshal.SizeOf(osv);
            InteropMethods.Gdip.RtlGetVersion(out osv);
            return new Version((int) osv.dwMajorVersion, (int) osv.dwMinorVersion, (int) osv.dwBuildNumber);
        }
    }
}
