﻿using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
namespace HandyControl.Controls
{
    public class InIHelper
    {
        internal static string AppName = Assembly.GetExecutingAssembly().GetName().Name;
        internal static string Pathx = Environment.CurrentDirectory + @"\config.ini";

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

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
            GetPrivateProfileString(Section ?? AppName, Key, "", RetVal, 255, Path ?? Pathx);
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
            WritePrivateProfileString(Section ?? AppName, Key, Value, Path ?? Pathx);
        }

        /// <summary>
        /// Delete Key from INI File
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Section">Optional</param>
        /// <param name="Path"></param>
        public static void DeleteKey(string Key, string Section = null, string Path = null)
        {
            AddValue(Key, null, Section ?? AppName, Path ?? Pathx);
        }

        /// <summary>
        /// Delete Section from INI File
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Path"></param>
        public static void DeleteSection(string Section = null, string Path = null)
        {
            AddValue(null, null, Section ?? AppName, Path ?? Pathx);
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
            return ReadValue(Key, Section, Path ?? Pathx).Length > 0;
        }
    }
}
