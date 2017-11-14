using System;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.Core.Helpers;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalCapacitorModel : TwoNodeLargeSignalModel<CapacitorElement>, ITimeDependentLargeSignalDeviceModel
    {
        public double Current => State.Ic;

        private struct CapacitorState
        {
            public double Vc;
            public double GEq;
            public double IEq;
            public double Ic;
        }

        private readonly StateHelper<CapacitorState> stateHelper;
        private ref CapacitorState State => ref stateHelper.Value;

        private int branchVariable;

        public double Voltage => State.Vc;

        public override void Initialize(IEquationSystemBuilder builder)
        {
            base.Initialize(builder);
//            branchVariable = builder.AddVariable();
        }

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
            var vc = context.EquationSolution[Parent.Anode] - context.EquationSolution[Parent.Kathode];
            State.Vc = vc;
            State.IEq = State.GEq * State.Vc;
        }

        public void RollbackTimeDependentModel()
        {
            stateHelper.Rollback();
        }

        public override void PostProcess(SimulationContext context)
        {
            base.PostProcess(context);
            State.Ic = context.EquationSolution[branchVariable];
        }

        public void ApplyTimeDependentModelValues(IEquationSystem equation, SimulationContext context)
        {
            equation
                .AddConductance(Anode, Kathode, State.GEq)
                .AddCurrent(Anode, Kathode, State.IEq);

//            equation.AddMatrixEntry(branchVariable, Anode, State.GEq);
//            equation.AddMatrixEntry(branchVariable, Kathode, -State.GEq);
//
//            equation.AddMatrixEntry(Anode, branchVariable, 1);
//            equation.AddMatrixEntry(Kathode, branchVariable, -1);
//
//            equation.AddMatrixEntry(branchVariable, branchVariable, -1);
//
//            equation.AddRightHandSideEntry(branchVariable, State.IEq);
        }
    }


}