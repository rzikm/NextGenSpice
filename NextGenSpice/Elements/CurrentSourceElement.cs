using NextGenSpice.Circuit;
using NextGenSpice.Equations;

namespace NextGenSpice.Elements
{
    public class CurrentSourceElement : SimpleTwoNodeElement
    {
        public double Current { get; internal set; }
        public CurrentSourceElement(double current)
        {
            Current = current;
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
            equationSystem.AddRightHandSideEntry(Anode.Id, Current);
            equationSystem.AddRightHandSideEntry(Kathode.Id, -Current);
        }
    }
}