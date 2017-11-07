namespace NextGenSpice.Core.Elements
{
    public class CapacitorElement : TwoNodeCircuitElement
    {
        public double Capacity { get; }
        public double InitialVoltage { get; }

        public CapacitorElement(double capacity, double initialVoltage = 0)
        {
            this.Capacity = capacity;
            InitialVoltage = initialVoltage;
        }
    }
}