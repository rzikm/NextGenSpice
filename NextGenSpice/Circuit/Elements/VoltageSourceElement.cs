namespace NextGenSpice.Circuit
{
    public class VoltageSourceElement : TwoNodeCircuitElement, ICircuitModelElement
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

        public override ICircuitModelElement GetDcOperatingPointModel()
        {
            return this;
        }

        public override ICircuitModelElement GetTransientModel()
        {
            return this;
        }

        public void ApplyToEquationsPermanent(IEquationSystemBuilder equationSystem, SimulationContext context)
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