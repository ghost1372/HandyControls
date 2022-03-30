using Microsoft.Win32;
using System;

namespace HandyControl.Tools;

public static class RegistryHelper
{
    /// <summary>
    /// Add Or Update Key
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
            using (RegistryKey key = Location.CreateSubKey(Folder))
            {
                key?.SetValue(Key, Value);
                key?.Close();
            }
        }
        catch (UnauthorizedAccessException)
        {
            throw new UnauthorizedAccessException();
        }
    }

    /// <summary>
    /// Get Value from Key
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
            using (RegistryKey key = Location.OpenSubKey(Folder))
            {
                if (key == null)
                {
                    return default(T);
                }
                else
                {
                    var result = (T) Convert.ChangeType(key.GetValue(Key), typeof(T));
                    return result;
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            throw new UnauthorizedAccessException();
        }
    }

    /// <summary>
    /// Delete Key
    /// </summary>
    /// <param name="Key"></param>
    /// <param name="Folder"></param>
    /// <param name="Location"></param>
    /// <param name="IsDeleteSubKey"></param>
    /// <returns></returns>
    public static bool DeleteKey(string Key, string Folder, RegistryKey Location = null, bool IsDeleteSubKey = false)
    {
        try
        {
            if (Location == null)
            {
                Location = Registry.CurrentUser;
            }

            if (Location == Registry.LocalMachine)
            {
                Folder = $@"SOFTWARE\{Folder}";
            }

            using (RegistryKey key = Location.OpenSubKey(Folder, true))
            {
                if (key == null)
                {
                    return false;
                }
                else
                {
                    if (IsDeleteSubKey)
                    {
                        Location.DeleteSubKey(Folder, true);
                        return true;

                    }
                    else
                    {
                        key.DeleteValue(Key);
                        return true;
                    }
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            throw new UnauthorizedAccessException();
        }
        catch (ArgumentException) { }

        return false;
    }

    /// <summary>
    /// Delete a Key and any childs recursively
    /// </summary>
    /// <param name="SubKey"></param>
    /// <param name="Folder"></param>
    /// <param name="Location"></param>
    /// <returns></returns>
    public static bool DeleteSubKeyTree(string SubKey, string Folder, RegistryKey Location = null)
    {
        try
        {
            if (Location == null)
            {
                Location = Registry.CurrentUser;
            }

            if (Location == Registry.LocalMachine)
            {
                Folder = $@"SOFTWARE\{Folder}";
            }

            using (RegistryKey key = Location.OpenSubKey(Folder, true))
            {
                if (key == null)
                {
                    return false;
                }
                else
                {
                    key.DeleteSubKeyTree(SubKey, true);
                    return true;
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            throw new UnauthorizedAccessException();
        }
        catch (ArgumentException) { }

        return false;
    }
}
