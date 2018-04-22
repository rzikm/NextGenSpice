﻿using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Devices;
using NextGenSpice.LargeSignal.Behaviors;
using NextGenSpice.Numerics.Equations;
using NextGenSpice.Numerics.Equations.Eq;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Large signal model for <see cref="VoltageSourceDevice" /> device.</summary>
    public class LargeSignalVoltageSource : TwoTerminalLargeSignalDevice<VoltageSourceDevice>
    {
        private VoltageStamper stamper;

        public LargeSignalVoltageSource(VoltageSourceDevice definitionDevice, IInputSourceBehavior behavior) :
            base(definitionDevice)
        {
            stamper = new VoltageStamper();
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
            Current = stamper.GetCurrent();
        }

        /// <summary>Performs necessary initialization of the device, like mapping to the equation system.</summary>
        /// <param name="adapter">The equation system builder.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void Initialize(IEquationSystemAdapter adapter, ISimulationContext context)
        {
            stamper.Register(adapter, Anode, Cathode);
        }

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
        ///     called once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(ISimulationContext context)
        {
            Voltage = Behavior.GetValue(context);
            stamper.Stamp(Voltage);
        }
    }
}