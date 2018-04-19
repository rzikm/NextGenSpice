using NextGenSpice.Core.Circuit;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;
using NextGenSpice.Numerics.Equations;

namespace NextGenSpiceTest
{
    public class SwitchModel : TwoTerminalLargeSignalDeviceModel<SwitchDevice>
    {
        private int branchVariable;

        public SwitchModel(SwitchDevice definitionDevice) : base(definitionDevice)
        {
        }

        public bool IsOn { get; set; } = true;

        /// <summary>Specifies how often the model should be updated.</summary>
        public override ModelUpdateMode UpdateMode
        {
            get => ModelUpdateMode.TimePoint;
        }

        public void UpdateTimeDependentModel(ISimulationContext context)
        {
        }

        public override void Initialize(IEquationSystemBuilder builder, ISimulationContext context)
        {
            base.Initialize(builder, context);
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