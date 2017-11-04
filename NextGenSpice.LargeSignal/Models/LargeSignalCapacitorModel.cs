using System;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalCapacitorModel : TwoNodeLargeSignalModel<CapacitorElement>, ITimeDependentLargeSignalDeviceModel
    {
        private double vc;
        private double gEq;
        private double iEq;

        public LargeSignalCapacitorModel(CapacitorElement parent) : base(parent)
        {
            gEq = 0;
            iEq = parent.InitialCurrent;
        }

        public void UpdateTimeDependentModel(SimulationContext context)
        {
            gEq = Parent.Capacity / context.Timestep;
            iEq = gEq * vc;
            vc = context.NodeVoltages[Parent.Anode] - context.NodeVoltages[Parent.Kathode];
        }

        public void ApplyTimeDependentModelValues(IEquationSystem equation, SimulationContext context)
        {
            equation
                .AddConductance(Anode, Kathode, gEq)
                .AddCurrent(Anode, Kathode, iEq);
        }
    }
}