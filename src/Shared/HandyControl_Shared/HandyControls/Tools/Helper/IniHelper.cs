using System;
using System.Text;
using HandyControl.Tools.Interop;

namespace HandyControl.Tools;

public static class InIHelper
{
    internal static string Path = Environment.CurrentDirectory + @"\config.ini";

    private static string GetAssemblyName()
    {
        return System.IO.Path.GetFileNameWithoutExtension(ApplicationHelper.GetExecutablePathNative());
    }

    /// <summary>
    /// Read Data Value From the Ini File
    /// </summary>
    /// <param name="Key">must be unique</param>
    /// <param name="Section">Optional</param>
    /// <param name="Path">default is: application startup folder location</param>
    /// <returns></returns>
    public static string ReadValue(string Key, string Section = null, string Path = null)
    {
        var RetVal = new StringBuilder(255);
        InteropMethods.GetPrivateProfileString(Section ?? GetAssemblyName(), Key, "", RetVal, 255, Path ?? InIHelper.Path);
        return RetVal.ToString();
    }

    /// <summary>
    /// Write Data to the INI File
    /// </summary>
    /// <param name="Key"></param>
    /// <param name="Value"></param>
    /// <param name="Section">Optional</param>
    /// <param name="Path">default is: application startup folder location</param>
    public static void AddValue(string Key, string Value, string Section = null, string Path = null)
    {
        InteropMethods.WritePrivateProfileString(Section ?? GetAssemblyName(), Key, Value, Path ?? InIHelper.Path);
    }

    /// <summary>
    /// Delete Key from INI File
    /// </summary>
    /// <param name="Key"></param>
    /// <param name="Section">Optional</param>
    /// <param name="Path"></param>
    public static void DeleteKey(string Key, string Section = null, string Path = null)
    {
        AddValue(Key, null, Section ?? GetAssemblyName(), Path ?? InIHelper.Path);
    }

    /// <summary>
    /// Delete Section from INI File
    /// </summary>
    /// <param name="Section"></param>
    /// <param name="Path"></param>
    public static void DeleteSection(string Section = null, string Path = null)
    {
        AddValue(null, null, Section ?? GetAssemblyName(), Path ?? InIHelper.Path);
    }

    /// <summary>
    /// Check if Key Exist or Not in INI File
    /// </summary>
    /// <param name="Key"></param>
    /// <param name="Section">Optional</param>
    /// <param name="Path"></param>
    /// <returns></returns>
    public static bool IsKeyExists(string Key, string Section = null, string Path = null)
    {
        return ReadValue(Key, Section, Path ?? InIHelper.Path).Length > 0;
    }
}
