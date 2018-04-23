using NextGenSpice.Core.Devices;
using NextGenSpice.LargeSignal.Behaviors;
using NextGenSpice.LargeSignal.Stamping;
using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Large signal model for <see cref="CurrentSourceDevice" /> device.</summary>
    public class LargeSignalCurrentSource : TwoTerminalLargeSignalDevice<CurrentSourceDevice>
    {
        private readonly CurrentStamper stamper;
        private readonly VoltageProxy voltage;

        public LargeSignalCurrentSource(CurrentSourceDevice definitionDevice, IInputSourceBehavior behavior) :
            base(definitionDevice)
        {
            stamper = new CurrentStamper();
            voltage = new VoltageProxy();
            Behavior = behavior;
        }

        /// <summary>Strategy class specifying behavior of this source.</summary>
        public IInputSourceBehavior Behavior { get; }

        /// <summary>Specifies how often the model should be updated.</summary>
        public override ModelUpdateMode UpdateMode => Behavior.UpdateMode;

        /// <summary>
        ///     Notifies model class that DC bias for given timepoint is established. This method can be used for processing
        ///     circuit equation solution for current timepoint.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnDcBiasEstablished(ISimulationContext context)
        {
            base.OnDcBiasEstablished(context);
            Voltage = voltage.GetValue();
        }

        /// <summary>Performs necessary initialization of the device, like mapping to the equation system.</summary>
        /// <param name="adapter">The equation system builder.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void Initialize(IEquationSystemAdapter adapter, ISimulationContext context)
        {
            stamper.Register(adapter, Anode, Cathode);
            voltage.Register(adapter, Anode, Cathode);
        }

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
        ///     called once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(ISimulationContext context)
        {
            Current = Behavior.GetValue(context);
            stamper.Stamp(Current);
        }
    }
}