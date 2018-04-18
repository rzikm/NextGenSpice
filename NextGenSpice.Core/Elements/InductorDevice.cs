﻿using System.Collections.Generic;

namespace NextGenSpice.Core.Devices
{
    /// <summary>Class that represents an inductor device.</summary>
    public class InductorDevice : TwoTerminalCircuitDevice
    {
        public InductorDevice(double inductance, double? initialCurrent = null, string name = null) : base(name)
        {
            Inductance = inductance;
            InitialCurrent = initialCurrent;
        }

        /// <summary>Inductance of the device in henry.</summary>
        public double Inductance { get; set; }

        /// <summary>Initial current flowing from the positive node to the negative node through the inductor in volts.</summary>
        public double? InitialCurrent { get; set; }

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