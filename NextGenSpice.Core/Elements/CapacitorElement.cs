﻿using System.Collections.Generic;

namespace NextGenSpice.Core.Elements
{
    /// <summary>
    ///     Class that represents a capacitor device.
    /// </summary>
    public class CapacitorElement : TwoNodeCircuitElement
    {
        public CapacitorElement(double capacity, double? initialVoltage = null, string name = null) : base(name)
        {
            Capacity = capacity;
            InitialVoltage = initialVoltage;
        }

        /// <summary>
        ///     Capacity in farads.
        /// </summary>
        public double Capacity { get; set; }

        /// <summary>
        ///     Initial voltage across the capacitor in volts.
        /// </summary>
        public double? InitialVoltage { get; set; }

        /// <summary>
        ///     Gets metadata about this device interconnections in the circuit.
        /// </summary>
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