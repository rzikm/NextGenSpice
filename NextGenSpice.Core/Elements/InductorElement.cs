namespace NextGenSpice.Core.Elements
{
    public class InductorElement : TwoNodeCircuitElement
    {
        public double Inductance { get; }
        public double InitialCurrent { get; }


        public InductorElement(double inductance, double initialCurrent = 0)
        {
            this.Inductance = inductance;
            InitialCurrent = initialCurrent;
        }
    }
}