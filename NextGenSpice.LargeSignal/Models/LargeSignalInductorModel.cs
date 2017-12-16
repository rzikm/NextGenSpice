using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.Core.Helpers;
using NextGenSpice.LargeSignal.NumIntegration;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalInductorModel : TwoNodeLargeSignalModel<InductorElement>
    {

        private int branchVariable;

        private IIntegrationMethod IntegrationMethod { get; }

        public LargeSignalInductorModel(InductorElement parent) : base(parent)
        {
            IntegrationMethod = new TrapezoidalIntegrationMethod();
        }


        public override bool IsNonlinear => false;
        public override bool IsTimeDependent => true;

        public double Current { get; private set; }
        public double Voltage { get; private set; }

        public override void RegisterAdditionalVariables(IEquationSystemBuilder builder, ISimulationContext context)
        {
            branchVariable = builder.AddVariable();
            base.RegisterAdditionalVariables(builder, context);
        }

        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            var (req, veq) = IntegrationMethod.GetEquivalents(Parent.Inductance / context.TimeStep);

            equations.AddVoltage(Anode, Kathode, branchVariable, -veq);
            equations.AddMatrixEntry(branchVariable, branchVariable, -req);
        }

        public override void ApplyInitialCondition(IEquationEditor equations, ISimulationContext context)
        {
            if (Parent.InitialCurrent.HasValue)
            {
                equations.AddCurrent(Anode, Kathode, Parent.InitialCurrent.Value);
            }
            else
            {
                // model as short circuit
                equations.AddVoltage(Anode, Kathode, branchVariable, 0);
            }
        }
        

        public override void OnDcBiasEstablished(ISimulationContext context)
        {
            base.OnDcBiasEstablished(context);
            Current = context.GetSolutionForVariable(branchVariable);
            Voltage = context.GetSolutionForVariable(Anode) - context.GetSolutionForVariable(Kathode);

            IntegrationMethod.SetState(Voltage, Current);
        }
    }
}