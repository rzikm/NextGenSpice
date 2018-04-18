﻿using System.Collections.Generic;
using NextGenSpice.Core.BehaviorParams;

namespace NextGenSpice.Core.Devices
{
    /// <summary>Class that represents a current source device.</summary>
    public class CurrentSourceDevice : TwoTerminalCircuitDevice
    {
        public CurrentSourceDevice(SourceBehaviorParams behavior, string name = null) : base(name)
        {
            BehaviorParams = behavior;
        }

        public CurrentSourceDevice(double current, string name = null) : this(
            new ConstantBehaviorParams {Value = current}, name)
        {
        }

        /// <summary>Behavior parameters of the input source.</summary>
        public SourceBehaviorParams BehaviorParams { get; set; }

        /// <summary>Creates a deep copy of this device.</summary>
        /// <returns></returns>
        public override ICircuitDefinitionDevice Clone()
        {
            var clone = (CurrentSourceDevice) base.Clone();
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