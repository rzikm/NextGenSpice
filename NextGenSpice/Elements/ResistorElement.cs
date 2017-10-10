using NextGenSpice.Circuit;
using NextGenSpice.Equations;

namespace NextGenSpice.Elements
{
    public class ResistorElement : SimpleTwoNodeElement, ICanonicalElement
    {
        public double Resistance { get; internal set; }

        public ResistorElement(double resistance)
        {
            this.Resistance = resistance;
        }
        public override void Accept<T>(ICircuitVisitor<T> visitor)
        {
            throw new System.NotImplementedException();
        }

        public override ICircuitModelElement GetLargeSignalModel()
        {
            return this;
        }

        public override ICircuitModelElement GetSmallSignalModel()
        {
            return this;
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