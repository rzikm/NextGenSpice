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

        public double Voltage => State.Vc;

        public LargeSignalCapacitorModel(CapacitorElement parent) : base(parent)
        {
            stateHelper = new StateHelper<CapacitorState>();
            State.GEq = 0;
            State.Vc = parent.InitialVoltage;
        }

        public void AdvanceTimeDependentModel(SimulationContext context)
        {
            stateHelper.Commit();

            State.GEq = Parent.Capacity / context.Timestep;
            State.Vc = context.EquationSolution[Parent.Anode] - context.EquationSolution[Parent.Kathode];
            State.IEq = State.GEq * State.Vc;
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