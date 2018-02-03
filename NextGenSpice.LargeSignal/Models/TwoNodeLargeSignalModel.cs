using NextGenSpice.Core.Elements;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>
    ///     Base class for large signal device models that have exactly two terminals.
    /// </summary>
    /// <typeparam name="TDefinitionElement">Class used for the element in the circuit definition that this class is model for.</typeparam>
    public abstract class TwoNodeLargeSignalModel<TDefinitionElement> : LargeSignalModelBase<TDefinitionElement>,
        ITwoTerminalLargeSignalDeviceModel
        where TDefinitionElement : TwoNodeCircuitElement
    {
        protected TwoNodeLargeSignalModel(TDefinitionElement definitionElement) : base(definitionElement)
        {
        }

        /// <summary>
        ///     Id of node connected to positive terminal of this device.
        /// </summary>
        public int Anode => DefinitionElement.ConnectedNodes[0];

        /// <summary>
        ///     Id of node connected to negative terminal of this device.
        /// </summary>
        public int Kathode => DefinitionElement.ConnectedNodes[1];

        /// <summary>
        ///     Current flowing from positive terminal to negative terminal through the device.
        /// </summary>
        public double Current { get; protected set; }

        /// <summary>
        ///     Voltage across this device, difference of potential between positive and negative terminals.
        /// </summary>
        public double Voltage { get; protected set; }
    }
}