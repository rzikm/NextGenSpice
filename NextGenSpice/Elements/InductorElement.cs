using NextGenSpice.Circuit;
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
        public override void Accept<T>(ICircuitVisitor<T> visitor)
        {
            throw new System.NotImplementedException();
        }

        public override ICircuitModelElement GetLargeSignalModel()
        {
            return new InductorElementModel(this);
//            return new ShortCircuitModel(this);
        }

        public override ICircuitModelElement GetSmallSignalModel()
        {
            return new InductorElementModel(this);
        }
    }
}