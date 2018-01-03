using System.Collections.Generic;
using NextGenSpice.Core.Elements;

namespace NextGenSpice
{
    public class DiodeStatementProcessor : ElementStatementProcessor
    {
        class DiodeModelStatementHandler : IModelStatementHandler
        {
            public string Discriminator => "D";
            public void Process(Token[] tokens, ParsingContext context)
            {
                var name = tokens[2].Value;

                context.SymbolTable.Models[ModelType.Diode][name] = new DiodeModelParams();
            }
        }

        public override char Discriminator => 'D';
        protected override void DoProcess(Token[] tokens)
        {
            if (tokens.Length != 4)
            {
                InvalidNumberOfArguments(tokens[0]);
            }

            var name = DeclareElement(tokens[0]);
            var nodes = GetNodeIndices(tokens, 1, 2);


            if (Errors == 0)
            {
                var modelToken = tokens[3];
                Context.DeferredStatements.Add(
                    new ModeledElementStatement<DiodeModelParams>(
                        (par, cb) => cb.AddElement(nodes, new DiodeElement(par, name)),
                        () => (DiodeModelParams)Context.SymbolTable.Models[ModelType.Diode].GetValueOrDefault(modelToken.Value),
                        modelToken));
            }
        }

        public override IEnumerable<IModelStatementHandler> GetModelStatementHandlers()
        {
            return new IModelStatementHandler[] { new DiodeModelStatementHandler() };
        }
    }
}