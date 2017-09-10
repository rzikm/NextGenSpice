namespace NextGenSpice.Circuit
{
    public class CurrentSourceElement : TwoNodeCircuitElement, ICanonicalElement, ICircuitModelElement
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
            ApplyToEquationsPermanent(equationSystem as IEquationEditor, context);
        }

        public void ApplyToEquationsPermanent(IEquationEditor equationSystem, SimulationContext context)
        {
            equationSystem.AddRightHandSideEntry(Anode.Id, Current);
            equationSystem.AddRightHandSideEntry(Kathode.Id, -Current);
        }
    }
}