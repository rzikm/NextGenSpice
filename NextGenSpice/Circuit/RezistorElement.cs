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

        public override void ApplyToEquations(ICircuitEquationSystem equationSystem)
        {
            equationSystem.AddConductance(Anode.Id, Kathode.Id, -1/Resistance);
            equationSystem.AddConductance(Anode.Id, Anode.Id, 1/Resistance);
            equationSystem.AddConductance(Kathode.Id, Kathode.Id, 1/Resistance);
        }
    }
}