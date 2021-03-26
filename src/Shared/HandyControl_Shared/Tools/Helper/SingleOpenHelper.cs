using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using HandyControl.Controls;

namespace HandyControl.Tools
{
    /// <summary>
    ///     This class can provide single-open functions for visual elements
    /// </summary>
    public class SingleOpenHelper
    {
        private static readonly Dictionary<string, ISingleOpen> OpenDic = new Dictionary<string, ISingleOpen>();

        /// <summary>
        ///     Create an instance based on the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateControl<T>() where T : Visual, ISingleOpen, new()
        {
            var typeStr = typeof(T).FullName;

            if (string.IsNullOrEmpty(typeStr)) return default;

            var temp = new T();
            if (!OpenDic.Keys.Contains(typeStr))
            {
                OpenDic.Add(typeStr, temp);
                return temp;
            }
            var currentCtl = OpenDic[typeStr];
            if (currentCtl.CanDispose)
            {
                currentCtl.Dispose();
                OpenDic[typeStr] = temp;
                return temp;
            }
            return default;
        }
    }
}
