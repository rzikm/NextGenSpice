namespace NextGenSpice.Core.Elements
{
    public class InductorElement : TwoNodeCircuitElement
    {
        public double Inductance { get; }
        public double InitialVoltage { get; }


        public InductorElement(double inductance, double initialVoltage = 0)
        {
            this.Inductance = inductance;
            InitialVoltage = initialVoltage;
        }
    }
}