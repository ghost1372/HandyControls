using Microsoft.Win32;
using System;
namespace HandyControl.Tools
{
    public class RegistryHelper
    {
        public static void AddOrUpdateKey<T>(string Key, string SubFolder, T value, HKEYType hkeyType = HKEYType.CurrentUser)
        {
            RegistryKey rgkey = RegistryType(hkeyType).CreateSubKey(SubFolder);
            rgkey?.SetValue(Key, value);
            rgkey?.Close();
        }

        public static T GetKey<T>(string Key, string SubFolder, HKEYType hkeyType = HKEYType.CurrentUser)
        {
            RegistryKey rgkey = RegistryType(hkeyType).OpenSubKey(SubFolder);
            try
            {
                if (rgkey != null)
                {
                    var result = (T)Convert.ChangeType(rgkey.GetValue(Key), typeof(T));
                    return result;
                }

            }
            finally
            {
                rgkey?.Close();
            }
            return default(T);
        }

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
                    ApplicationHelper.IsAdministrator();
                    break;
                case HKEYType.ClassesRoot:
                    key = Registry.ClassesRoot;
                    ApplicationHelper.IsAdministrator();
                    break;
            }
            return key;
        }
        
    }
}
