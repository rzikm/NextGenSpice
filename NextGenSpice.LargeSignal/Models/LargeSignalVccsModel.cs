using System.Collections.Generic;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>
    ///     Large signal model for <see cref="VoltageControlledVoltageSourceElement"/> element.
    /// </summary>
    public class LargeSignalVccsModel : LargeSignalModelBase<VoltageControlledCurrentSourceElement>, ITwoTerminalLargeSignalDeviceModel
    {
        public LargeSignalVccsModel(VoltageControlledCurrentSourceElement definitionElement) : base(definitionElement)
        {
        }
        
        /// <summary>
        ///     Specifies how often the model should be updated.
        /// </summary>
        public override ModelUpdateMode UpdateMode => ModelUpdateMode.Always; // due to possible dependencies on nonlinear elements.

        public double Voltage { get; private set; }

        public double Current { get; private set; }

        /// <summary>
        ///     Id of node connected to positive terminal of this device.
        /// </summary>
        public int Anode => DefinitionElement.ConnectedNodes[0];

        /// <summary>
        ///     Id of node connected to negative terminal of this device.
        /// </summary>
        public int Cathode => DefinitionElement.ConnectedNodes[1];

        /// <summary>
        ///     Positive terminal of the reference voltage.
        /// </summary>
        public int ReferenceAnode => DefinitionElement.ConnectedNodes[2];

        /// <summary>
        ///     Negative terminal of the reference voltage.
        /// </summary>
        public int ReferenceCathode => DefinitionElement.ConnectedNodes[3];

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is called
        ///     once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            equations.AddMatrixEntry(Anode, ReferenceAnode, DefinitionElement.TransConductance);
            equations.AddMatrixEntry(Cathode, ReferenceAnode, -DefinitionElement.TransConductance);

            equations.AddMatrixEntry(Anode, ReferenceCathode, -DefinitionElement.TransConductance);
            equations.AddMatrixEntry(Cathode, ReferenceCathode, DefinitionElement.TransConductance);
        }

        /// <summary>
        ///     Gets provider instance for specified attribute value or null if no provider for requested parameter exists. For
        ///     example "I" for the current flowing throught the two
        ///     terminal element.
        /// </summary>
        /// <returns>IPrintValueProvider for specified attribute.</returns>
        public override IEnumerable<IDeviceStatsProvider> GetDeviceStatsProviders()
        {
            return new[]
            {
                new SimpleDeviceStatsProvider("I", () => Current),
                new SimpleDeviceStatsProvider("V", () => Voltage)
            };
        }

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
            Current = (context.GetSolutionForVariable(ReferenceAnode) - context.GetSolutionForVariable(ReferenceCathode)) * DefinitionElement.TransConductance;
        }
    }
}