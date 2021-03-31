// https://github.com/MartinKuschnik/Goji

using System;
using System.Diagnostics;
using System.Globalization;

namespace HandyControl.Tools
{
    public class CultureChangedEventArgs : EventArgs
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly CultureInfo oldValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly CultureInfo newValue;

        public CultureChangedEventArgs(CultureInfo oldValue, CultureInfo newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public CultureInfo OldValue
        {
            get { return this.oldValue; }
        }

        public CultureInfo NewValue
        {
            get { return this.newValue; }
        }
    }
}
