using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Equations;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpiceTest
{
    public class SwitchModel : TwoNodeLargeSignalModel<SwitchElement>
    {
        private int branchVariable;

        public SwitchModel(SwitchElement definitionElement) : base(definitionElement)
        {
        }

        public bool IsOn { get; set; } = true;

        /// <summary>
        ///     Specifies how often the model should be updated.
        /// </summary>
        public override ModelUpdateMode UpdateMode
        {
            get => ModelUpdateMode.TimePoint;
        }

        public void UpdateTimeDependentModel(ISimulationContext context)
        {
        }

        public override void RegisterAdditionalVariables(IEquationSystemBuilder builder, ISimulationContext context)
        {
            base.RegisterAdditionalVariables(builder, context);
            branchVariable = builder.AddVariable();
        }

        public void ApplyTimeDependentModelValues(IEquationEditor equation, ISimulationContext context)
        {
        }

        public void RollbackTimeDependentModel()
        {
        }

        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            if (IsOn)
            {
                equations.AddVoltage(Anode, Cathode, branchVariable, 0);
            }
            else
            {
                equations.AddMatrixEntry(Anode, branchVariable, 1);
                equations.AddMatrixEntry(Cathode, branchVariable, -1);

                equations.AddMatrixEntry(branchVariable, branchVariable, -1);

                equations.AddRightHandSideEntry(branchVariable, 0);
            }
        }
    }
}