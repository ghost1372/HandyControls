using System;
using System.Collections.Generic;
namespace HandyControl.Tools
{
    public class GenericCompare<T> : IEqualityComparer<T> where T : class
    {
        private Func<T, object> _expr { get; set; }
        public GenericCompare(Func<T, object> expr)
        {
            this._expr = expr;
        }
        public bool Equals(T x, T y)
        {
            var first = _expr.Invoke(x);
            var sec = _expr.Invoke(y);
            return first != null && first.Equals(sec);
        }
        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}
