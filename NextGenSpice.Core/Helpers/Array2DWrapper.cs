namespace NextGenSpice.Core.Helpers
{
    public class Array2DWrapper
    {
        public Array2DWrapper(int size)
        {
            this.size = size;
            values = new double[size * size];
        }
        private readonly int size;
        private readonly double[] values;

        public int SideLength => size;
        public double[] RawData => values;

        public double this[int i, int j]
        {
            get => values[i * size + j];
            set => values[i * size + j] = value;
        }

        public Array2DWrapper Clone()
        {
            var clone = new Array2DWrapper(size);
            RawData.CopyTo(clone.RawData, 0);

            return clone;
        }
    }
}