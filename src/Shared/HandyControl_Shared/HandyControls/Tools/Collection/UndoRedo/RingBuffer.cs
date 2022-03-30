// https://github.com/Fujiwo/Shos.UndoRedoList

using System.Collections;
using System.Collections.Generic;

namespace HandyControl.Tools
{
    internal class RingBuffer<TElement> : IEnumerable<TElement>
    {
        protected const int defaultSize = ModuloArithmetic.DefaultDivisor;

        readonly TElement[] elements;

        public int Count => HasData ? BottomIndex - TopIndex + 1 : 0;
        public ModuloArithmetic TopIndex { get; private set; }
        public ModuloArithmetic BottomIndex { get; private set; }

        bool HasData => BottomIndex.IsValid;

        public TElement this[ModuloArithmetic index]
        {
            get => elements[index.Value];
            set => elements[index.Value] = value;
        }

        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when size is 1 or less.</exception>
        public RingBuffer(int size = defaultSize)
        {
            elements = new TElement[size];
            TopIndex = new ModuloArithmetic(size);
            BottomIndex = new ModuloArithmetic(size, false);
        }

        public virtual void Add(TElement element)
        {
            if (BottomIndex.IsValid)
            {
                BottomIndex++;
                if (BottomIndex == TopIndex)
                    TopIndex++;
            }
            else
            {
                BottomIndex = TopIndex;
            }
            elements[BottomIndex.Value] = element;
        }

        public virtual void Clear() => BottomIndex = BottomIndex.InvalidItem;

        /// <summary>Remove all elements at the index and after the index.</summary>
        public void RemoveAfter(ModuloArithmetic index)
        {
            if (!HasData)
                return;

            if (index == TopIndex)
                Clear();
            else
                BottomIndex = index.Previous;
        }

        public bool Remove()
        {
            if (!HasData)
                return false;
            if (Count == 1)
                BottomIndex = BottomIndex.InvalidItem;
            else
                BottomIndex--;
            return true;
        }

        #region IEnumerable<T> implementation
        public IEnumerator<TElement> GetEnumerator()
        {
            if (HasData)
            {
                for (var index = TopIndex; ;)
                {
                    yield return this[index];
                    index++;
                    if (index == BottomIndex.Next)
                        break;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }
}
