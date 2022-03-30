using System;

namespace HandyControl.Tools.Extension;

public static class ObjectExtension
{
    public static T CastTo<T>(this object value)
    {
        return typeof(T).IsValueType && value != null
            ? (T) Convert.ChangeType(value, typeof(T))
            : value is T typeValue ? typeValue : default;
    }
}
