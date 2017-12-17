using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Elements;

namespace NextGenSpice
{
    public class CurrentSourceStatementProcessor : InputSourceStatementProcessor
    {
        public override char Discriminator => 'I';
        protected override ElementStatement GetStatement(string name, int[] nodes, SourceBehaviorParams par)
        {
            return new SimpleElementStatement(builder => builder.AddElement(nodes, new CurrentSourceElement(par, name)));
        }
    }
}