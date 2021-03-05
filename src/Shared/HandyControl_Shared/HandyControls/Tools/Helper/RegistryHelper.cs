using Microsoft.Win32;
using System;
namespace HandyControl.Tools
{
    public class RegistryHelper
    {
        /// <summary>
        /// Add Or Update Value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="Folder"></param>
        /// <param name="Value"></param>
        /// <param name="Location">null mean CurrentUser</param>
        public static void AddOrUpdateKey<T>(string Key, string Folder, T Value, RegistryKey Location = null)
        {
            if (Location == null)
            {
                Location = Registry.CurrentUser;
            }

            if (Location == Registry.LocalMachine)
            {
                Folder = $@"SOFTWARE\{Folder}";
            }

            try
            {
                RegistryKey rgkey = Location.CreateSubKey(Folder);
                rgkey?.SetValue(Key, Value);
                rgkey?.Close();
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException();
            }
        }

        /// <summary>
        /// Get Value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="Folder"></param>
        /// <param name="Location"></param>
        /// <returns></returns>
        public static T GetValue<T>(string Key, string Folder, RegistryKey Location = null)
        {
            if (Location == null)
            {
                Location = Registry.CurrentUser;
            }

            if (Location == Registry.LocalMachine)
            {
                Folder = $@"SOFTWARE\{Folder}";
            }

            try
            {
                RegistryKey rgkey = Location.OpenSubKey(Folder);
                if (rgkey != null)
                {
                    var result = (T)Convert.ChangeType(rgkey.GetValue(Key), typeof(T));
                    return result;
                }

            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException();
            }
            return default(T);
        }

        /// <summary>
        /// Delete Value
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Folder"></param>
        /// <param name="Location"></param>
        /// <param name="IsDeleteSubKey"></param>
        public static void DeleteKey(string Key, string Folder, RegistryKey Location = null, bool IsDeleteSubKey = false)
        {
            if (Location == null)
            {
                Location = Registry.CurrentUser;
            }

            if (Location == Registry.LocalMachine)
            {
                Folder = $@"SOFTWARE\{Folder}";
            }

            try
            {
                RegistryKey rgkey = Location.OpenSubKey(Folder, true);

                if (rgkey != null)
                {
                    if (IsDeleteSubKey)
                    {
                        Location.DeleteSubKey(Folder);
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
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException();
            }
        }

        public static void DeleteSubKeyTree(string SubKey, string Folder, RegistryKey Location = null)
        {
            if (Location == null)
            {
                Location = Registry.CurrentUser;
            }

            if (Location == Registry.LocalMachine)
            {
                Folder = $@"SOFTWARE\{Folder}";
            }

            try
            {
                RegistryKey rgkey = Location.OpenSubKey(Folder, true);

                if (rgkey != null)
                {
                    rgkey.DeleteSubKeyTree(SubKey);
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException();
            }
        }

    }
}
