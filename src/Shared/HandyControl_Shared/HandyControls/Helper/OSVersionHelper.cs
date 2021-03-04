using System;
using System.Runtime.InteropServices;
using HandyControl.Tools.Interop;

namespace HandyControl.Tools
{
    public static class OSVersionHelper
    {
        private static readonly Version _osVersion = GetOSVersion();

                /// <summary>
        /// Windows NT
        /// </summary>
        public static bool IsWindowsNT { get; } = Environment.OSVersion.Platform == PlatformID.Win32NT;

        /// <summary>
        /// Windows 7
        /// </summary>
        public static bool IsWindows7 { get; } = IsWindowsNT && _osVersion == new Version(6, 1);

        /// <summary>
        /// Windows 7 Or Greater
        /// </summary>
        public static bool IsWindows7_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(6, 1);

        /// <summary>
        /// Windows 8
        /// </summary>
        public static bool IsWindows8 { get; } = IsWindowsNT && _osVersion == new Version(6, 2);

        /// <summary>
        /// Windows 8 Or Greater
        /// </summary>
        public static bool IsWindows8_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(6, 2);

        /// <summary>
        /// Windows 8.1
        /// </summary>
        public static bool IsWindows81 { get; } = IsWindowsNT && _osVersion == new Version(6, 3);

        /// <summary>
        /// Windows 8.1 Or Greater
        /// </summary>
        public static bool IsWindows81_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(6, 3);

        /// <summary>
        /// Windows 10
        /// </summary>
        public static bool IsWindows10 { get; } = IsWindowsNT && _osVersion == new Version(10, 0);

        /// <summary>
        /// Windows 10 Or Greater
        /// </summary>
        public static bool IsWindows10_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0);

        /// <summary>
        /// Windows 10 Threshold1 Version 1507 Build 10240
        /// </summary>
        public static bool IsWindows10_1507 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 10240);

        /// <summary>
        /// Windows 10 Threshold1 Version 1507 Build 10240 Or Greater
        /// </summary>
        public static bool IsWindows10_1507_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 10240);

        /// <summary>
        /// Windows 10 Threshold2 Version 1511 Build 10586 (November Update)
        /// </summary>
        public static bool IsWindows10_1511 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 10586);

        /// <summary>
        /// Windows 10 Threshold2 Version 1511 Build 10586 Or Greater (November Update)
        /// </summary>
        public static bool IsWindows10_1511_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 10586);

        /// <summary>
        /// Windows 10 Redstone1 Version 1607 Build 14393 (Anniversary Update)
        /// </summary>
        public static bool IsWindows10_1607 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 14393);

        /// <summary>
        /// Windows 10 Redstone1 Version 1607 Build 14393 Or Greater (Anniversary Update)
        /// </summary>
        public static bool IsWindows10_1607_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 14393);

        /// <summary>
        /// Windows 10 Redstone2 Version 1703 Build 15063 (Creators Update)
        /// </summary>
        public static bool IsWindows10_1703 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 15063);

        /// <summary>
        /// Windows 10 Redstone2 Version 1703 Build 15063 Or Greater (Creators Update)
        /// </summary>
        public static bool IsWindows10_1703_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 15063);

        /// <summary>
        /// Windows 10 Redstone3 Version 1709 Build 16299 (Fall Creators Update)
        /// </summary>
        public static bool IsWindows10_1709 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 16299);

        /// <summary>
        /// Windows 10 Redstone3 Version 1709 Build 16299 Or Greater (Fall Creators Update)
        /// </summary>
        public static bool IsWindows10_1709_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 16299);

        /// <summary>
        /// Windows 10 Redstone4 Version 1803 Build 17134 (April 2018 Update)
        /// </summary>
        public static bool IsWindows10_1803 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 17134);

        /// <summary>
        /// Windows 10 Redstone4 Version 1803 Build 17134 Or Greater (April 2018 Update)
        /// </summary>
        public static bool IsWindows10_1803_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 17134);

        /// <summary>
        /// Windows 10 Redstone5 Version 1809 Build 17763 (October 2018 Update)
        /// </summary>
        public static bool IsWindows10_1809 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 17763);

        /// <summary>
        /// Windows 10 Redstone5 Version 1809 Build 17763 Or Greater (October 2018 Update)
        /// </summary>
        public static bool IsWindows10_1809_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 17763);

        /// <summary>
        /// Windows 10 19H1 Version 1903 Build 18362 (May 2019 Update)
        /// </summary>
        public static bool IsWindows10_1903 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 18362);

        /// <summary>
        /// Windows 10 19H1 Version 1903 Build 18362 Or Greater (May 2019 Update)
        /// </summary>
        public static bool IsWindows10_1903_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 18362);

        /// <summary>
        /// Windows 10 19H2 Version 1909 Build 18363 (November 2019 Update)
        /// </summary>
        public static bool IsWindows10_1909 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 18363);

        /// <summary>
        /// Windows 10 19H2 Version 1909 Build 18363 Or Greater (November 2019 Update)
        /// </summary>
        public static bool IsWindows10_1909_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 18363);

        /// <summary>
        /// Windows 10 20H1 Version 2004 Build 19041 (May 2020 Update)
        /// </summary>
        public static bool IsWindows10_2004 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 19041);

        /// <summary>
        /// Windows 10 20H1 Version 2004 Build 19041 Or Greater (May 2020 Update)
        /// </summary>
        public static bool IsWindows10_2004_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 19041);

        /// <summary>
        /// Windows 10 20H2 Version 2009 Build 19042 (October 2020 Update)
        /// </summary>
        public static bool IsWindows10_2009 { get; } = IsWindowsNT && _osVersion == new Version(10, 0, 19042);

        /// <summary>
        /// Windows 10 20H2 Version 2009 Build 19042 Or Greater (October 2020 Update)
        /// </summary>
        public static bool IsWindows10_2009_OrGreater { get; } = IsWindowsNT && _osVersion >= new Version(10, 0, 19042);

        public static Version GetOSVersion()
        {
            var osv = new InteropValues.RTL_OSVERSIONINFOEX();
            osv.dwOSVersionInfoSize = (uint) Marshal.SizeOf(osv);
            InteropMethods.Gdip.RtlGetVersion(out osv);
            return new Version((int) osv.dwMajorVersion, (int) osv.dwMinorVersion, (int) osv.dwBuildNumber, (int) osv.dwRevision);
        }
    }
}
