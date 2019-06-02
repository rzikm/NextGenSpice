using NextGenSpice.Core.Devices;
using NextGenSpice.LargeSignal.Stamping;
using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Devices
{
	/// <summary>Large signal model for <see cref="Cccs" /> device.</summary>
	public class LargeSignalCccs : TwoTerminalLargeSignalDevice<Cccs>
	{
		private readonly LargeSignalVoltageSource ampermeter;
		private readonly CccsStamper stamper;
		private readonly VoltageProxy voltage;

		public LargeSignalCccs(Cccs definitionDevice, LargeSignalVoltageSource ampermeterDevice) : base(definitionDevice)
		{
			voltage = new VoltageProxy();
			stamper = new CccsStamper();
			ampermeter = ampermeterDevice;
		}

		public double ReferenceCurrent => ampermeter.Current;

		/// <summary>Performs necessary initialization of the device, like mapping to the equation system.</summary>
		/// <param name="adapter">The equation system builder.</param>
		/// <param name="context">Context of current simulation.</param>
		public override void Initialize(IEquationSystemAdapter adapter, ISimulationContext context)
		{
			stamper.Register(adapter, Anode, Cathode, ampermeter.BranchVariable);
			voltage.Register(adapter, Anode, Cathode);
		}

		/// <summary>
		///   Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
		///   called once every Newton-Raphson iteration.
		/// </summary>
		/// <param name="context">Context of current simulation.</param>
		public override void ApplyModelValues(ISimulationContext context)
		{
			stamper.Stamp(DefinitionDevice.Gain);
		}

		/// <summary>This method is called each time an equation is solved.</summary>
		/// <param name="context">Context of current simulation.</param>
		public override void OnEquationSolution(ISimulationContext context)
		{
			Voltage = voltage.GetValue();
			Current = ReferenceCurrent * DefinitionDevice.Gain;
		}
	}
}