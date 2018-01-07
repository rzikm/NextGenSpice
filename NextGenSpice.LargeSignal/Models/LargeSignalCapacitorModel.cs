using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.Core.Helpers;
using NextGenSpice.LargeSignal.NumIntegration;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalCapacitorModel : TwoNodeLargeSignalModel<CapacitorElement>
    {
        private int branchVariable;
        private IIntegrationMethod IntegrationMethod { get; }

        public LargeSignalCapacitorModel(CapacitorElement parent) : base(parent)
        {
            IntegrationMethod = new GearIntegrationMethod(3);
//            IntegrationMethod = new AdamsMoultonIntegrationMethod(4);
//            IntegrationMethod = new BackwardEulerIntegrationMethod();
        }


        public override bool IsNonlinear => false;
        public override bool IsTimeDependent => true;
        
        private double geq;

        public override void RegisterAdditionalVariables(IEquationSystemBuilder builder, ISimulationContext context)
        {
            base.RegisterAdditionalVariables(builder, context);
            branchVariable = builder.AddVariable();
        }

        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            var (geq, ieq) = IntegrationMethod.GetEquivalents(Parent.Capacity / context.TimeStep);
            
            equations.AddMatrixEntry(branchVariable, Anode, geq);
            equations.AddMatrixEntry(branchVariable, Kathode, -geq);
            AddBranchCurrent(equations, ieq);
        }

        private void AddBranchCurrent(IEquationEditor equations, double ieq)
        {
            equations.AddMatrixEntry(Anode, branchVariable, 1);
            equations.AddMatrixEntry(Kathode, branchVariable, -1);

            equations.AddMatrixEntry(branchVariable, branchVariable, -1);

            equations.AddRightHandSideEntry(branchVariable, ieq);
        }

        public override void ApplyInitialCondition(IEquationEditor equations, ISimulationContext context)
        {
            if (Parent.InitialVoltage.HasValue)
            {
                equations.AddVoltage(Anode, Kathode, branchVariable, Parent.InitialVoltage.Value);
            }
            else
            {
                // model as open circuit
                AddBranchCurrent(equations, 0);
            }
        }

        public override void OnDcBiasEstablished(ISimulationContext context)
        {
            base.OnDcBiasEstablished(context);
            Current = context.GetSolutionForVariable(branchVariable);

            var vc = context.GetSolutionForVariable(Parent.Anode) - context.GetSolutionForVariable(Parent.Kathode);
            Voltage = vc;
            
            IntegrationMethod.SetState(Current, Voltage);
        }
    }
}