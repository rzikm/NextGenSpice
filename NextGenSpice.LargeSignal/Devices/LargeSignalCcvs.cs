using NextGenSpice.Core.Devices;
using NextGenSpice.LargeSignal.Stamping;
using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Devices
{
    /// <summary>Large signal model for <see cref="CurrentControlledVoltageSource" /> device.</summary>
    public class LargeSignalCcvs : TwoTerminalLargeSignalDevice<CurrentControlledVoltageSource>
    {
        private readonly CcvsStamper stamper;

        private readonly LargeSignalVoltageSource ampermeter;

        public LargeSignalCcvs(CurrentControlledVoltageSource definitionDevice, LargeSignalVoltageSource ampermeterDevice) : base(definitionDevice)
        {
            stamper = new CcvsStamper();
            ampermeter = ampermeterDevice;
        }

        public double ReferenceCurrent => ampermeter.Current;

        /// <summary>Allows devices to register any additional variables.</summary>
        /// <param name="adapter">The equation system builder.</param>
        public override void RegisterAdditionalVariables(IEquationSystemAdapter adapter)
        {
            base.RegisterAdditionalVariables(adapter);
            stamper.RegisterVariable(adapter);
        }

        /// <summary>Performs necessary initialization of the device, like mapping to the equation system.</summary>
        /// <param name="adapter">The equation system builder.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void Initialize(IEquationSystemAdapter adapter, ISimulationContext context)
        {
            stamper.Register(adapter, Anode, Cathode, ampermeter.BranchVariable);
        }

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
        ///     called once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(ISimulationContext context)
        {
            stamper.Stamp(DefinitionDevice.Gain);
        }

        /// <summary>This method is called each time an equation is solved.</summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnEquationSolution(ISimulationContext context)
        {
            Current = stamper.GetCurrent();
            Voltage = ReferenceCurrent * DefinitionDevice.Gain;
        }
    }
}