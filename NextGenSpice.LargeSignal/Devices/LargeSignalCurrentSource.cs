using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Devices;
using NextGenSpice.LargeSignal.Stamping;
using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Devices
{
    /// <summary>Large signal model for <see cref="CurrentSource" /> device.</summary>
    public class LargeSignalCurrentSource : TwoTerminalLargeSignalDevice<CurrentSource>
    {
        private readonly CurrentStamper stamper;
        private readonly VoltageProxy voltage;

        public LargeSignalCurrentSource(CurrentSource definitionDevice) :
            base(definitionDevice)
        {
            stamper = new CurrentStamper();
            voltage = new VoltageProxy();
            Behavior = DefinitionDevice.Behavior;
        }

        /// <summary>Strategy class specifying behavior of this source.</summary>
        public InputSourceBehavior Behavior { get; }

        /// <summary>This method is called each time an equation is solved.</summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnEquationSolution(ISimulationContext context)
        {
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
            Current = Behavior.GetValue(context.TimePoint);
            stamper.Stamp(Current);
        }
    }
}