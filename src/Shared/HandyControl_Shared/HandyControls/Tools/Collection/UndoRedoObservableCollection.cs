#if !(NET40 || NET45 || NET451 || NET452 || NET46 || NET461 || NET462)

// https://github.com/Fujiwo/Shos.UndoRedoList

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using HandyControl.Tools;

namespace HandyControl.Controls
{
    /// <summary>ObservableCollection which supports undo/redo.</summary>
    /// <typeparam name="TElement">type of elements</typeparam>
    public class UndoRedoObservableCollection<TElement> : UndoRedoList<TElement, ObservableCollection<TElement>>, INotifyCollectionChanged
    {
#region INotifyCollectionChanged implementation
        public event NotifyCollectionChangedEventHandler CollectionChanged;
#endregion

        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when maximumUndoTimes is 1 or less.</exception>
        public UndoRedoObservableCollection(int maximumUndoTimes = ModuloArithmetic.DefaultDivisor) : base(maximumUndoTimes) => List.CollectionChanged += (_, e) => CollectionChanged?.Invoke(this, e);
    }
}
#endif
