﻿using System.Collections.Generic;
using NextGenSpice.Core.BehaviorParams;

namespace NextGenSpice.Core.Elements
{
    /// <summary>Class that represents a current source device.</summary>
    public class VoltageSourceElement : TwoNodeCircuitElement
    {
        public VoltageSourceElement(SourceBehaviorParams behavior, string name = null) : base(name)
        {
            BehaviorParams = behavior;
        }

        public VoltageSourceElement(double voltage, string name = null) : this(
            new ConstantBehaviorParams {Value = voltage}, name)
        {
        }

        /// <summary>Behavior parameters of the input source.</summary>
        public SourceBehaviorParams BehaviorParams { get; set; }

        /// <summary>Creates a deep copy of this device.</summary>
        /// <returns></returns>
        public override ICircuitDefinitionElement Clone()
        {
            var clone = (VoltageSourceElement) base.Clone();
            clone.BehaviorParams = (SourceBehaviorParams) clone.BehaviorParams.Clone();
            return clone;
        }

        /// <summary>Gets metadata about this device interconnections in the circuit.</summary>
        /// <returns></returns>
        public override IEnumerable<CircuitBranchMetadata> GetBranchMetadata()
        {
            return new[]
            {
                new CircuitBranchMetadata(Anode, Cathode, BranchType.VoltageDefined, this)
            };
        }
    }
}