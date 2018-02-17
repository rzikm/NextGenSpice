using System;
using NextGenSpice.Parser.Statements.Deferring;

namespace NextGenSpice.Parser.Statements.Devices
{
    /// <summary>
    ///     Class for processing SPICE statements calling a certain subcircuit.
    /// </summary>
    public class SubcircuitElementStatementProcessor : ElementStatementProcessor
    {
        /// <summary>
        ///     Discriminator of the element type this processor can parse.
        /// </summary>
        public override char Discriminator => 'X';

        /// <summary>
        ///     Processes given set of statements.
        /// </summary>
        /// <param name="tokens"></param>
        protected override void DoProcess(Token[] tokens)
        {
            if (tokens.Length < 3) // XName Node SubcircuitName
                InvalidNumberOfArguments(tokens[0]);

            var name = DeclareElement(tokens[0]);
            var nodes = GetNodeIndices(tokens, 1, tokens.Length - 2);
            var subcircuitName = tokens[tokens.Length - 1];

            if (Errors == 0)
                Context.DeferredStatements.Add(new SubcircuitElementDeferredStatement(name, nodes, subcircuitName));
        }
    }
}