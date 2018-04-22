using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Devices;
using NextGenSpice.LargeSignal.Behaviors;
using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Large signal model for <see cref="VoltageSourceDevice" /> device.</summary>
    public class LargeSignalVoltageSource : TwoTerminalLargeSignalDevice<VoltageSourceDevice>
    {
        private int branchVariable = -1;

        public LargeSignalVoltageSource(VoltageSourceDevice definitionDevice, IInputSourceBehavior behavior) :
            base(definitionDevice)
        {
            Behavior = behavior;
        }

        /// <summary>Specifies how often the model should be updated.</summary>
        public override ModelUpdateMode UpdateMode => Behavior.UpdateMode;

        /// <summary>Strategy class specifying behavior of this source.</summary>
        private IInputSourceBehavior Behavior { get; }

        /// <summary>
        ///     Notifies model class that DC bias for given timepoint is established. This method can be used for processing
        ///     circuit equation solution for current timepoint.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnDcBiasEstablished(ISimulationContext context)
        {
            base.OnDcBiasEstablished(context);
            Current = context.GetSolutionForVariable(branchVariable);
        }

        /// <summary>
        ///     Allows models to register additional vairables to the linear system equations. E.g. branch current variables.
        ///     And perform other necessary initialization
        /// </summary>
        /// <param name="builder">The equation system builder.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void Initialize(IEquationSystemBuilder builder, ISimulationContext context)
        {
            base.Initialize(builder, context);
            branchVariable = builder.AddVariable();
        }

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
        ///     called once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            Voltage = Behavior.GetValue(context);
            equations.AddVoltage(Anode, Cathode, branchVariable, Voltage);
        }
    }
}