using NextGenSpice.Core.Devices;
using NextGenSpice.Parser.Statements.Deferring;

namespace NextGenSpice.Parser.Statements.Devices
{
    /// <summary>Class for processing current controlled voltage source SPICE statements.</summary>
    public class CurrentControlledCurrentSourceStatementProcessor : DeviceStatementProcessor
    {
        public CurrentControlledCurrentSourceStatementProcessor()
        {
            MinArgs = MaxArgs = 4;
        }

        /// <summary>Discriminator of the device type this processor can parse.</summary>
        public override char Discriminator => 'F';

        /// <summary>Processes given set of statements.</summary>
        protected override void DoProcess()
        {
            var name = DeviceName;
            var nodes = GetNodeIndices(1, 2);
            var vsource = RawStatement[3];
            var gain = GetValue(4);

            if (Errors == 0)
                Context.DeferredStatements.Add(new VoltageSourceDependentDeferredStatement(Context.CurrentScope, vsource, (builder, vs) =>
                    builder.AddDevice(
                        nodes,
                        new Cccs(
                            vs,
                            gain,
                            name
                        )
                    )
                ));
        }
    }
}