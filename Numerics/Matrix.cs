using System;

namespace Numerics
{
    /// <summary>
    ///     Class that represents square matrix of given size and represents it internally as one-dimensional array.
    /// </summary>
    /// <typeparam name="T">Element type</typeparam>
    public class Matrix<T> : ICloneable
    {
        public Matrix(int size)
        {
            Size = size;
            RawData = new T[size * size];
        }

        /// <summary>
        ///     Number of rows or columns in the matrix.
        /// </summary>
        public int Size { get; }

        /// <summary>
        ///     Inner one-dimensional representation of the matrix, element [i,j] is on index i * size + j.
        /// </summary>
        public T[] RawData { get; }

        public T this[int row, int col]
        {
            get => RawData[row * Size + col];
            set => RawData[row * Size + col] = value;
        }

        /// <summary>
        ///     Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        ///     Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns></returns>
        public Matrix<T> Clone()
        {
            var clone = new Matrix<T>(Size);
            RawData.CopyTo(clone.RawData, 0);


            return clone;
        }
    }
}