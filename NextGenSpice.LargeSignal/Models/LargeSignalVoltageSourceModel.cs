using System;
using NextGenSpice.Equations;

namespace NextGenSpice.Elements
{
    public class LargeSignalVoltageSourceModel : TwoNodeCircuitElement, ILinearLargeSignalDeviceModel
    {
        public double Voltage { get; internal set; }
        public LargeSignalVoltageSourceModel(double voltage)
        {
            Voltage = voltage;
        }

        public override ILargeSignalDeviceModel GetLargeSignalModel()
        {
            return this;
        }

        public override ILargeSignalDeviceModel GetSmallSignalModel()
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            
        }

        public void ApplyLinearModelValues(IEquationSystemBuilder equationSystem, SimulationContext context)
        {
            var i = equationSystem.AddVariable();
            equationSystem.AddMatrixEntry(i, Anode.Id, 1);
            equationSystem.AddMatrixEntry(i, Kathode.Id, -1);

            equationSystem.AddMatrixEntry(Anode.Id, i, 1);
            equationSystem.AddMatrixEntry(Kathode.Id, i, -1);

            equationSystem.AddRightHandSideEntry(i, Voltage);
        }
    }
}