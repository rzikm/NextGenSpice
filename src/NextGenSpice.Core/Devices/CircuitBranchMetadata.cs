namespace NextGenSpice.Core.Devices
{
	/// <summary>Type containing metadata about the connections inside the circuit.</summary>
	public struct CircuitBranchMetadata
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Object"></see> class.</summary>
		public CircuitBranchMetadata(int n1, int n2, BranchType branchType, ICircuitDefinitionDevice device)
		{
			N1 = n1;
			N2 = n2;
			BranchType = branchType;
			Device = device;
		}

		/// <summary>Source connection of the branch.</summary>
		public int N1 { get; }

		/// <summary>Target connection of the branch.</summary>
		public int N2 { get; }

		/// <summary>Type of this branch.</summary>
		public BranchType BranchType { get; }

		/// <summary>Source device of the branch.</summary>
		public ICircuitDefinitionDevice Device { get; }
	}
}