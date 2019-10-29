using HandyControl.Data;
using System;
using System.Runtime.CompilerServices;

namespace HandyControl.Controls
{
    internal static class DoubleUtil
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNaN(this double value) => double.IsNaN(value);

        public const double Epsilon = 2.2204460492503131E-16;

        public static object GetBoxed(double value)
        {
            if (IsCloseToZero(value))
                return ValueBoxes.Double0Box;

            if (IsCloseToOne(value))
                return ValueBoxes.Double1Box;

            if (value.IsNaN())
                return double.NaN;

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCloseToZero(double value) => Math.Abs(value) < Epsilon;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCloseToOne(double value) => Math.Abs(value - 1.0) < Epsilon;
        
    }
}
