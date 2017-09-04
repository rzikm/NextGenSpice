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
//
//    public class NodeConnectionSet : ICollection<CircuitNode>
//    {
//        private readonly CircuitNode[] set;
//
//        public NodeConnectionSet(int count)
//        {
//            set = new CircuitNode[count];
//        }
//
//        public NodeConnectionSet(CircuitNode[] values)
//        {
//            set = values.ToArray();
//        }
//
//        public CircuitNode this[int index]
//        {
//            get => set[index];
//            internal set => set[index] = value;
//        }
//
//        public IEnumerator<CircuitNode> GetEnumerator()
//        {
//            return (IEnumerator<CircuitNode>) set.GetEnumerator();
//        }
//
//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return GetEnumerator();
//        }
//
//        public void Add(CircuitNode item)
//        {
//            throw new System.InvalidOperationException("Collection is readonly");
//        }
//
//        public void Clear()
//        {
//            throw new System.InvalidOperationException("Collection is readonly");
//        }
//
//        public bool Contains(CircuitNode item)
//        {
//            return set.Contains(item);
//        }
//
//        public void CopyTo(CircuitNode[] array, int arrayIndex)
//        {
//            set.CopyTo(array,arrayIndex);
//        }
//
//        public bool Remove(CircuitNode item)
//        {
//            throw new System.InvalidOperationException("Collection is readonly");
//        }
//
//        public int Count => set.Length;
//        public bool IsReadOnly { get; } = true;
//    }
}