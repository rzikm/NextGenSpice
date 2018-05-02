using System.Collections.Generic;
using NextGenSpice.Core.BehaviorParams;

namespace NextGenSpice.Core.Devices
{
    /// <summary>Class that represents a current source device.</summary>
    public class CurrentSource : TwoTerminalCircuitDevice
    {
        public CurrentSource(SourceBehaviorParams behavior, object tag = null) : base(tag)
        {
            BehaviorParams = behavior;
        }

        public CurrentSource(double current, object tag = null) : this(
            new ConstantBehaviorParams {Value = current}, tag)
        {
        }

        /// <summary>Behavior parameters of the input source.</summary>
        public SourceBehaviorParams BehaviorParams { get; set; }

        /// <summary>Creates a deep copy of this device.</summary>
        /// <returns></returns>
        public override ICircuitDefinitionDevice Clone()
        {
            var clone = (CurrentSource) base.Clone();
            clone.BehaviorParams = (SourceBehaviorParams) clone.BehaviorParams.Clone();
            return clone;
        }

        /// <summary>Gets metadata about this device interconnections in the circuit.</summary>
        /// <returns></returns>
        public override IEnumerable<CircuitBranchMetadata> GetBranchMetadata()
        {
            return new[]
            {
                new CircuitBranchMetadata(Anode, Cathode, BranchType.CurrentDefined, this)
            };
        }
    }
}