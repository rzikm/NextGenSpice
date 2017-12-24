using System.Collections.Generic;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Extensions;

namespace NextGenSpice
{
    public class ResistorStatementProcessor : ElementStatementProcessor
    {
        public override char Discriminator => 'R';

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