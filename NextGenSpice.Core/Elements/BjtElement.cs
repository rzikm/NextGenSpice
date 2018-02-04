using NextGenSpice.Core.Elements.Parameters;

namespace NextGenSpice.Core.Elements
{
    /// <summary>
    ///     Class that represents Homo-Junction Bipolar Transistor device.
    /// </summary>
    public class BjtElement : CircuitDefinitionElement
    {
        public BjtElement(BjtModelParams parameters, string name = null) : base(4, name)
        {
            Parameters = parameters;
        }

        /// <summary>
        ///     Node connected to collector terminal of the transistor.
        /// </summary>
        public int Collector => ConnectedNodes[0];

        /// <summary>
        ///     Node connected to base terminal of the transistor.
        /// </summary>
        public int Base => ConnectedNodes[1];

        /// <summary>
        ///     Node connected to emitter terminal of the transistor.
        /// </summary>
        public int Emitter => ConnectedNodes[2];

        /// <summary>
        ///     Node connected to substrate terminal of the transistor.
        /// </summary>
        public int Substrate => ConnectedNodes[3];

        /// <summary>
        ///     Set of model parameters for this device.
        /// </summary>
        public BjtModelParams Parameters { get; }
    }
}