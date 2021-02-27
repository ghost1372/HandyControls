using System.Runtime.InteropServices;
using HandyControl.Tools.Interop;

namespace HandyControl.Controls
{
    public class InternetHelper
    {
        [DllImport(InteropValues.ExternDll.WinInet)]
        private static extern bool InternetGetConnectedState(out int Description, int ReservedValue);
        public static bool IsConnectedToInternet()
        {
            return InternetGetConnectedState(out int Desc, 0);
        }
    }
}
