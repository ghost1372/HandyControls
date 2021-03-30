using System;
using System.Diagnostics;

namespace HandyControl.Tools
{
    internal class Subscription : IDisposable
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool isDisposed = false;

        public event EventHandler Disposed;

        public void Dispose()
        {
            if (!this.isDisposed)
            {
                this.isDisposed = true;

                EventHandler disposedEvent = this.Disposed;

                if (disposedEvent != null)
                {
                    disposedEvent(this, EventArgs.Empty);
                }
            }
        }
    }
}
