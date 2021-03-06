﻿using NextGenSpice.Core.Devices;

namespace NextGenSpice.Parser.Statements.Devices
{
	/// <summary>Class responsible for handling spice resistor statements.</summary>
	public class ResistorStatementProcessor : DeviceStatementProcessor
	{
		public ResistorStatementProcessor()
		{
			MinArgs = MaxArgs = 3;
		}

		/// <summary>Discriminator of the device type this processor can parse.</summary>
		public override char Discriminator => 'R';

		/// <summary>Processes given set of statements.</summary>
		protected override void DoProcess()
		{
			var name = DeviceName;
			var nodes = GetNodeIds(1, 2);
			var rvalue = GetValue(3);

			if (Errors == 0)
				CircuitBuilder.AddDevice(nodes, new Resistor(rvalue, name));
		}
	}
}