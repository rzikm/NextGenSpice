using System;
using NextGenSpice.Equations;

namespace NextGenSpice.Elements
{
    public class Resistor : SimpleTwoNodeElement, ICanonicalElement
    {
        public double Resistance { get; set; }

        public Resistor(double resistance)
        {
            this.Resistance = resistance;
        }

        public override ILargeSignalDeviceModel GetLargeSignalModel()
        {
            return this;
        }

        public override ILargeSignalDeviceModel GetSmallSignalModel()
        {
            throw new NotImplementedException();
        }

        public override void ApplyLinearModelValues(IEquationEditor equationSystem, SimulationContext context)
        {
            equationSystem.AddMatrixEntry(Kathode.Id, Anode.Id, -1 / Resistance);
            equationSystem.AddMatrixEntry(Anode.Id, Kathode.Id, -1 / Resistance);
            equationSystem.AddMatrixEntry(Anode.Id, Anode.Id, 1 / Resistance);
            equationSystem.AddMatrixEntry(Kathode.Id, Kathode.Id, 1 / Resistance);
        }
    }
}