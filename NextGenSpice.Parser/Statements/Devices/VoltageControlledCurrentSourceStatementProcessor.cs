using NextGenSpice.Core.Devices;
using NextGenSpice.Parser.Statements.Deferring;

namespace NextGenSpice.Parser.Statements.Devices
{
    /// <summary>Class for processing voltage controlled voltage source SPICE statements.</summary>
    public class VoltageControlledCurrentSourceStatementProcessor : DeviceStatementProcessor
    {
        public VoltageControlledCurrentSourceStatementProcessor()
        {
            MinArgs = MaxArgs = 5;
        }

        /// <summary>Discriminator of the device type this processor can parse.</summary>
        public override char Discriminator => 'G';

        /// <summary>Processes given set of statements.</summary>
        protected override void DoProcess()
        {
            var name = DeviceName;
            var nodes = GetNodeIds(1, 4);
            var gain = GetValue(5);

            if (Errors == 0)
                Context.DeferredStatements.Add(new SimpleDeviceDeferredStatement(Context.CurrentScope, builder =>
                    builder.AddDevice(
                        nodes,
                        new Vccs(
                            gain,
                            name
                        )
                    )
                ));
        }
    }
}