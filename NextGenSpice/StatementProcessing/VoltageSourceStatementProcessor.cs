using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Elements;

namespace NextGenSpice
{
    public class VoltageSourceStatementProcessor : InputSourceStatementProcessor
    {
        public override char Discriminator => 'V';
        protected override DeferredStatement GetStatement(string name, int[] nodes, SourceBehaviorParams par)
        {
            return new SimpleElementStatement(builder => builder.AddElement(nodes, new VoltageSourceElement(par, name)));
        }
    }
}