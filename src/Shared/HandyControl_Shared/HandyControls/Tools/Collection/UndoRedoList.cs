#if !(NET40 || NET45 || NET451 || NET452 || NET46 || NET461 || NET462)
// https://github.com/Fujiwo/Shos.UndoRedoList

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HandyControl.Tools;
using HandyControl.Tools.Extension;

namespace HandyControl.Controls
{
    /// <summary>IList implemented collection which supports undo/redo.</summary>
    /// <typeparam name="TElement">type of elements</typeparam>
    /// <typeparam name="TList">type of IList implemented collection</typeparam>
    public class UndoRedoList<TElement, TList> : IList<TElement> where TList : IList<TElement>, new()
    {
        abstract class Action
        {
            public IList<TElement> Container { get; set; } = null;
            public TElement Element { get; set; }
            public int Index { get; set; } = 0;

            bool IsValidIndex => 0 <= Index && Index < Container.Count;

            public Action()
            { }
            public Action(IList<TElement> container, TElement element, int index) => (Container, Element, Index) = (container, element, index);

            public abstract void Undo();
            public abstract void Redo();

            protected void Add()
            {
                if (IsValidIndex)
                {
                    Container.Insert(Index, Element);
                }
                else
                {
                    Index = Container.Count;
                    Container.Add(Element);
                }
            }

            protected void Remove()
            {
                if (IsValidIndex)
                    Container.RemoveAt(Index);
            }

            protected void Exchange()
            {
                if (IsValidIndex)
                    Container[Index] = Element;
            }

            protected void Exchange(TElement element)
            {
                if (IsValidIndex)
                    Container[Index] = element;
            }
        }

        class AddAction : Action
        {
            public AddAction(IList<TElement> container, TElement element, int index) : base(container, element, index)
            { }

            public override void Undo() => Remove();
            public override void Redo() => Add();
        }

        class RemoveAction : Action
        {
            public RemoveAction(IList<TElement> container, TElement element, int index) : base(container, element, index)
            { }

            public override void Undo() => Add();
            public override void Redo() => Remove();
        }

        class ExchangeAction : Action
        {
            public ExchangeAction(IList<TElement> container, TElement oldElement, TElement newElement, int index) : base(container, newElement, index) => OldElement = oldElement;

            public TElement OldElement { get; set; }

            public override void Undo() => Exchange(OldElement);
            public override void Redo() => Exchange();
        }

        class ActionCollection : Action, IEnumerable<Action>
        {
            List<Action> actions;

            public ActionCollection() => actions = new List<Action>();
            public ActionCollection(IEnumerable<Action> actions) => this.actions = actions.ToList();

            public void Add(Action action) => actions.Add(action);

            public override void Undo() => actions.ReverseForEach(action => action.Undo());
            public override void Redo() => actions.ForEach(action => action.Redo());

            public IEnumerator<Action> GetEnumerator() => actions.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        /// <summary>Actions in ActionScope can undo in one time.</summary>
        public class ActionScope : IDisposable
        {
            readonly UndoRedoList<TElement, TList> list;

            public ActionScope(UndoRedoList<TElement, TList> list)
            {
                this.list = list;
                list.BeginActions();
            }

            public void Dispose() => list.EndActions();
        }

        /// <summary>You can't undo actions in DisabledUndoScope.</summary>
        public class DisabledUndoScope : IDisposable
        {
            readonly UndoRedoList<TElement, TList> list;
            readonly bool listUndoEnabled;

            public DisabledUndoScope(UndoRedoList<TElement, TList> list)
            {
                this.list = list;
                listUndoEnabled = list.UndoEnabled;
                list.UndoEnabled = false;
            }

            public void Dispose() => list.UndoEnabled = listUndoEnabled;
        }

        readonly UndoRedoRingBuffer<Action> undoBuffer;
        List<Action> storedActions = new List<Action>();
        bool hasBeganStoringAction = false;

        public bool CanUndo => undoBuffer.CanGoBackward;
        public bool CanRedo => undoBuffer.CanGoForward;
        /// <summary>You can't undo actions while UndoEnabled is false.</summary>
        public bool UndoEnabled { get; set; } = true;

        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when maximumUndoTimes is 1 or less.</exception>
        public UndoRedoList(int maximumUndoTimes = ModuloArithmetic.DefaultDivisor)
            => undoBuffer = new UndoRedoRingBuffer<Action>(maximumUndoTimes);

        public bool Undo()
        {
            if (CanUndo)
            {
                var action = undoBuffer.Current;
                action.Undo();
                undoBuffer.GoBackward();
                return true;
            }
            return false;
        }

        public bool Redo()
        {
            if (CanRedo)
            {
                undoBuffer.GoForward();
                var action = undoBuffer.Current;
                action.Redo();
                return true;
            }
            return false;
        }

        public void ClearUndo() => undoBuffer.Clear();

        /// <summary>
        /// Method for ActionScope.
        /// You can undo actions between BeginActions() and EndActions() in one time.
        /// </summary>
        public void BeginActions()
        {
            if (hasBeganStoringAction || storedActions.Count != 0)
                throw new InvalidOperationException();
            hasBeganStoringAction = true;
        }

        /// <summary>
        /// Method for ActionScope.
        /// You can undo actions between BeginActions() and EndActions() in one time.
        /// </summary>
        public void EndActions()
        {
            if (!hasBeganStoringAction)
                throw new InvalidOperationException();
            if (storedActions.Count == 1)
                undoBuffer.Add(storedActions[0]);
            else if (storedActions.Count > 1)
                undoBuffer.Add(new ActionCollection(storedActions));
            storedActions.Clear();
            hasBeganStoringAction = false;
        }

        /// <summary>Inner IList implemented collection.</summary>
        public TList List { get; } = new TList();

        #region IList<T> implementation
        public int Count => List.Count;

        public bool IsReadOnly => List.IsReadOnly;

        public TElement this[int index]
        {
            get => List[index];
            set
            {
                Add(new ExchangeAction(container: List, oldElement: List[index], newElement: value, index: index));
                List[index] = value;
            }
        }

        public void Add(TElement element)
        {
            Add(new AddAction(container: List, element: element, index: List.Count));
            List.Add(element);
        }

        public void Clear()
        {
            var actionCollection = new ActionCollection { Container = List };
            for (var index = List.Count - 1; index >= 0; index--)
                actionCollection.Add(new RemoveAction(container: List, element: List[index], index: index));
            Add(actionCollection);
            List.Clear();
        }

        public bool Contains(TElement element) => List.Contains(element);

        public void CopyTo(TElement[] array, int arrayIndex) => List.CopyTo(array, arrayIndex);

        public int IndexOf(TElement element) => List.IndexOf(element);

        public void Insert(int index, TElement element)
        {
            Add(new AddAction(container: List, element: element, index: index));
            List.Insert(index, element);
        }

        public bool Remove(TElement element)
        {
            var index = List.IndexOf(element);
            if (index < 0)
                return false;
            RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            Add(new RemoveAction(container: List, element: List[index], index: index));
            List.RemoveAt(index);
        }

        public IEnumerator<TElement> GetEnumerator() => List.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        void Add(Action action)
        {
            if (!UndoEnabled)
                return;

            if (hasBeganStoringAction)
                storedActions.Add(action);
            else
                undoBuffer.Add(action);
        }
    }

    /// <summary>List which supports undo/redo.</summary>
    /// <typeparam name="TElement">type of elements</typeparam>
    public class UndoRedoList<TElement> : UndoRedoList<TElement, List<TElement>>
    {
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when maximumUndoTimes is 1 or less.</exception>
        public UndoRedoList(int maximumUndoTimes = ModuloArithmetic.DefaultDivisor) : base(maximumUndoTimes)
        { }
    }
}
#endif
