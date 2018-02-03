using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Equations;
using NextGenSpice.Core.Representation;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>
    ///     Defines basic methods and properties for large signal devices.
    /// </summary>
    public interface ILargeSignalDeviceModel : IAnalysisDeviceModel<LargeSignalCircuitModel>
    {
        /// <summary>
        ///     Name identifier of the corresponding element.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     If true, the device behavior is not linear is not constant and the <see cref="ApplyModelValues" /> function is
        ///     called every iteration during nonlinear solving.
        /// </summary>
        bool IsNonlinear { get; }

        /// <summary>
        ///     If true, the device behavior is not constant over time and the <see cref="ApplyModelValues" /> function is called
        ///     every timestep.
        /// </summary>
        bool IsTimeDependent { get; }

        /// <summary>
        ///     Allows models to register additional vairables to the linear system equations. E.g. branch current variables.
        /// </summary>
        /// <param name="builder">The equation system builder.</param>
        /// <param name="context">Context of current simulation.</param>
        void RegisterAdditionalVariables(IEquationSystemBuilder builder, ISimulationContext context);

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is called
        ///     once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        void ApplyModelValues(IEquationEditor equations, ISimulationContext context);

        /// <summary>
        ///     Applies model values before first DC bias has been established for the first time.
        /// </summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        void ApplyInitialCondition(IEquationEditor equations, ISimulationContext context);

        /// <summary>
        ///     Notifies model class that DC bias for given timepoint is established. This method can be used for processing
        ///     circuit equation solution
        ///     for current timepoint.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        void OnDcBiasEstablished(ISimulationContext context);
    }
}