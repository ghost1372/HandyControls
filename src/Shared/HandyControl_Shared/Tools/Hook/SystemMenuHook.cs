using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using HandyControl.Tools.Interop;

namespace HandyControl.Tools
{
    public class SystemMenuHook
    {
        private static ContextMenu context = null;
        private static readonly Dictionary<int, HwndSource> DataDic = new Dictionary<int, HwndSource>();

        public static event Action<int> Click;

        public static void SetCustomContextMenu(Window window, ContextMenu contextMenu)
        {
            context = contextMenu;
            var hookId = window.GetHandle();
            var source = HwndSource.FromHwnd(hookId);
            if (source != null)
            {
                source.AddHook(new HwndSourceHook(WndProc));
            }
        }

        public static void Insert(int index, int id, string text, Window window)
        {
            var hookId = window.GetHandle();
            var source = HwndSource.FromHwnd(hookId);
            if (source != null)
            {
                DataDic[id] = source;
                InteropMethods.InsertMenu(InteropMethods.GetSystemMenu(hookId, false), index, InteropValues.MF_BYPOSITION, id, text);
                source.AddHook(WinProc);
            }
        }

        public static void InsertSeperator(int index, Window window) => InteropMethods.InsertMenu(
            InteropMethods.GetSystemMenu(window.GetHandle(), false), index,
            InteropValues.MF_BYPOSITION | InteropValues.MF_SEPARATOR, 0, "");

        public static void Remove(int id)
        {
            if (DataDic.TryGetValue(id, out var data))
            {
                data.RemoveHook(WinProc);
                DataDic.Remove(id);
            }
        }

        private static IntPtr WinProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            if (msg == InteropValues.WM_SYSCOMMAND)
            {
                var id = wparam.ToInt32();
                if (DataDic.ContainsKey(id))
                {
                    Click?.Invoke(id);
                }
            }
            return IntPtr.Zero;
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if ((msg == InteropValues.WM_NCRBUTTONDOWN) && (wParam.ToInt32() == InteropValues.HTCAPTION))
            {
                context.IsOpen = true;
                handled = true;
            }
            return IntPtr.Zero;
        }
    }
}
