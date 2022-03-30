// https://github.com/meziantou/Meziantou.Framework

#if NETCOREAPP
using System.Runtime.InteropServices;

namespace HandyControl.Tools
{
    [StructLayout(LayoutKind.Auto)]
    internal readonly struct PendingEvent<T>
    {
        public PendingEvent(PendingEventType type)
        {
            Type = type;
            Item = default!;
            Index = -1;
        }

        public PendingEvent(PendingEventType type, int index)
        {
            Type = type;
            Item = default!;
            Index = index;
        }

        public PendingEvent(PendingEventType type, T item)
        {
            Type = type;
            Item = item;
            Index = -1;
        }

        public PendingEvent(PendingEventType type, T item, int index)
        {
            Type = type;
            Item = item;
            Index = index;
        }

        public PendingEventType Type { get; }
        public T Item { get; }
        public int Index { get; }
    }
}
#endif
