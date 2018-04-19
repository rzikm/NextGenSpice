﻿using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Devices;
using NextGenSpice.LargeSignal.Behaviors;
using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Large signal model for <see cref="CurrentSourceDevice" /> device.</summary>
    public class LargeSignalCurrentSourceModel : TwoTerminalLargeSignalDeviceModel<CurrentSourceDevice>
    {
        public LargeSignalCurrentSourceModel(CurrentSourceDevice definitionDevice, IInputSourceBehavior behavior) :
            base(definitionDevice)
        {
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
            Voltage = context.GetSolutionForVariable(Anode) - context.GetSolutionForVariable(Cathode);
        }

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
        ///     called once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            Current = Behavior.GetValue(context);
            equations.AddCurrent(Anode, Cathode, -Current);
        }
    }
}