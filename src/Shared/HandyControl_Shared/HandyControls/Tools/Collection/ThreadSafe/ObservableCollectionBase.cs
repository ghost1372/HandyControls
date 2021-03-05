// https://github.com/meziantou/Meziantou.Framework

#if NETCOREAPP
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace HandyControl.Tools
{
    internal abstract class ObservableCollectionBase<T> : INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        private protected List<T> Items { get; }

        protected ObservableCollectionBase()
        {
            Items = new List<T>();
        }

        protected ObservableCollectionBase(IEnumerable<T> items)
        {
            if (items == null)
            {
                Items = new List<T>();
            }
            else
            {
                Items = new List<T>(items);
            }
        }

        protected void ReplaceItem(int index, T item)
        {
            var oldItem = Items[index];
            Items[index] = item;

            OnIndexerPropertyChanged();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem, index));
        }

        protected void InsertItem(int index, T item)
        {
            Items.Insert(index, item);

            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        protected void AddItem(T item)
        {
            var index = Items.Count;
            Items.Add(item);

            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        protected void RemoveItemAt(int index)
        {
            var item = Items[index];
            Items.RemoveAt(index);

            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        protected bool RemoveItem(T item)
        {
            var index = Items.IndexOf(item);
            if (index < 0)
                return false;

            Items.RemoveAt(index);

            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            return true;
        }

        protected void ClearItems()
        {
            Items.Clear();
            OnCountPropertyChanged();
            OnIndexerPropertyChanged();
            CollectionChanged?.Invoke(this, EventArgsCache.ResetCollectionChanged);
        }

        private void OnCountPropertyChanged() => OnPropertyChanged(EventArgsCache.CountPropertyChanged);
        private void OnIndexerPropertyChanged() => OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
        private void OnPropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);
    }
}
#endif
