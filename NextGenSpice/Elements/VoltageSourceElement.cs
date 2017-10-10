using NextGenSpice.Circuit;
using NextGenSpice.Equations;

namespace NextGenSpice.Elements
{
    public class VoltageSourceElement : TwoNodeCircuitElement, ILinearCircuitModelElement
    {
        public double Voltage { get; internal set; }
        public VoltageSourceElement(double voltage)
        {
            Voltage = voltage;
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

        public void Initialize()
        {
            
        }

        public void ApplyLinearModelValues(IEquationSystemBuilder equationSystem, SimulationContext context)
        {
            var i = equationSystem.AddVariable();
            equationSystem.AddMatrixEntry(i, Anode.Id, 1);
            equationSystem.AddMatrixEntry(i, Kathode.Id, -1);

            equationSystem.AddMatrixEntry(Anode.Id, i, 1);
            equationSystem.AddMatrixEntry(Kathode.Id, i, -1);

            equationSystem.AddRightHandSideEntry(i, Voltage);
        }
    }
}