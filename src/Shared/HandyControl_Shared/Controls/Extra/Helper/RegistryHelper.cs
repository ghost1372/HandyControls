using Microsoft.Win32;
using System;
using System.Security.Principal;

namespace HandyControl.Controls
{
    public class RegistryHelper
    {
        /// <summary>
        /// افزودن کلید به رجیستری
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key">شناسه کلید</param>
        /// <param name="SubFolder">پوشه موردنظر</param>
        /// <param name="hkeyType">دسته بندی کلید</param>
        /// <param name="value">مقدار (میتواند در هر نوعی باشد، نیازی به تبدیل نیست)</param>
        public static void AddOrUpdateKey<T>(string Key, string SubFolder, T value, HKEYType hkeyType = HKEYType.CurrentUser)
        {
            RegistryKey rgkey = RegistryType(hkeyType).CreateSubKey(SubFolder);

            //storing the values  
            rgkey.SetValue(Key, value);
            rgkey.Close();
        }

        /// <summary>
        /// دریافت مقدار کلید
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key">شناسه کلید</param>
        /// <param name="SubFolder">پوشه موردنظر</param>
        /// <param name="hkeyType">دسته بندی کلید</param>
        /// <returns>bool, int, string...</returns>
        public static T GetKey<T>(string Key, string SubFolder, HKEYType hkeyType = HKEYType.CurrentUser)
        {
            RegistryKey rgkey = RegistryType(hkeyType).OpenSubKey(SubFolder);

            T result;
            try
            {
                if (rgkey != null)
                {
                    result = (T)Convert.ChangeType(rgkey.GetValue(Key), typeof(T));

                    return result;
                }

            }
            finally
            {
                if(rgkey != null)
                    rgkey.Close();
            }
            return default(T);
        }

        /// <summary>
        /// حذف کلید از رجیستری
        /// </summary>
        /// <param name="Key">شناسه کلید</param>
        /// <param name="SubFolder">پوشه موردنظر</param>
        /// <param name="hkeyType">دسته بندی کلید</param>
        /// <param name="IsDeleteSubFolder">در صورت نیاز به حذف کل پوشه موردنظر این مقدار را True کنید</param>
        public static void DeleteKey(string Key, string SubFolder, bool IsDeleteSubFolder = false, HKEYType hkeyType = HKEYType.CurrentUser)
        {
            RegistryKey rgkey = RegistryType(hkeyType).OpenSubKey(SubFolder, true);

            if (rgkey != null)
            {
                if (IsDeleteSubFolder)
                {
                    RegistryType(hkeyType).DeleteSubKey(SubFolder);
                }
                else
                {
                    if (rgkey.GetValue(Key) != null)
                    {
                        rgkey.DeleteValue(Key);
                        rgkey.Close();
                    }
                }
            }
        }

        internal static RegistryKey RegistryType(HKEYType type)
        {
            RegistryKey key = null;
            switch (type)
            {
                case HKEYType.CurrentUser:
                    key = Registry.CurrentUser;
                    break;
                case HKEYType.LocalMachine:
                    key = Registry.LocalMachine;
                    IsAdministrator();
                    break;
                case HKEYType.ClassesRoot:
                    key = Registry.ClassesRoot;
                    IsAdministrator();
                    break;
            }
            return key;
        }
        internal static void IsAdministrator()
        {
            if (!(new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                      .IsInRole(WindowsBuiltInRole.Administrator))
                throw new Exception("You need Administrator access, please run app as Administrator");
        }
    }
}
