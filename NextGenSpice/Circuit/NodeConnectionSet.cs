using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NextGenSpice.Circuit
{
    public class NodeConnectionSet : IReadOnlyList<CircuitNode>
    {
        private readonly CircuitNode[] set;

        public NodeConnectionSet(int count)
        {
            set = new CircuitNode[count];
        }

        public IEnumerator<CircuitNode> GetEnumerator()
        {
            return (IEnumerator<CircuitNode>) set.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => set.Length;

        public CircuitNode this[int index]
        {
            get => set[index];
            set => set[index] = value;
        }
    }
}