using System.Collections;
using System.Collections.Generic;

namespace NextGenSpice.Core.Circuit
{
    /// <summary>
    ///     Class for representing terminal connection of a circuit device.
    /// </summary>
    public class NodeConnectionSet : IReadOnlyList<int>
    {
        private readonly int[] set;

        public NodeConnectionSet(int count)
        {
            set = new int[count];
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the connected nodes.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<int> GetEnumerator()
        {
            return ((IEnumerable<int>) set).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Number of terminals of the device.
        /// </summary>
        public int Count => set.Length;

        public int this[int index]
        {
            get => set[index];
            internal set => set[index] = value;
        }

        /// <summary>
        ///     Creates a deep copy of the connection set.
        /// </summary>
        /// <returns></returns>
        public NodeConnectionSet Clone()
        {
            var clone = new NodeConnectionSet(set.Length);
            set.CopyTo(clone.set, 0);

            return clone;
        }

        /// <summary>
        ///     Returns zero-based index of the terminal that is connected to node of given id. Returns a negative number if not
        ///     found.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public int IndexOf(int i)
        {
            for (var j = 0; j < set.Length; j++)
                if (set[j] == i)
                    return j;

            return -1;
        }
    }
}