using NextGenSpice.Core.Representation;
using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Defines basic methods and properties for large signal devices.</summary>
    public interface ILargeSignalDevice : IAnalysisDeviceModel<LargeSignalCircuitModel>
    {
        /// <summary>Allows devices to register any additional variables.</summary>
        /// <param name="adapter">The equation system builder.</param>
        void RegisterAdditionalVariables(IEquationSystemAdapter adapter);

        /// <summary>Performs necessary initialization of the device, like mapping to the equation system.</summary>
        /// <param name="adapter">The equation system builder.</param>
        /// <param name="context">Context of current simulation.</param>
        void Initialize(IEquationSystemAdapter adapter, ISimulationContext context);

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
        ///     called once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        void ApplyModelValues(ISimulationContext context);

        /// <summary>This method is called each time an equation is solved.</summary>
        /// <param name="context">Context of current simulation.</param>
        void OnEquationSolution(ISimulationContext context);

        /// <summary>
        ///     Notifies model class that DC bias for given timepoint is established (i.e after Newton-Raphson iterations
        ///     converged).
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        void OnDcBiasEstablished(ISimulationContext context);
    }
}