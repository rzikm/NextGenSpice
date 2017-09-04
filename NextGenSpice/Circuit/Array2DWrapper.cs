namespace NextGenSpice.Circuit
{
    public struct Array2DWrapper
    {
        public Array2DWrapper(int size)
        {
            this.size = size;
            values = new double[size * size];
        }
        private readonly int size;
        private readonly double[] values;

        public int SizeLength => size;
        public double[] RawData => values;

        public double this[int i, int j]
        {
            get => values[i * size + j];
            set => values[i * size + j] = value;
        }

        public Array2DWrapper Clone()
        {
            var clone = new Array2DWrapper(size);
            for (int i = 0; i < RawData.Length; i++)
            {
                clone.RawData[i] = RawData[i];
            }

            return clone;
        }
    }
}