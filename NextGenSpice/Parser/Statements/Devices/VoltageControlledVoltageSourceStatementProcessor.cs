using System.Linq;
using NextGenSpice.Core.Elements;
using NextGenSpice.Parser.Statements.Deferring;

namespace NextGenSpice.Parser.Statements.Devices
{
    /// <summary>
    ///     Class for processing voltage controlled voltage source SPICE statements.
    /// </summary>
    public class VoltageControlledVoltageSourceStatementProcessor : ElementStatementProcessor
    {
        public VoltageControlledVoltageSourceStatementProcessor()
        {
            MinArgs = MaxArgs = 5;
        }
        /// <summary>
        ///     Discriminator of the element type this processor can parse.
        /// </summary>
        public override char Discriminator => 'E';

        /// <summary>
        ///     Processes given set of statements.
        /// </summary>
        protected override void DoProcess()
        {
            var name = ElementName;
            var nodes = GetNodeIndices(1, 4);
            var gain = GetValue(5);

            if (Errors == 0)
                Context.DeferredStatements.Add(new SimpleElementDeferredStatement(builder =>
                    builder.AddElement(
                        nodes,
                        new VoltageControlledVoltageSourceElement(
                            gain,
                            name
                        )
                    )
                ));
        }
    }

    /// <summary>
    ///     Class for processing voltage controlled voltage source SPICE statements.
    /// </summary>
    public class VoltageControlledCurrentSourceStatementProcessor : ElementStatementProcessor
    {
        public VoltageControlledCurrentSourceStatementProcessor()
        {
            MinArgs = MaxArgs = 5;
        }
        /// <summary>
        ///     Discriminator of the element type this processor can parse.
        /// </summary>
        public override char Discriminator => 'G';

        /// <summary>
        ///     Processes given set of statements.
        /// </summary>
        protected override void DoProcess()
        {
            var name = ElementName;
            var nodes = GetNodeIndices(1, 4);
            var gain = GetValue(5);

            if (Errors == 0)
                Context.DeferredStatements.Add(new SimpleElementDeferredStatement(builder =>
                    builder.AddElement(
                        nodes,
                        new VoltageControlledCurrentSourceElement(
                            gain,
                            name
                        )
                    )
                ));
        }
    }
}