﻿using System.Collections.Generic;

namespace NextGenSpice.Core.Elements
{
    /// <summary>
    ///     Class that represents an inductor device.
    /// </summary>
    public class InductorElement : TwoNodeCircuitElement
    {
        public InductorElement(double inductance, double? initialCurrent = null, string name = null) : base(name)
        {
            Inductance = inductance;
            InitialCurrent = initialCurrent;
        }

        /// <summary>
        ///     Inductance of the device in henry.
        /// </summary>
        public double Inductance { get; set; }

        /// <summary>
        ///     Initial current flowing from the positive node to the negative node through the inductor in volts.
        /// </summary>
        public double? InitialCurrent { get; set; }

        /// <summary>
        ///     Gets metadata about this device interconnections in the circuit.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<CircuitBranchMetadata> GetBranchMetadata()
        {
            return new[]
            {
                new CircuitBranchMetadata
                {
                    N1 = Anode,
                    N2 = Cathode,
                    BranchType = BranchType.VoltageDefined
                }
            };
        }
    }
}