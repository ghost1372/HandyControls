using System;
using System.Runtime.InteropServices;
using HandyControl.Tools.Interop;
using System.Windows.Interop;

namespace HandyControl.Tools;

public static partial class ApplicationHelper
{
    /// <summary>
    /// Send Message to Another Application
    /// </summary>
    /// <param name="MainWindowTitle"></param>
    /// <param name="Message"></param>
    public static void SendMessageToAnotherProcess(string MainWindowTitle, string Message)
    {
        IntPtr handle;
        InteropMethods.FindWindowTitleMatch(MainWindowTitle, out handle, out MainWindowTitle);
        if (handle != IntPtr.Zero)
        {
            string text = Message;
            byte[] sarr = System.Text.Encoding.Default.GetBytes(text);
            int len = sarr.Length;
            InteropValues.COPYDATASTRUCT cds;
            cds.dwData = (IntPtr) 100;
            cds.lpData = text;
            cds.cbData = len + 1;
            InteropMethods.SendMessage(handle, InteropValues.WM_COPYDATA, 0, ref cds);
        }
    }

    /// <summary>
    /// Listen to Receive Message from Another Process
    /// </summary>
    /// <param name="window"></param>
    /// <param name="action"></param>
    public static void ListenToReceiveMessageFromAnotherProcess(System.Windows.Window window, Action<string> action)
    {
        HwndSource hWndSource;
        WindowInteropHelper wih = new WindowInteropHelper(window);
        hWndSource = HwndSource.FromHwnd(wih.Handle);
        HwndSourceHook eventHandler = (IntPtr hwnd, int msg, IntPtr param, IntPtr lParam, ref bool handled) =>
        {
            switch (msg)
            {
                case InteropValues.WM_COPYDATA:
                    {
                        InteropValues.COPYDATASTRUCT dataStruct = (InteropValues.COPYDATASTRUCT) Marshal.PtrToStructure(lParam, typeof(InteropValues.COPYDATASTRUCT));
                        action.Invoke(dataStruct.lpData);
                        break;
                    }
            }
            return IntPtr.Zero;
        };
        hWndSource.AddHook(eventHandler);
    }
}
