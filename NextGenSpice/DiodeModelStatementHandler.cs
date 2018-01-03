using NextGenSpice.Core.Elements;

namespace NextGenSpice
{
    public class DiodeModelStatementHandler : IModelStatementHandler
    {
        public string Discriminator => "D";
        public void Process(Token[] tokens, ParsingContext context)
        {
            var name = tokens[2].Value;

            context.SymbolTable.Models[ModelType.Diode][name] = new DiodeModelParams();
        }
    }
}