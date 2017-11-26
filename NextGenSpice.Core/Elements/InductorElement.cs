using System;

namespace NextGenSpice.Core.Elements
{
    public class InductorElement : TwoNodeCircuitElement
    {
        public double Inductance { get; }
        public double? InitialCurrent { get; }


        public InductorElement(double inductance, double? initialCurrent = null, string name = null) : base(name)
        {
            this.Inductance = inductance;
            InitialCurrent = initialCurrent;
        }
    }
}