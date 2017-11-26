using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpiceTest
{
    public class SwitchModel : TwoNodeLargeSignalModel<SwitchElement>
    {
        public bool IsOn { get; set; } = true;

        private int branchVariable;

        public SwitchModel(SwitchElement parent) : base(parent)
        {
        }

        public void UpdateTimeDependentModel(ISimulationContext context)
        {
        }

        public override void RegisterAdditionalVariables(IEquationSystemBuilder builder)
        {
            base.RegisterAdditionalVariables(builder);
            branchVariable = builder.AddVariable();
        }

        public void ApplyTimeDependentModelValues(IEquationSystem equation, ISimulationContext context)
        {
        }

        public void RollbackTimeDependentModel()
        {
        }

        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            if (IsOn)
                equations.AddVoltage(Anode, Kathode, branchVariable, 0);
            else
            {
                equations.AddMatrixEntry(Anode, branchVariable, 1);
                equations.AddMatrixEntry(Kathode, branchVariable, -1);

                equations.AddMatrixEntry(branchVariable, branchVariable, -1);

                equations.AddRightHandSideEntry(branchVariable, 0);
            }
        }
    }
}