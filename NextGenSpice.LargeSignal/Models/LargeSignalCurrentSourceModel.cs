using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.LargeSignal.Behaviors;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>
    ///     Large signal model for <see cref="CurrentSourceElement" /> device.
    /// </summary>
    public class LargeSignalCurrentSourceModel : TwoNodeLargeSignalModel<CurrentSourceElement>
    {
        public LargeSignalCurrentSourceModel(CurrentSourceElement definitionElement, IInputSourceBehavior behavior) :
            base(definitionElement)
        {
            Behavior = behavior;
        }

        /// <summary>
        ///     Strategy class specifying behavior of this source.
        /// </summary>
        public IInputSourceBehavior Behavior { get; }

        /// <summary>
        ///     Specifies how often the model should be updated.
        /// </summary>
        public override ModelUpdateMode UpdateMode => Behavior.UpdateMode;

        /// <summary>
        ///     Notifies model class that DC bias for given timepoint is established. This method can be used for processing
        ///     circuit equation solution
        ///     for current timepoint.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnDcBiasEstablished(ISimulationContext context)
        {
            base.OnDcBiasEstablished(context);
            Voltage = context.GetSolutionForVariable(Anode) - context.GetSolutionForVariable(Cathode);
        }

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is called
        ///     once every Newton-Raphson iteration.
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