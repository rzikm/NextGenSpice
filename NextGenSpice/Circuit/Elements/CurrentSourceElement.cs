namespace NextGenSpice.Circuit
{
    public class CurrentSourceElement : TwoNodeCircuitElement, ICanonicalElement
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

        public override void ApplyToEquationsPermanent(IEquationSystemBuilder equationSystem, SimulationContext context)
        {
            ApplyToEquationsPermanent(equationSystem, context);
        }

        public void ApplyToEquationsPermanent(IEquationEditor equationSystem, SimulationContext context)
        {
            equationSystem.AddRightHandSideEntry(Anode.Id, Current);
            equationSystem.AddRightHandSideEntry(Kathode.Id, -Current);
        }
    }

    public class VoltageSourceElement : TwoNodeCircuitElement
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

        public override void ApplyToEquationsPermanent(IEquationSystemBuilder equationSystem, SimulationContext context)
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