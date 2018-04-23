using System.Collections.Generic;
using NextGenSpice.Core.Representation;
using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Defines basic methods and properties for large signal devices.</summary>
    public interface ILargeSignalDevice : IAnalysisDeviceModel<LargeSignalCircuitModel>
    {
        /// <summary>Specifies how often the model should be updated.</summary>
        ModelUpdateMode UpdateMode { get; }

        /// <summary>Performs necessary initialization of the device, like mapping to the equation system.</summary>
        /// <param name="adapter">The equation system builder.</param>
        /// <param name="context">Context of current simulation.</param>
        /// >
        void Initialize(IEquationSystemAdapter adapter, ISimulationContext context);

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
        ///     called once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        void ApplyModelValues(ISimulationContext context);

        /// <summary>Applies model values before first DC bias has been established for the first time.</summary>
        /// <param name="context">Context of current simulation.</param>
        void ApplyInitialCondition(ISimulationContext context);

        /// <summary>
        ///     Notifies model class that DC bias for given timepoint is established. This method can be used for processing
        ///     circuit equation solution for current timepoint.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        void OnDcBiasEstablished(ISimulationContext context);

        /// <summary>Gets stats provider instances for this device.</summary>
        /// <returns>IPrintValueProviders for specified attribute.</returns>
        IEnumerable<IDeviceStatsProvider> GetDeviceStatsProviders();
    }
}