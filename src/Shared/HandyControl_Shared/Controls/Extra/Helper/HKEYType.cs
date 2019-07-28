using System.ComponentModel;

namespace HandyControl.Controls
{
    public enum HKEYType
    {
        /// <summary>
        /// بدون نیاز دسترسی ادمین
        /// </summary>
        CurrentUser,

        /// <summary>
        /// برای استفاده نیاز به دسترسی ادمین دارد
        /// </summary>
        LocalMachine,

        /// <summary>
        /// برای استفاده نیاز به دسترسی ادمین دارد
        /// </summary>
        ClassesRoot
    }
}
