using System;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.Core.Helpers;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalCapacitorModel : TwoNodeLargeSignalModel<CapacitorElement>, ITimeDependentLargeSignalDeviceModel
    {
        private struct CapacitorState
        {
            public double Vc;
            public double GEq;
            public double IEq;
        }

        private readonly StateHelper<CapacitorState> stateHelper;
        private ref CapacitorState State => ref stateHelper.Value;

        public LargeSignalCapacitorModel(CapacitorElement parent) : base(parent)
        {
            stateHelper = new StateHelper<CapacitorState>();
            State.GEq = 0;
            State.IEq = parent.InitialCurrent;
        }

        public void AdvanceTimeDependentModel(SimulationContext context)
        {
            stateHelper.Commit();

            State.GEq = Parent.Capacity / context.Timestep;
            State.IEq = State.GEq * State.Vc;
            State.Vc = context.NodeVoltages[Parent.Anode] - context.NodeVoltages[Parent.Kathode];
        }

        public void RollbackTimeDependentModel()
        {
            stateHelper.Rollback();
        }

        public void ApplyTimeDependentModelValues(IEquationSystem equation, SimulationContext context)
        {
            equation
                .AddConductance(Anode, Kathode, State.GEq)
                .AddCurrent(Anode, Kathode, State.IEq);
        }
    }


}