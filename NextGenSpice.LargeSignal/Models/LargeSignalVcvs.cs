using System.Collections.Generic;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Devices;
using NextGenSpice.Numerics.Equations;
using NextGenSpice.Numerics.Equations.Eq;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Large signal model for <see cref="VoltageControlledVoltageSourceDevice" /> device.</summary>
    public class LargeSignalVcvs : LargeSignalDeviceBase<VoltageControlledVoltageSourceDevice>,
        ITwoTerminalLargeSignalDevice
    {
        private VcvsStamper stamper;
        private VoltageProxy voltage;

        public LargeSignalVcvs(VoltageControlledVoltageSourceDevice definitionDevice) : base(definitionDevice)
        {
            stamper = new VcvsStamper();
            voltage = new VoltageProxy();
        }

        /// <summary>Id of node connected to positive terminal of this device.</summary>
        public int Anode => DefinitionDevice.ConnectedNodes[0];

        /// <summary>Id of node connected to negative terminal of this device.</summary>
        public int Cathode => DefinitionDevice.ConnectedNodes[1];

        /// <summary>Positive terminal of the reference voltage.</summary>
        public int ReferenceAnode => DefinitionDevice.ConnectedNodes[2];

        /// <summary>Negative terminal of the reference voltage.</summary>
        public int ReferenceCathode => DefinitionDevice.ConnectedNodes[3];

        /// <summary>Performs necessary initialization of the device, like mapping to the equation system.</summary>
        /// <param name="adapter">The equation system builder.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void Initialize(IEquationSystemAdapter adapter, ISimulationContext context)
        {
            stamper.Register(adapter, Anode, Cathode, ReferenceAnode, ReferenceCathode);
            voltage.Register(adapter, Anode, Cathode);
        }

        /// <summary>Specifies how often the model should be updated.</summary>
        public override ModelUpdateMode UpdateMode =>
            ModelUpdateMode.Always; // due to possible dependencies on nonlinear devices.

        public double Voltage { get; private set; }

        public double Current { get; private set; }

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
        ///     called once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(ISimulationContext context)
        {
            stamper.Stamp(DefinitionDevice.Gain);
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
            Current = stamper.GetCurrent();
        }
    }

    public class VcvsStamper
    {
        private IEquationSystemSolutionProxy cur;

        private IEquationSystemCoefficientProxy nab;
        private IEquationSystemCoefficientProxy ncb;
        private IEquationSystemCoefficientProxy nba;
        private IEquationSystemCoefficientProxy nbc;
        private IEquationSystemCoefficientProxy nbra;
        private IEquationSystemCoefficientProxy nbrc;

        public int BranchVariable { get; private set; }

        public void Register(IEquationSystemAdapter adapter, int anode, int cathode, int ranode, int rcathode)
        {
            BranchVariable = adapter.AddVariable();
            nab = adapter.GetMatrixCoefficientProxy(anode, BranchVariable);
            ncb = adapter.GetMatrixCoefficientProxy(cathode, BranchVariable);
            nba = adapter.GetMatrixCoefficientProxy(BranchVariable, anode);
            nbc = adapter.GetMatrixCoefficientProxy(BranchVariable, cathode);
            nbra = adapter.GetMatrixCoefficientProxy(BranchVariable, ranode);
            nbrc = adapter.GetMatrixCoefficientProxy(BranchVariable, rcathode);
        }

        public void Stamp(double gain)
        {
            nab.Add(1);
            ncb.Add(-1);
            nba.Add(1);
            nbc.Add(-1);
            nbra.Add(gain);
            nbrc.Add(gain);
        }

        public double GetCurrent()
        {
            return cur.GetValue();
        }
    }
}