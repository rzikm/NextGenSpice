using System.Collections.Generic;
using NextGenSpice.Core.Devices;
using NextGenSpice.LargeSignal.Stamping;
using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Large signal model for <see cref="VoltageControlledVoltageSourceDevice" /> device.</summary>
    public class LargeSignalVccs : LargeSignalDeviceBase<VoltageControlledCurrentSourceDevice>,
        ITwoTerminalLargeSignalDevice
    {
        private readonly VoltageProxy refvoltage;
        private readonly VccsStamper stamper;
        private readonly VoltageProxy voltage;

        public LargeSignalVccs(VoltageControlledCurrentSourceDevice definitionDevice) : base(definitionDevice)
        {
            voltage = new VoltageProxy();
            stamper = new VccsStamper();
            refvoltage = new VoltageProxy();
        }

        /// <summary>Id of node connected to positive terminal of this device.</summary>
        public int Anode => DefinitionDevice.ConnectedNodes[0];

        /// <summary>Id of node connected to negative terminal of this device.</summary>
        public int Cathode => DefinitionDevice.ConnectedNodes[1];

        /// <summary>Positive terminal of the reference voltage.</summary>
        public int ReferenceAnode => DefinitionDevice.ConnectedNodes[2];

        /// <summary>Negative terminal of the reference voltage.</summary>
        public int ReferenceCathode => DefinitionDevice.ConnectedNodes[3];

        /// <summary>Specifies how often the model should be updated.</summary>
        public override ModelUpdateMode UpdateMode =>
            ModelUpdateMode.Always; // due to possible dependencies on nonlinear devices.

        public double Voltage { get; private set; }

        public double Current { get; private set; }

        /// <summary>Performs necessary initialization of the device, like mapping to the equation system.</summary>
        /// <param name="adapter">The equation system builder.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void Initialize(IEquationSystemAdapter adapter, ISimulationContext context)
        {
            voltage.Register(adapter, Anode, Cathode);
            refvoltage.Register(adapter, ReferenceAnode, ReferenceCathode);
        }

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
        ///     called once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(ISimulationContext context)
        {
            stamper.Stamp(DefinitionDevice.TransConductance);
        }

        /// <summary>
        ///     Gets provider instance for specified attribute value or null if no provider for requested parameter exists.
        ///     For example "I" for the current flowing throught the two terminal device.
        /// </summary>
        /// <returns>IPrintValueProvider for specified attribute.</returns>
        public override IEnumerable<IDeviceStatsProvider> GetDeviceStatsProviders()
        {
            return new[]
            {
                new SimpleDeviceStatsProvider("I", () => Current),
                new SimpleDeviceStatsProvider("V", () => Voltage)
            };
        }

        /// <summary>
        ///     Notifies model class that DC bias for given timepoint is established. This method can be used for processing
        ///     circuit equation solution for current timepoint.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnDcBiasEstablished(ISimulationContext context)
        {
            base.OnDcBiasEstablished(context);
            Voltage = voltage.GetValue();
            Current = refvoltage.GetValue() * DefinitionDevice.TransConductance;
        }
    }
}