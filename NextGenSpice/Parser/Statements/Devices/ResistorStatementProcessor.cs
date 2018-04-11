using NextGenSpice.Core.Elements;
using NextGenSpice.Parser.Statements.Deferring;

namespace NextGenSpice.Parser.Statements.Devices
{
    /// <summary>Class responsible for handling spice resistor statements.</summary>
    public class ResistorStatementProcessor : ElementStatementProcessor
    {
        public ResistorStatementProcessor()
        {
            MinArgs = MaxArgs = 3;
        }

        /// <summary>Discriminator of the element type this processor can parse.</summary>
        public override char Discriminator => 'R';

        /// <summary>Processes given set of statements.</summary>
        protected override void DoProcess()
        {
            var name = ElementName;
            var nodes = GetNodeIndices(1, 2);
            var rvalue = GetValue(3);

            if (Errors == 0)
                Context.DeferredStatements.Add(new SimpleElementDeferredStatement(builder =>
                    builder.AddElement(nodes, new ResistorElement(rvalue, name))));
        }
    }
}