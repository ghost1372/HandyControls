// https://github.com/meziantou/Meziantou.Framework

#if NETCOREAPP
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace HandyControl.Tools
{
    public interface IReadOnlyObservableCollection<T> : IReadOnlyList<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
    }
}
#endif
