using System;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>
    ///     Large signal model for <see cref="BjtElement" /> device.
    /// </summary>
    internal class LargeSignalBjtModel : LargeSignalModelBase<BjtElement>
    {
        public LargeSignalBjtModel(BjtElement definitionElement) : base(definitionElement)
        {
        }

        /// <summary>
        ///     If true, the device behavior is not constant over time and the
        ///     <see cref="ILargeSignalDeviceModel.ApplyModelValues" /> function is called
        ///     every timestep.
        /// </summary>
        public override bool IsTimeDependent => false;

        /// <summary>
        ///     If true, the device behavior is not linear is not constant and the
        ///     <see cref="ILargeSignalDeviceModel.ApplyModelValues" /> function is
        ///     called every iteration during nonlinear solving.
        /// </summary>
        public override bool IsNonlinear => true;

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is called
        ///     once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            throw new NotImplementedException();
        }
    }
}