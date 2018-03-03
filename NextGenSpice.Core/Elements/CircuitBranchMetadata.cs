namespace NextGenSpice.Core.Elements
{
    /// <summary>
    ///     Type containing metadata about the connections inside the circuit.
    /// </summary>
    public struct CircuitBranchMetadata
    {
        /// <summary>
        ///     Source connection of the branch.
        /// </summary>
        public int N1 { get; set; }

        /// <summary>
        ///     Target connection of the branch.
        /// </summary>
        public int N2 { get; set; }

        /// <summary>
        ///     Type of this branch.
        /// </summary>
        public BranchType BranchType { get; set; }
    }
}