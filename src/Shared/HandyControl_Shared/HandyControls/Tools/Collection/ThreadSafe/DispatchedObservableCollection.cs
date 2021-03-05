// https://github.com/meziantou/Meziantou.Framework
#if NETCOREAPP
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows.Threading;
using HandyControl.Controls;

namespace HandyControl.Tools
{
    internal sealed class DispatchedObservableCollection<T> : ObservableCollectionBase<T>, IReadOnlyObservableCollection<T>, IList<T>, IList
    {
        private readonly ConcurrentQueue<PendingEvent<T>> _pendingEvents = new ConcurrentQueue<PendingEvent<T>>();
        private readonly ThreadSafeObservableCollection<T> _collection;
        private readonly Dispatcher _dispatcher;

        private bool _isDispatcherPending;

        public DispatchedObservableCollection(ThreadSafeObservableCollection<T> collection, Dispatcher dispatcher)
            : base(collection)
        {
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        private void AssertIsOnDispatcherThread()
        {
            if (!IsOnDispatcherThread())
            {
                throw new InvalidOperationException("The collection must be accessed from the dispatcher thread only. Current thread ID: " + Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture));
            }
        }

        private static void AssertType(object? value, string argumentName)
        {
            if (value is null || value is T)
                return;

            throw new ArgumentException($"value must be of type '{typeof(T).FullName}'", argumentName);
        }

        public int Count
        {
            get
            {
                AssertIsOnDispatcherThread();
                return Items.Count;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                AssertIsOnDispatcherThread();
                return ((ICollection<T>)_collection).IsReadOnly;
            }
        }

        int ICollection.Count
        {
            get
            {
                AssertIsOnDispatcherThread();
                return Count;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                AssertIsOnDispatcherThread();
                return ((ICollection)Items).SyncRoot;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                AssertIsOnDispatcherThread();
                return ((ICollection)Items).IsSynchronized;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                AssertIsOnDispatcherThread();
                return ((IList)Items).IsReadOnly;
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                AssertIsOnDispatcherThread();
                return ((IList)Items).IsFixedSize;
            }
        }

        object? IList.this[int index]
        {
            get
            {
                AssertIsOnDispatcherThread();
                return this[index];
            }

            set
            {
                // it will immediatly modify both collections as we are on the dispatcher thread
                AssertType(value, nameof(value));
                AssertIsOnDispatcherThread();
                _collection[index] = (T)value!;
            }
        }

        T IList<T>.this[int index]
        {
            get
            {
                AssertIsOnDispatcherThread();
                return this[index];
            }
            set
            {
                // it will immediatly modify both collections as we are on the dispatcher thread
                AssertIsOnDispatcherThread();
                _collection[index] = value;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            AssertIsOnDispatcherThread();
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void CopyTo(T[] array, int arrayIndex)
        {
            AssertIsOnDispatcherThread();
            Items.CopyTo(array, arrayIndex);
        }

        public int IndexOf(T item)
        {
            AssertIsOnDispatcherThread();
            return Items.IndexOf(item);
        }

        public bool Contains(T item)
        {
            AssertIsOnDispatcherThread();
            return Items.Contains(item);
        }

        public T this[int index]
        {
            get
            {
                AssertIsOnDispatcherThread();
                return Items[index];
            }
        }

        internal void EnqueueReplace(int index, T value)
        {
            EnqueueEvent(PendingEvent.Replace(index, value));
        }

        internal void EnqueueAdd(T item)
        {
            EnqueueEvent(PendingEvent.Add(item));
        }

        internal bool EnqueueRemove(T item)
        {
            EnqueueEvent(PendingEvent.Remove(item));
            return true;
        }

        internal void EnqueueRemoveAt(int index)
        {
            EnqueueEvent(PendingEvent.RemoveAt<T>(index));
        }

        internal void EnqueueClear()
        {
            EnqueueEvent(PendingEvent.Clear<T>());
        }

        internal void EnqueueInsert(int index, T item)
        {
            EnqueueEvent(PendingEvent.Insert(index, item));
        }

        private void EnqueueEvent(PendingEvent<T> @event)
        {
            _pendingEvents.Enqueue(@event);
            ProcessPendingEventsOrDispatch();
        }

        private void ProcessPendingEventsOrDispatch()
        {
            if (!IsOnDispatcherThread())
            {
                if (!_isDispatcherPending)
                {
                    _isDispatcherPending = true;
                    _dispatcher.BeginInvoke((Action)ProcessPendingEvents);
                }

                return;
            }

            ProcessPendingEvents();
        }

        private void ProcessPendingEvents()
        {
            _isDispatcherPending = false;
            while (_pendingEvents.TryDequeue(out var pendingEvent))
            {
                switch (pendingEvent.Type)
                {
                    case PendingEventType.Add:
                        AddItem(pendingEvent.Item);
                        break;

                    case PendingEventType.Remove:
                        RemoveItem(pendingEvent.Item);
                        break;

                    case PendingEventType.Clear:
                        ClearItems();
                        break;

                    case PendingEventType.Insert:
                        InsertItem(pendingEvent.Index, pendingEvent.Item);
                        break;

                    case PendingEventType.RemoveAt:
                        RemoveItemAt(pendingEvent.Index);
                        break;

                    case PendingEventType.Replace:
                        ReplaceItem(pendingEvent.Index, pendingEvent.Item);
                        break;
                }
            }
        }

        private bool IsOnDispatcherThread()
        {
            return _dispatcher.Thread == Thread.CurrentThread;
        }

        void IList<T>.Insert(int index, T item)
        {
            // it will immediatly modify both collections as we are on the dispatcher thread
            AssertIsOnDispatcherThread();
            _collection.Insert(index, item);
        }

        void IList<T>.RemoveAt(int index)
        {
            // it will immediatly modify both collections as we are on the dispatcher thread
            AssertIsOnDispatcherThread();
            _collection.RemoveAt(index);
        }

        void ICollection<T>.Add(T item)
        {
            // it will immediatly modify both collections as we are on the dispatcher thread
            AssertIsOnDispatcherThread();
            _collection.Add(item);
        }

        void ICollection<T>.Clear()
        {
            // it will immediatly modify both collections as we are on the dispatcher thread
            AssertIsOnDispatcherThread();
            _collection.Clear();
        }

        bool ICollection<T>.Remove(T item)
        {
            // it will immediatly modify both collections as we are on the dispatcher thread
            AssertIsOnDispatcherThread();
            return _collection.Remove(item);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)Items).CopyTo(array, index);
        }

        int IList.Add(object? value)
        {
            // it will immediatly modify both collections as we are on the dispatcher thread
            AssertType(value, nameof(value));
            AssertIsOnDispatcherThread();
            return ((IList)_collection).Add(value);
        }

        bool IList.Contains(object? value)
        {
            // it will immediatly modify both collections as we are on the dispatcher thread
            AssertType(value, nameof(value));
            AssertIsOnDispatcherThread();
            return ((IList)_collection).Contains(value);
        }

        void IList.Clear()
        {
            // it will immediatly modify both collections as we are on the dispatcher thread
            AssertIsOnDispatcherThread();
            ((IList)_collection).Clear();
        }

        int IList.IndexOf(object? value)
        {
            // it will immediatly modify both collections as we are on the dispatcher thread
            AssertType(value, nameof(value));
            AssertIsOnDispatcherThread();
            return Items.IndexOf((T)value!);
        }

        void IList.Insert(int index, object? value)
        {
            // it will immediatly modify both collections as we are on the dispatcher thread
            AssertType(value, nameof(value));
            AssertIsOnDispatcherThread();
            ((IList)_collection).Insert(index, value);
        }

        void IList.Remove(object? value)
        {
            // it will immediatly modify both collections as we are on the dispatcher thread
            AssertType(value, nameof(value));
            AssertIsOnDispatcherThread();
            ((IList)_collection).Remove(value);
        }

        void IList.RemoveAt(int index)
        {
            // it will immediatly modify both collections as we are on the dispatcher thread
            AssertIsOnDispatcherThread();
            ((IList)_collection).RemoveAt(index);
        }
    }
}
#endif
