using System.Collections.Generic;
using System.Linq;
namespace HandyControl.Tools.Extension
{
    public static class EnumeratorWithIndex
    {
        /// <summary>
        /// This Extension Help you to access item index in foreach loop
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static IEnumerable<ValueWithIndex<T>> GetEnumeratorWithIndex<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Select(ValueWithIndex<T>.Create);
        }
    }
    public struct ValueWithIndex<T>
    {
        public readonly T Value;
        public readonly int Index;

        public ValueWithIndex(T value, int index)
        {
            this.Value = value;
            this.Index = index;
        }

        public static ValueWithIndex<T> Create(T value, int index)
        {
            return new ValueWithIndex<T>(value, index);
        }
    }
}
