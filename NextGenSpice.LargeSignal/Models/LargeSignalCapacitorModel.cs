using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.Core.Helpers;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalCapacitorModel : TwoNodeLargeSignalModel<CapacitorElement>,
        ITimeDependentLargeSignalDeviceModel
    {
        private readonly StateHelper<CapacitorState> stateHelper;

        private int branchVariable;

        public LargeSignalCapacitorModel(CapacitorElement parent) : base(parent)
        {
            stateHelper = new StateHelper<CapacitorState>();
            State.GEq = 0;
            State.Vc = parent.InitialVoltage;
        }

        public double Current => State.Ic;
        private ref CapacitorState State => ref stateHelper.Value;

        public double Voltage => State.Vc;

        public override void Initialize(IEquationSystemBuilder builder)
        {
            base.Initialize(builder);
            branchVariable = builder.AddVariable();
        }

        public void UpdateTimeDependentModel(ISimulationContext context)
        {
            stateHelper.Commit();

            State.GEq = Parent.Capacity / context.Timestep;
            var vc = context.GetSolutionForVariable(Parent.Anode) - context.GetSolutionForVariable(Parent.Kathode);
            State.Vc = vc;
            State.IEq = State.GEq * State.Vc;
        }

        public void RollbackTimeDependentModel()
        {
            stateHelper.Rollback();
        }

        public override void PostProcess(ISimulationContext context)
        {
            base.PostProcess(context);
            State.Ic = context.GetSolutionForVariable(branchVariable);
        }

        public void ApplyTimeDependentModelValues(IEquationSystem equation, ISimulationContext context)
        {
            equation.AddMatrixEntry(branchVariable, Anode, State.GEq);
            equation.AddMatrixEntry(branchVariable, Kathode, -State.GEq);

            equation.AddMatrixEntry(Anode, branchVariable, 1);
            equation.AddMatrixEntry(Kathode, branchVariable, -1);

            equation.AddMatrixEntry(branchVariable, branchVariable, -1);

            equation.AddRightHandSideEntry(branchVariable, State.IEq);
        }

        private struct CapacitorState
        {
            public double Vc;
            public double GEq;
            public double IEq;
            public double Ic;
        }
    }
}