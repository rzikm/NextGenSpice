namespace Numerics
{
    public class QdArray2DWrapper
    {
        public QdArray2DWrapper(int size)
        {
            this.size = size;
            values = new qd_real[size * size];
        }
        private readonly int size;
        private readonly qd_real[] values;

        public int SideLength => size;
        public qd_real[] RawData => values;

        public qd_real this[int row, int col]
        {
            get => values[row * size + col];
            set => values[row * size + col] = value;
        }

        public QdArray2DWrapper Clone()
        {
            var clone = new QdArray2DWrapper(size);
            RawData.CopyTo(clone.RawData, 0);

            return clone;
        }
    }
}