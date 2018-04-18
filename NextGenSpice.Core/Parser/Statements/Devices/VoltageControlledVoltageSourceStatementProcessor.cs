using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Parser.Statements.Deferring;

namespace NextGenSpice.Core.Parser.Statements.Devices
{
    /// <summary>Class for processing voltage controlled voltage source SPICE statements.</summary>
    public class VoltageControlledVoltageSourceStatementProcessor : DeviceStatementProcessor
    {
        public VoltageControlledVoltageSourceStatementProcessor()
        {
            MinArgs = MaxArgs = 5;
        }

        /// <summary>Discriminator of the device type this processor can parse.</summary>
        public override char Discriminator => 'E';

        /// <summary>Processes given set of statements.</summary>
        protected override void DoProcess()
        {
            var name = DeviceName;
            var nodes = GetNodeIndices(1, 4);
            var gain = GetValue(5);

            if (Errors == 0)
                Context.DeferredStatements.Add(new SimpleDeviceDeferredStatement(builder =>
                    builder.AddDevice(
                        nodes,
                        new VoltageControlledVoltageSourceDevice(
                            gain,
                            name
                        )
                    )
                ));
        }
    }

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
            var nodes = GetNodeIndices(1, 4);
            var gain = GetValue(5);

            if (Errors == 0)
                Context.DeferredStatements.Add(new SimpleDeviceDeferredStatement(builder =>
                    builder.AddDevice(
                        nodes,
                        new VoltageControlledCurrentSourceDevice(
                            gain,
                            name
                        )
                    )
                ));
        }
    }
}