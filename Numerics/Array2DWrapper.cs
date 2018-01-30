namespace Numerics
{
    public class Array2DWrapper<T>
    {
        public Array2DWrapper(int size)
        {
            this.size = size;
            values = new T[size * size];
        }
        private readonly int size;
        private readonly T[] values;

        public int SideLength => size;
        public T[] RawData => values;

        public T this[int row, int col]
        {
            get => values[row * size + col];
            set => values[row * size + col] = value;
        }

        public Array2DWrapper<T> Clone()
        {
            var clone = new Array2DWrapper<T>(size);
            RawData.CopyTo(clone.RawData, 0);


            return clone;
        }
    }
}