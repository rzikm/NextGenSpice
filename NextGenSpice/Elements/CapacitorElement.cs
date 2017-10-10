using NextGenSpice.Circuit;
using NextGenSpice.Models;

namespace NextGenSpice.Elements
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
        public override void Accept<T>(ICircuitVisitor<T> visitor)
        {
            throw new System.NotImplementedException();
        }

        public override ICircuitModelElement GetLargeSignalModel()
        {
            return new CapacitorElementModel(this);
        }

        public override ICircuitModelElement GetSmallSignalModel()
        {
            return new CapacitorElementModel(this);
        }
    }
}