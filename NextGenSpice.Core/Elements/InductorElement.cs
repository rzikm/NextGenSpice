using System;

namespace NextGenSpice.Core.Elements
{
    /// <summary>
    /// Class that represents an inductor device.
    /// </summary>
    public class InductorElement : TwoNodeCircuitElement
    {
        /// <summary>
        /// Inductance of the device in henry.
        /// </summary>
        public double Inductance { get; }

        /// <summary>
        /// Initial current flowing from the positive node to the negative node through the inductor in volts.
        /// </summary>
        public double? InitialCurrent { get; }


        public InductorElement(double inductance, double? initialCurrent = null, string name = null) : base(name)
        {
            this.Inductance = inductance;
            InitialCurrent = initialCurrent;
        }
    }
}