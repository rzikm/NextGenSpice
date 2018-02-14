using System.Collections.Generic;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>
    ///     Base class for large signal device models.
    /// </summary>
    /// <typeparam name="TDefinitionElement">Class used for the element in the circuit definition that this class is model for.</typeparam>
    public abstract class LargeSignalModelBase<TDefinitionElement> : ILargeSignalDeviceModel
        where TDefinitionElement : ICircuitDefinitionElement
    {
        protected LargeSignalModelBase(TDefinitionElement definitionElement)
        {
            DefinitionElement = definitionElement;
        }

        /// <summary>
        ///     Parent definition element that this model instance corresponds to.
        /// </summary>
        protected TDefinitionElement DefinitionElement { get; }

        /// <summary>
        ///     Specifies how often the model should be updated.
        /// </summary>
        public abstract ModelUpdateMode UpdateMode { get; }

        /// <summary>
        ///     Name identifier of the corresponding element.
        /// </summary>
        public string Name => DefinitionElement.Name;

        /// <summary>
        ///     Allows models to register additional vairables to the linear system equations. E.g. branch current variables.
        /// </summary>
        /// <param name="builder">The equation system builder.</param>
        /// <param name="context">Context of current simulation.</param>
        public virtual void RegisterAdditionalVariables(IEquationSystemBuilder builder, ISimulationContext context)
        {
        }

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is called
        ///     once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        public abstract void ApplyModelValues(IEquationEditor equations, ISimulationContext context);

        /// <summary>
        ///     Applies model values before first DC bias has been established for the first time.
        /// </summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        public virtual void ApplyInitialCondition(IEquationEditor equations, ISimulationContext context)
        {
            ApplyModelValues(equations, context);
        }

        /// <summary>
        ///     Notifies model class that DC bias for given timepoint is established. This method can be used for processing
        ///     circuit equation solution
        ///     for current timepoint.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public virtual void OnDcBiasEstablished(ISimulationContext context)
        {
        }

        /// <summary>
        ///     Gets provider instance for specified attribute value or null if no provider for requested parameter exists. For
        ///     example "I" for the current flowing throught the two
        ///     terminal element.
        /// </summary>
        /// <returns>IPrintValueProvider for specified attribute.</returns>
        public abstract IEnumerable<IDeviceStatsProvider> GetDeviceStatsProviders();
    }
}