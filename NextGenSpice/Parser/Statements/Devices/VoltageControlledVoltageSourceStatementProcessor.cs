using System.Linq;
using NextGenSpice.Core.Elements;
using NextGenSpice.Parser.Statements.Deferring;

namespace NextGenSpice.Parser.Statements.Devices
{
    /// <summary>
    ///     Class for processing voltage controlled voltage source SPICE statements.
    /// </summary>
    public class VoltageControlledVoltageSourceStatementProcessor : ElementStatementProcessor
    {
        /// <summary>
        ///     Discriminator of the element type this processor can parse.
        /// </summary>
        public override char Discriminator => 'E';

        /// <summary>
        ///     Processes given set of statements.
        /// </summary>
        /// <param name="tokens"></param>
        protected override void DoProcess(Token[] tokens)
        {
            if (tokens.Length != 6)
                InvalidNumberOfArguments(tokens[0]);

            var name = DeclareElement(tokens[0]);
            var nodes = GetNodeIndices(tokens, 1, 4);
            var gain = GetValue(tokens[5]); // TODO: unsafe! repair

            if (Errors == 0)
                Context.DeferredStatements.Add(new SimpleElementDeferredStatement(builder =>
                    builder.AddElement(
                        nodes,
                        new VoltageControlledVoltageSourceElement(
                            gain,
                            name
                        )
                    )
                ));
        }
    }
}