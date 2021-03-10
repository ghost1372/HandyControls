namespace HandyControl.Tools.Extension
{
    public struct EnumeratorWithIndex<T>
    {
        public readonly T Value;
        public readonly int Index;

        public EnumeratorWithIndex(T value, int index)
        {
            this.Value = value;
            this.Index = index;
        }

        public static EnumeratorWithIndex<T> Create(T value, int index)
        {
            return new EnumeratorWithIndex<T>(value, index);
        }
    }
}
