namespace NextGenSpice.Circuit
{
    public class RezistorElement : TwoNodeCircuitElement
    {
        public double Resistance { get; internal set; }

        public RezistorElement(double resistance)
        {
            this.Resistance = resistance;
        }
        public override void Accept<T>(ICircuitVisitor<T> visitor)
        {
            throw new System.NotImplementedException();
        }

        public override void ApplyToEquationsPermanent(IEquationEditor equationSystem, SimulationContext context)
        {
            equationSystem.AddMatrixEntry(Kathode.Id, Anode.Id, -1/Resistance);
            equationSystem.AddMatrixEntry(Anode.Id, Kathode.Id, -1/Resistance);
            equationSystem.AddMatrixEntry(Anode.Id, Anode.Id, 1/Resistance);
            equationSystem.AddMatrixEntry(Kathode.Id, Kathode.Id, 1/Resistance);
        }
    }
}