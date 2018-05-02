﻿using System.Collections.Generic;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal.Stamping;
using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Devices
{
    /// <summary>Large signal model for <see cref="VoltageControlledVoltageSource" /> device.</summary>
    public class LargeSignalVcvs : LargeSignalDeviceBase<VoltageControlledVoltageSource>,
        ITwoTerminalLargeSignalDevice
    {
        private readonly VcvsStamper stamper;
        private readonly VoltageProxy voltage;

        public LargeSignalVcvs(VoltageControlledVoltageSource definitionDevice) : base(definitionDevice)
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
            stamper.Register(adapter, Anode, Cathode, ReferenceAnode, ReferenceCathode);
            voltage.Register(adapter, Anode, Cathode);
        }

        /// <summary>Voltage across this device, difference of potential between positive and negative terminals.</summary>
        public double Voltage { get; private set; }

        /// <summary>Current flowing from positive terminal to negative terminal through the device.</summary>
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
        public override IEnumerable<IDeviceStateProvider> GetDeviceStateProviders()
        {
            return new[]
            {
                new SimpleDeviceStateProvider("I", () => Current),
                new SimpleDeviceStateProvider("V", () => Voltage)
            };
        }

        /// <summary>This method is called each time an equation is solved.</summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnEquationSolution(ISimulationContext context)
        {
            Voltage = voltage.GetValue();
            Current = stamper.GetCurrent();
        }
    }
}