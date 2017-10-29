namespace NextGenSpice.Core.Elements
{
    public class CapacitorElement : TwoNodeCircuitElement
    {
        public double Capacity { get; }
        public double InitialCurrent { get; }

        public CapacitorElement(double capacity, double initialCurrent = 0)
        {
            this.Capacity = capacity;
            InitialCurrent = initialCurrent;
        }
    }
}