using System.Collections.Generic;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Devices.Parameters;
using NextGenSpice.Parser.Statements.Deferring;
using NextGenSpice.Parser.Statements.Models;

namespace NextGenSpice.Parser.Statements.Devices
{
	/// <summary>Class that handles diode device statements.</summary>
	public class DiodeStatementProcessor : DeviceStatementProcessor
	{
		public DiodeStatementProcessor()
		{
			MinArgs = MaxArgs = 3;
		}

		/// <summary>Discriminator of the device type this processor can parse.</summary>
		public override char Discriminator => 'D';

		/// <summary>Processes given set of statements.</summary>
		protected override void DoProcess()
		{
			var nodes = GetNodeIds(1, 2);

			// cannot check for model existence yet, defer checking for model later
			if (Errors != 0) return;

			var name = DeviceName; // capture
			var modelToken = RawStatement[3];
			Context.DeferredStatements.Add(
				new ModeledDeviceDeferedStatement<DiodeParams>(Context.CurrentScope,
					(par, cb) => cb.AddDevice(nodes, new Diode(par, name)), modelToken));
		}

		/// <summary>Gets list of model statement handlers that are responsible to parsing respective models of this device.</summary>
		/// <returns></returns>
		public override IEnumerable<IDeviceModelHandler> GetModelStatementHandlers()
		{
			return new IDeviceModelHandler[] {new DiodeModelHandler()};
		}

		/// <summary>Class that handles diode device model statements.</summary>
		private class DiodeModelHandler : DeviceModelHandlerBase<DiodeParams>,
			IDeviceModelHandler
		{
			public DiodeModelHandler()
			{
				Map(p => p.SaturationCurrent, "IS");
				Map(p => p.SeriesResistance, "RS");
				Map(p => p.EmissionCoefficient, "N");
				Map(p => p.TransitTime, "TT");
				Map(p => p.JunctionCapacitance, "CJO");
				Map(p => p.JunctionPotential, "VJ");
				Map(p => p.JunctionGradingCoefficient, "M");
				Map(p => p.ActivationEnergy, "EG");
				Map(p => p.SaturationCurrentTemperatureExponent, "XTI");
				Map(p => p.FlickerNoiseCoefficient, "KF");
				Map(p => p.FlickerNoiseExponent, "AF");
				Map(p => p.ForwardBiasDepletionCapacitanceCoefficient, "FC");
				Map(p => p.ReverseBreakdownVoltage, "BV");
				Map(p => p.ReverseBreakdownCurrent, "IBV");
				Map(p => p.NominalTemperature, "TNOM");
			}

			/// <summary>Discriminator of handled model type.</summary>
			public override string Discriminator => "D";

			/// <summary>Creates new instance of parameter class for this device model.</summary>
			/// <returns></returns>
			protected override DiodeParams CreateDefaultModel()
			{
				return new DiodeParams();
			}
		}
	}
}