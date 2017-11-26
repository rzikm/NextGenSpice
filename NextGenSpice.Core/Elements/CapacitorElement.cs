namespace NextGenSpice.Core.Elements
{
    public class CapacitorElement : TwoNodeCircuitElement
    {
        public double Capacity { get; }
        public double? InitialVoltage { get; }

        public CapacitorElement(double capacity, double? initialVoltage = null, string name = null) : base(name)
        {
            this.Capacity = capacity;
            InitialVoltage = initialVoltage;
        }
    }
}