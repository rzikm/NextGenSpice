using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Devices;
using NextGenSpice.Parser.Statements.Deferring;

namespace NextGenSpice.Parser.Statements.Devices
{
    /// <summary>Class for handling independent current source statement ///</summary>
    public class VoltageSourceStatementProcessor : InputSourceStatementProcessor
    {
        /// <summary>Discriminator of the device type this processor can parse.</summary>
        public override char Discriminator => 'V';

        /// <summary>Factory method for a deferred statement that should be processed later.</summary>
        /// <param name="par"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override ICircuitDefinitionDevice GetDevice(InputSourceBehavior par, string name)
        {
            return new VoltageSource(par, name);
        }
    }
}