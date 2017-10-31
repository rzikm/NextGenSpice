﻿using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalVoltageSourceModel : TwoNodeLargeSignalModel<VoltageSourceElement>, ILinearLargeSignalDeviceModel
    {
        public double Voltage => Parent.Voltage;

        private int additionalVariable = -1;
        public LargeSignalVoltageSourceModel(VoltageSourceElement parent) : base(parent)
        {
        }

        public override void Initialize(IEquationSystemBuilder builder)
        {
            base.Initialize(builder);
            additionalVariable = builder.AddVariable();
        }

        public void ApplyLinearModelValues(IEquationEditor equationSystem, SimulationContext context)
        {
            equationSystem.AddMatrixEntry(additionalVariable, Anode, 1);
            equationSystem.AddMatrixEntry(additionalVariable, Kathode, -1);

            equationSystem.AddMatrixEntry(Anode, additionalVariable, 1);
            equationSystem.AddMatrixEntry(Kathode, additionalVariable, -1);

            equationSystem.AddRightHandSideEntry(additionalVariable, Voltage);
        }
    }
}