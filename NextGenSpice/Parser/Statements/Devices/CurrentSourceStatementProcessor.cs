using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Elements;
using NextGenSpice.Parser.Statements.Deferring;

namespace NextGenSpice.Parser.Statements.Devices
{
    /// <summary>
    ///     Class for handling independent current source statement
    /// </summary>
    public class CurrentSourceStatementProcessor : InputSourceStatementProcessor
    {
        /// <summary>
        ///     Discriminator of the element type this processor can parse.
        /// </summary>
        public override char Discriminator => 'I';

        /// <summary>
        ///     Factory method for a deferred statement that should be processed later.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nodes"></param>
        /// <param name="par"></param>
        /// <returns></returns>
        protected override DeferredStatement GetStatement(string name, int[] nodes, SourceBehaviorParams par)
        {
            return new SimpleElementDeferredStatement(builder =>
                builder.AddElement(nodes, new CurrentSourceElement(par, name)));
        }
    }
}