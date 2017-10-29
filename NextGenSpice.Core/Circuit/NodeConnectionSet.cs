using System.Collections;
using System.Collections.Generic;

namespace NextGenSpice.Core.Circuit
{
    public class NodeConnectionSet : IReadOnlyList<int>
    {
        private readonly int[] set;

        public NodeConnectionSet(int count)
        {
            set = new int[count];
        }

        public IEnumerator<int> GetEnumerator()
        {
            return ((IEnumerable<int>) set).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => set.Length;

        public int this[int index]
        {
            get => set[index];
            internal set => set[index] = value;
        }
    }
}