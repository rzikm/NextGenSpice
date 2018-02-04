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
        ///     Set of model parameters for this device.
        /// </summary>
        public BjtModelParams Parameters { get; }
    }
}