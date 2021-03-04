#if !NET40
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
