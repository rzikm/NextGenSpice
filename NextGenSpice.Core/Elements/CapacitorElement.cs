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

        public override ILargeSignalDeviceModel GetLargeSignalModel()
        {
            return new CapacitorModel(this);
        }

        public override ILargeSignalDeviceModel GetSmallSignalModel()
        {
            return new CapacitorModel(this);
        }
    }
}