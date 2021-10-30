using System;
using HandyControl.Tools.Interop;

namespace HandyControl.Tools
{
    public static partial class WindowHelper
    {
        public static void EnableMicaEffect(IntPtr windowHandle, bool isDarkTheme)
        {
            InteropMethods.WindowExtendIntoClientArea(windowHandle, new InteropValues.MARGINS(-1, -1, -1, -1));

            var trueValue = 0x01;
            var falseValue = 0x00;

            // Set dark mode before applying the material, otherwise you'll get an ugly flash when displaying the window.
            if (isDarkTheme)
            {
                InteropMethods.SetWindowAttributeValue(windowHandle, InteropValues.DWMWINDOWATTRIBUTE.USE_IMMERSIVE_DARK_MODE, trueValue);
            }
            else
            {
                InteropMethods.SetWindowAttributeValue(windowHandle, InteropValues.DWMWINDOWATTRIBUTE.USE_IMMERSIVE_DARK_MODE, falseValue);
            }

            InteropMethods.SetWindowAttributeValue(windowHandle, InteropValues.DWMWINDOWATTRIBUTE.MICA_EFFECT, trueValue);
        }
    }
}
