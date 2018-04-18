using NextGenSpice.Core.Devices;
using NextGenSpice.Parser.Statements.Deferring;

namespace NextGenSpice.Parser.Statements.Devices
{
    /// <summary>Class that handles inductor device statements.</summary>
    public class InductorStatementProcessor : DeviceStatementProcessor
    {
        public InductorStatementProcessor()
        {
            MinArgs = 3;
            MaxArgs = 4;
        }

        /// <summary>Discriminator of the device type this processor can parse.</summary>
        public override char Discriminator => 'L';

        /// <summary>Processes given set of statements.</summary>
        protected override void DoProcess()
        {
            var name = DeviceName;
            var nodes = GetNodeIndices(1, 2);
            var lvalue = GetValue(3);
            var ic = RawStatement.Length == 5 ? GetValue(4) : (double?) null;


            if (Errors == 0)
                Context.DeferredStatements.Add(new SimpleDeviceDeferredStatement(cb =>
                    cb.AddDevice(nodes, new InductorDevice(lvalue, ic, name))));
        }
    }
}