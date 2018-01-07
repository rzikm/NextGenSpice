using NextGenSpice.Core.Elements;
using NextGenSpice.Parser.Statements.Deferring;

namespace NextGenSpice.Parser.Statements.Devices
{
    /// <summary>
    /// Class responsible for handling spice resistor statements.
    /// </summary>
    public class ResistorStatementProcessor : ElementStatementProcessor
    {
        /// <summary>
        /// Discriminator of the element type this processor can parse.
        /// </summary>
        public override char Discriminator => 'R';

        /// <summary>
        /// Processes given set of statements.
        /// </summary>
        /// <param name="tokens"></param>
        protected override void DoProcess(Token[] tokens)
        {
            if (tokens.Length != 4)
            {
                InvalidNumberOfArguments(tokens[0]);
            }

            var name = DeclareElement(tokens[0]);
            var nodes = GetNodeIndices(tokens, 1, 2);
            var rvalue = GetValue(tokens[3]);

            if (Errors == 0)
                Context.DeferredStatements.Add(new SimpleElementStatement(builder => builder.AddElement(nodes, new ResistorElement(rvalue, name)))); 
        }
    }
}