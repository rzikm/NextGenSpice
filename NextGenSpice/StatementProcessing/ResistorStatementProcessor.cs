using System.Collections.Generic;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Extensions;

namespace NextGenSpice
{
    public class ResistorStatementProcessor : ElementStatementProcessor
    {
        public override char Discriminator => 'R';

        protected override ElementStatement DoProcess(Token[] tokens, List<ErrorInfo> errors)
        {
            if (tokens.Length != 4)
            {
                errors.Add(InvalidNumberOfArguments(tokens[0]));
                return null;
            }

            var name = DeclareElement(tokens[0], errors);
            var nodes = GetNodeIndices(tokens, 1, 2, errors);
            var rvalue = GetValue(tokens[3], errors);

            return errors.Count > 0 ? null : new SimpleElementStatement(builder => builder.AddElement(nodes, new ResistorElement(rvalue, name)));
        }
    }
}