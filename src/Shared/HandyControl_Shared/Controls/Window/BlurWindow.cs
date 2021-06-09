using System.Runtime.InteropServices;
using System.Windows;
using HandyControl.Data;
using HandyControl.Tools;
using HandyControl.Tools.Interop;

namespace HandyControl.Controls
{
    public class BlurWindow : Window
    {
        internal static BlurWindow Instance;

        public static readonly DependencyProperty FORCE_ENABLE_ACRYLIC_BLURProperty = DependencyProperty.Register(
                "FORCE_ENABLE_ACRYLIC_BLUR", typeof(bool), typeof(BlurWindow),
                new PropertyMetadata(ValueBoxes.FalseBox));

        public bool FORCE_ENABLE_ACRYLIC_BLUR
        {
            get => (bool) GetValue(FORCE_ENABLE_ACRYLIC_BLURProperty);
            set => SetValue(FORCE_ENABLE_ACRYLIC_BLURProperty, ValueBoxes.BooleanBox(value));
        }

        public BlurWindow()
        {
            Instance = this;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            EnableBlur(this);
        }

        internal static void EnableBlur(Window window)
        {
            var version = OSVersionHelper.GetOSVersion();
            var versionInfo = new SystemVersionInfo(version.Major, version.Minor, version.Build);

            var accentPolicy = new InteropValues.ACCENTPOLICY();
            var accentPolicySize = Marshal.SizeOf(accentPolicy);

            accentPolicy.AccentFlags = 2;

            if (versionInfo >= SystemVersionInfo.Windows10_1903)
            {
                accentPolicy.AccentState = Instance.FORCE_ENABLE_ACRYLIC_BLUR
                    ? InteropValues.ACCENTSTATE.ACCENT_ENABLE_ACRYLICBLURBEHIND
                    : InteropValues.ACCENTSTATE.ACCENT_ENABLE_BLURBEHIND;
            }
            else if (versionInfo >= SystemVersionInfo.Windows10_1809)
            {
                accentPolicy.AccentState = InteropValues.ACCENTSTATE.ACCENT_ENABLE_ACRYLICBLURBEHIND;
            }
            else if (versionInfo >= SystemVersionInfo.Windows10)
            {
                accentPolicy.AccentState = InteropValues.ACCENTSTATE.ACCENT_ENABLE_BLURBEHIND;
            }
            else
            {
                accentPolicy.AccentState = InteropValues.ACCENTSTATE.ACCENT_ENABLE_TRANSPARENTGRADIENT;
            }

            accentPolicy.GradientColor = ResourceHelper.GetResource<uint>(ResourceToken.BlurGradientValue);

            var accentPtr = Marshal.AllocHGlobal(accentPolicySize);
            Marshal.StructureToPtr(accentPolicy, accentPtr, false);

            var data = new InteropValues.WINCOMPATTRDATA
            {
                Attribute = InteropValues.WINDOWCOMPOSITIONATTRIB.WCA_ACCENT_POLICY,
                DataSize = accentPolicySize,
                Data = accentPtr
            };

            InteropMethods.Gdip.SetWindowCompositionAttribute(window.GetHandle(), ref data);

            Marshal.FreeHGlobal(accentPtr);
        }
    }
}
