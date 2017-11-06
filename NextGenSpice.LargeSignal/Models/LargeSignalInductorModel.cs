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
            public double Vc;
            public double VEq;
            public double GEq;
        }

        private readonly StateHelper<InductorState> stateHelper;

        private ref InductorState State => ref stateHelper.Value;

        private int additionalVariable;
        
        public LargeSignalInductorModel(InductorElement parent) : base(parent)
        {
            stateHelper = new StateHelper<InductorState>();
            State.GEq = double.PositiveInfinity;
            State.VEq = parent.InitialVoltage;
        }

        public override void Initialize(IEquationSystemBuilder builder)
        {
            additionalVariable = builder.AddVariable();
            base.Initialize(builder);
        }
        
        public void AdvanceTimeDependentModel(SimulationContext context)
        {
            stateHelper.Commit();

            State.GEq = Parent.Inductance / context.Timestep;
            State.VEq = State.GEq * State.Vc;
            State.Vc = context.NodeVoltages[Parent.Anode] - context.NodeVoltages[Parent.Kathode];
        }

        public void RollbackTimeDependentModel()
        {
            stateHelper.Rollback();
        }

        public void ApplyTimeDependentModelValues(IEquationSystem equation, SimulationContext context)
        {
            if (1/ State.GEq < double.Epsilon)
            {
                //TODO: Use different method of calculating?
                 equation.BindEquivalent(Anode, Kathode);
                return;
            }

            equation
                .AddConductance(Anode, Kathode, State.GEq)
                .AddVoltage(Anode, Kathode, additionalVariable, State.VEq);
            
        }
    }
}