using System.Collections.Generic;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Representation;
using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Defines basic methods and properties for large signal devices.</summary>
    public interface ILargeSignalDeviceModel : IAnalysisDeviceModel<LargeSignalCircuitModel>
    {
        /// <summary>Name identifier of the corresponding device.</summary>
        string Name { get; }

        /// <summary>Specifies how often the model should be updated.</summary>
        ModelUpdateMode UpdateMode { get; }

        /// <summary>
        ///     Allows models to register additional vairables to the linear system equations. E.g. branch current variables.
        ///     And perform other necessary initialization
        /// </summary>
        /// <param name="builder">The equation system builder.</param>
        /// <param name="context">Context of current simulation.</param>
        void Initialize(IEquationSystemBuilder builder, ISimulationContext context);

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
        ///     called once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        void ApplyModelValues(IEquationEditor equations, ISimulationContext context);

        /// <summary>Applies model values before first DC bias has been established for the first time.</summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        void ApplyInitialCondition(IEquationEditor equations, ISimulationContext context);

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