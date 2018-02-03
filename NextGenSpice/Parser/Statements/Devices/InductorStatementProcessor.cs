using NextGenSpice.Core.Elements;
using NextGenSpice.Parser.Statements.Deferring;

namespace NextGenSpice.Parser.Statements.Devices
{
    /// <summary>
    ///     Class that handles inductor element statements.
    /// </summary>
    public class InductorStatementProcessor : ElementStatementProcessor
    {
        /// <summary>
        ///     Discriminator of the element type this processor can parse.
        /// </summary>
        public override char Discriminator => 'L';

        /// <summary>
        ///     Processes given set of statements.
        /// </summary>
        /// <param name="tokens"></param>
        protected override void DoProcess(Token[] tokens)
        {
            if (tokens.Length != 4) // name, +N, -N, lvalue, <IC>
                InvalidNumberOfArguments(tokens[0]);

            var name = DeclareElement(tokens[0]);
            var nodes = GetNodeIndices(tokens, 1, 2);
            var lvalue = GetValue(tokens[3]);
            var ic = tokens.Length == 5 ? GetValue(tokens[4]) : (double?) null;


            if (Errors == 0)
                Context.DeferredStatements.Add(new SimpleElementStatement(cb =>
                    cb.AddElement(nodes, new InductorElement(lvalue, ic, name))));
        }
    }
}