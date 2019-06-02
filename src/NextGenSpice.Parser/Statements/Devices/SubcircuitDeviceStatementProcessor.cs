using NextGenSpice.Parser.Statements.Deferring;

namespace NextGenSpice.Parser.Statements.Devices
{
    /// <summary>Class for processing SPICE statements calling a certain subcircuit.</summary>
    public class SubcircuitDeviceStatementProcessor : DeviceStatementProcessor
    {
        public SubcircuitDeviceStatementProcessor()
        {
            MinArgs = 2;
        }

        /// <summary>Discriminator of the device type this processor can parse.</summary>
        public override char Discriminator => 'X';

        /// <summary>Processes given set of statements.</summary>
        protected override void DoProcess()
        {
            var name = DeviceName;
            var nodes = GetNodeIds(1, RawStatement.Length - 2);
            var subcircuitName = RawStatement[RawStatement.Length - 1];

            if (Errors == 0)
                Context.DeferredStatements.Add(new SubcircuitDeviceDeferredStatement(Context.CurrentScope, name, nodes, subcircuitName));
        }
    }
}