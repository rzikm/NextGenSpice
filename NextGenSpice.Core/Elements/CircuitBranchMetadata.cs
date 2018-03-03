namespace NextGenSpice.Core.Elements
{
    /// <summary>
    ///     Type containing metadata about the connections inside the circuit.
    /// </summary>
    public struct CircuitBranchMetadata
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object"></see> class.</summary>
        public CircuitBranchMetadata(int n1, int n2, BranchType branchType, ICircuitDefinitionElement element)
        {
            N1 = n1;
            N2 = n2;
            BranchType = branchType;
            Element = element;
        }

        /// <summary>
        ///     Source connection of the branch.
        /// </summary>
        public int N1 { get;  }

        /// <summary>
        ///     Target connection of the branch.
        /// </summary>
        public int N2 { get;  }

        /// <summary>
        ///     Type of this branch.
        /// </summary>
        public BranchType BranchType { get;  }

        /// <summary>
        ///     Source element of the branch.
        /// </summary>
        public ICircuitDefinitionElement Element { get; }
    }
}