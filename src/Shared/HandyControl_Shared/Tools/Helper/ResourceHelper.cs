using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace HandyControl.Tools
{
    /// <summary>
    ///     Resource help class
    /// </summary>
    public class ResourceHelper
    {
        /// <summary>
        ///     Get string
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetString(string key) => Application.Current.TryFindResource(key) as string;

        /// <summary>
        ///     Get string
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="keyArr"></param>
        /// <returns></returns>
        public static string GetString(string separator = ";", params string[] keyArr) =>
            string.Join(separator, keyArr.Select(key => Application.Current.TryFindResource(key) as string).ToList());

        /// <summary>
        ///     Get string
        /// </summary>
        /// <param name="keyArr"></param>
        /// <returns></returns>
        public static List<string> GetStringList(params string[] keyArr) => keyArr.Select(key => Application.Current.TryFindResource(key) as string).ToList();

        /// <summary>
        ///     Access to resources
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetResource<T>(string key)
        {
            if (Application.Current.TryFindResource(key) is T resource)
            {
                return resource;
            }

            return default;
        }
    }
}
