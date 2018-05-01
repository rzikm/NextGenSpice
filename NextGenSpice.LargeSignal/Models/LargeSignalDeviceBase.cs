using System.Collections.Generic;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Representation;
using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Base class for large signal device models.</summary>
    /// <typeparam name="TDefinitionDevice">Class used for the device in the circuit definition that this class is model for.</typeparam>
    public abstract class LargeSignalDeviceBase<TDefinitionDevice> : ILargeSignalDevice
        where TDefinitionDevice : ICircuitDefinitionDevice
    {
        protected LargeSignalDeviceBase(TDefinitionDevice definitionDevice)
        {
            DefinitionDevice = definitionDevice;
        }

        /// <summary>Parent definition device that this model instance corresponds to.</summary>
        public TDefinitionDevice DefinitionDevice { get; }

        ICircuitDefinitionDevice IAnalysisDeviceModel<LargeSignalCircuitModel>.DefinitionDevice => DefinitionDevice;

        /// <summary>Allows devices to register any additional variables.</summary>
        /// <param name="adapter">The equation system builder.</param>
        public virtual void RegisterAdditionalVariables(IEquationSystemAdapter adapter)
        {  // no registration default
        }

        /// <summary>Performs necessary initialization of the device, like mapping to the equation system.</summary>
        /// <param name="adapter">The equation system builder.</param>
        /// <param name="context">Context of current simulation.</param>
        public abstract void Initialize(IEquationSystemAdapter adapter, ISimulationContext context);

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
        ///     called once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public abstract void ApplyModelValues(ISimulationContext context);

        /// <summary>This method is called each time an equation is solved.</summary>
        /// <param name="context">Context of current simulation.</param>
        public abstract void OnEquationSolution(ISimulationContext context);

        /// <summary>
        ///     Notifies model class that DC bias for given timepoint is established (i.e after Newton-Raphson iterations
        ///     converged).
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public virtual void OnDcBiasEstablished(ISimulationContext context)
        {
        }

        /// <summary>
        ///     Gets provider instance for specified attribute value or null if no provider for requested parameter exists.
        ///     For example "I" for the current flowing throught the two terminal device.
        /// </summary>
        /// <returns>IPrintValueProvider for specified attribute.</returns>
        public abstract IEnumerable<IDeviceStatsProvider> GetDeviceStatsProviders();
    }
}