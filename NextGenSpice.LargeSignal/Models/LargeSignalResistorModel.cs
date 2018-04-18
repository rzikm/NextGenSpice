using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Large signal model for <see cref="ResistorDevice" /> device.</summary>
    public class LargeSignalResistorModel : TwoNodeLargeSignalModel<ResistorDevice>
    {
        public LargeSignalResistorModel(ResistorDevice definitionDevice) : base(definitionDevice)
        {
        }

        /// <summary>Specifies how often the model should be updated.</summary>
        public override ModelUpdateMode UpdateMode => ModelUpdateMode.NoUpdate;

        /// <summary>Resistance of the device in ohms.</summary>
        public double Resistance => DefinitionDevice.Resistance;

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
        ///     called once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            equations.AddConductance(Anode, Cathode, 1 / Resistance);
        }

        /// <summary>
        ///     Notifies model class that DC bias for given timepoint is established. This method can be used for processing
        ///     circuit equation solution for current timepoint.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnDcBiasEstablished(ISimulationContext context)
        {
            base.OnDcBiasEstablished(context);
            Voltage = context.GetSolutionForVariable(Anode) - context.GetSolutionForVariable(Cathode);
            Current = Voltage / Resistance;
        }
    }
}