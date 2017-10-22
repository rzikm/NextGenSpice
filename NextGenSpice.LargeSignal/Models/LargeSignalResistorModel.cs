using System;
using NextGenSpice.Equations;

namespace NextGenSpice.Elements
{
    public class LargeSignalResistorModel : TwoNodeCircuitElement, ILinearLargeSignalDeviceModel
    {
        public double Resistance { get; set; }

        public LargeSignalResistorModel(double resistance)
        {
            this.Resistance = resistance;
        }

        public override ILargeSignalDeviceModel GetLargeSignalModel()
        {
            return this;
        }

        public override ILargeSignalDeviceModel GetSmallSignalModel()
        {
            throw new NotImplementedException();
        }

        public void ApplyLinearModelValues(IEquationEditor equationSystem, SimulationContext context)
        {
            equationSystem.AddMatrixEntry(Kathode.Id, Anode.Id, -1 / Resistance);
            equationSystem.AddMatrixEntry(Anode.Id, Kathode.Id, -1 / Resistance);
            equationSystem.AddMatrixEntry(Anode.Id, Anode.Id, 1 / Resistance);
            equationSystem.AddMatrixEntry(Kathode.Id, Kathode.Id, 1 / Resistance);
        }

        public void Initialize()
        {
        }

        public void ApplyLinearModelValues(IEquationSystemBuilder equationSystem, SimulationContext context)
        {
            ApplyLinearModelValues(equationSystem, context);
        }
    }
}