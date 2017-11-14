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
            public double REq;
            public double Il;
        }
        
        private readonly StateHelper<InductorState> stateHelper;

        private ref InductorState State => ref stateHelper.Value;

        private int additionalVariable;

        public double Current => State.Il;

        public LargeSignalInductorModel(InductorElement parent) : base(parent)
        {
            stateHelper = new StateHelper<InductorState>();
            State.REq = 0;
            State.Il = parent.InitialCurrent;
            State.VEq = 0; // model initially as short circuit
        }

        public override void Initialize(IEquationSystemBuilder builder)
        {
            additionalVariable = builder.AddVariable();
            base.Initialize(builder);
        }

        public void AdvanceTimeDependentModel(SimulationContext context)
        {
            stateHelper.Commit();
            State.REq = Parent.Inductance / context.Timestep;
            State.VEq = State.REq * State.Il;
        }

        public override void PostProcess(SimulationContext context)
        {
            base.PostProcess(context);
            State.Il = context.EquationSolution[additionalVariable];
        }

        public void RollbackTimeDependentModel()
        {
            stateHelper.Rollback();
        }

        public void ApplyTimeDependentModelValues(IEquationSystem equation, SimulationContext context)
        {
            equation.AddVoltage(Anode, Kathode, additionalVariable, -State.VEq);
            equation.AddMatrixEntry(additionalVariable, additionalVariable, -State.REq);
        }
    }
}