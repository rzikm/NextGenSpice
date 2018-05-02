using NextGenSpice.Core.Devices;
using NextGenSpice.Parser.Statements.Deferring;
using NextGenSpice.Parser.Utils;

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
            var nodes = GetNodeIds(1, 2);
            var lvalue = GetValue(3);
            var ic = GetInitialCondition();


            if (Errors == 0)
                CircuitBuilder.AddDevice(nodes, new Inductor(lvalue, ic, name));
        }

        private double? GetInitialCondition()
        {
            double? ic = null;
            if (RawStatement.Length == 5)
            {
                var t = RawStatement[4];
                if (!t.Value.StartsWith("IC="))
                {
                    Context.Errors.Add(t.ToError(SpiceParserErrorCode.InvalidParameter));
                }
                else
                {
                    t.LineColumn += 3;
                    t.Value = t.Value.Substring(3);
                    ic = GetValue(4);
                }
            }

            return ic;
        }
    }
}