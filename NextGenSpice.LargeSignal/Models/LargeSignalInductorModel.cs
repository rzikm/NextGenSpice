using System;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.Core.Helpers;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalInductorModel : TwoNodeLargeSignalModel<InductorElement>, ITimeDependentLargeSignalDeviceModel
    {
        private struct InductorState
        {
            public double VEq;
            public double Il;
            public double REq;
        }
        
        private readonly StateHelper<InductorState> stateHelper;

        private ref InductorState State => ref stateHelper.Value;

        private int additionalVariable;
        
        public LargeSignalInductorModel(InductorElement parent) : base(parent)
        {
            stateHelper = new StateHelper<InductorState>();
            State.REq = 0;
            State.Il = parent.InitialCurrent;
        }

        public override void Initialize(IEquationSystemBuilder builder)
        {
            additionalVariable = builder.AddVariable();
            base.Initialize(builder);
        }

        public void AdvanceTimeDependentModel(SimulationContext context)
        {
            stateHelper.Commit();

            var vl = context.EquationSolution[Anode] - context.EquationSolution[Kathode];
            State.REq = Parent.Inductance / context.Timestep;
            State.VEq = State.REq * State.Il;
            State.Il = vl / State.REq;
        }

        public void RollbackTimeDependentModel()
        {
            stateHelper.Rollback();
        }

        public void ApplyTimeDependentModelValues(IEquationSystem equation, SimulationContext context)
        {
            equation.AddVoltage(Anode, Kathode, additionalVariable, State.VEq);
            equation.AddMatrixEntry(additionalVariable, additionalVariable, -State.REq);
        }
    }
}