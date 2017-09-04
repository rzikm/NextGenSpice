namespace NextGenSpice.Circuit
{
    public class CurrentSourceElement : TwoNodeCircuitElement
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

        public override void ApplyToEquations(ICircuitEquationSystem equationSystem)
        {
            equationSystem.AddCurrent(Anode.Id, Current);
            equationSystem.AddCurrent(Kathode.Id, -Current);
        }
    }
}