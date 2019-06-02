using NextGenSpice.Core.Devices;
using NextGenSpice.Parser.Utils;

namespace NextGenSpice.Parser.Statements.Devices
{
	/// <summary>Class that handles capacitor device statements.</summary>
	public class CapacitorStatementProcessor : DeviceStatementProcessor
	{
		public CapacitorStatementProcessor()
		{
			MinArgs = 3;
			MaxArgs = 4;
		}

		/// <summary>Discriminator of the device type this processor can parse.</summary>
		public override char Discriminator => 'C';

		/// <summary>Processes given set of statements.</summary>
		protected override void DoProcess()
		{
			var name = DeviceName;
			var nodes = GetNodeIds(1, 2);
			var cvalue = GetValue(3);
			var ic = GetInitialCondition();

			if (Errors == 0)
				CircuitBuilder.AddDevice(nodes, new Capacitor(cvalue, ic, name));
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