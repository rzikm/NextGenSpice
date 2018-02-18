using NextGenSpice.Core.Elements;
using NextGenSpice.Parser.Statements.Deferring;

namespace NextGenSpice.Parser.Statements.Devices
{
    /// <summary>
    ///     Class that handles capacitor element statements.
    /// </summary>
    public class CapacitorStatementProcessor : ElementStatementProcessor
    {
        public CapacitorStatementProcessor()
        {
            MinArgs = 3;
            MaxArgs = 4;
        }

        /// <summary>
        ///     Discriminator of the element type this processor can parse.
        /// </summary>
        public override char Discriminator => 'C';

        /// <summary>
        ///     Processes given set of statements.
        /// </summary>
        protected override void DoProcess()
        {
            var name = ElementName;
            var nodes = GetNodeIndices(1, 2);
            var cvalue = GetValue(3);
            var ic = RawStatement.Length == 5 ? GetValue(4) : (double?) null;

            if (Errors == 0)
                Context.DeferredStatements.Add(new SimpleElementDeferredStatement(cb =>
                    cb.AddElement(nodes, new CapacitorElement(cvalue, ic, name))));
        }
    }
}