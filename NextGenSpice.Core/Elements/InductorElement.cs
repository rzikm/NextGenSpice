using NextGenSpice.Models;

namespace NextGenSpice.Elements
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

        public override ILargeSignalDeviceModel GetLargeSignalModel()
        {
            return new InductorModel(this);
//            return new ShortCircuitModel(this);
        }

        public override ILargeSignalDeviceModel GetSmallSignalModel()
        {
            return new InductorModel(this);
        }
    }
}