// https://github.com/DingpingZhang/WpfExtensions

using System;

namespace HandyControl.Tools
{
    internal static class ObjectExtensions
    {
        internal static T CastTo<T>(this object value)
        {
            return typeof(T).IsValueType && value != null
                ? (T) Convert.ChangeType(value, typeof(T))
                : value is T typeValue ? typeValue : default;
        }
    }
}
