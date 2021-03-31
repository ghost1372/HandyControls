// https://github.com/MartinKuschnik/Goji

using System.Diagnostics;

namespace HandyControl.Tools
{
    internal class Subscription<TEventHandler> : Subscription where TEventHandler : class
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly TEventHandler eventhandler;

        internal Subscription(TEventHandler eventhandler)
        {
            this.eventhandler = eventhandler;
        }

        public TEventHandler EventHandler
        {
            get
            {
                return this.eventhandler;
            }
        }
    }
}
