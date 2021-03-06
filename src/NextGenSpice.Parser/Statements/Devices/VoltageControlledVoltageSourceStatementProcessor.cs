﻿using NextGenSpice.Core.Devices;

namespace NextGenSpice.Parser.Statements.Devices
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
			var nodes = GetNodeIds(1, 4);
			var gain = GetValue(5);

			if (Errors == 0)
				CircuitBuilder.AddDevice(nodes, new Vcvs(gain, name));
		}
	}
}