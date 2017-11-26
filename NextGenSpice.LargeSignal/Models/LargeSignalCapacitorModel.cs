using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.Core.Helpers;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalCapacitorModel : TwoNodeLargeSignalModel<CapacitorElement>
    {
        private int branchVariable;

        public LargeSignalCapacitorModel(CapacitorElement parent) : base(parent)
        {
        }

        public double Current { get; private set; }

        public double Voltage { get; private set; }

        public override void RegisterAdditionalVariables(IEquationSystemBuilder builder)
        {
            base.RegisterAdditionalVariables(builder);
            branchVariable = builder.AddVariable();
        }

        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            var geq = Parent.Capacity / context.Timestep * 2;
            var ieq = geq * Voltage + Current;

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
        }
    }
}