// https://github.com/meziantou/Meziantou.Framework

#if NETCOREAPP
namespace HandyControl.Tools
{
    internal static class PendingEvent
    {
        public static PendingEvent<T> Add<T>(T item) => new PendingEvent<T>(PendingEventType.Add, item);

        public static PendingEvent<T> Insert<T>(int index, T item) => new PendingEvent<T>(PendingEventType.Insert, item, index);

        public static PendingEvent<T> Remove<T>(T item) => new PendingEvent<T>(PendingEventType.Remove, item);

        public static PendingEvent<T> RemoveAt<T>(int index) => new PendingEvent<T>(PendingEventType.RemoveAt, index);

        public static PendingEvent<T> Replace<T>(int index, T item) => new PendingEvent<T>(PendingEventType.Replace, item, index);

        public static PendingEvent<T> Clear<T>() => new PendingEvent<T>(PendingEventType.Clear);
    }
}
#endif
