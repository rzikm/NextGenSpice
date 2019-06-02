using NextGenSpice.Core.Devices;
using NextGenSpice.LargeSignal.Stamping;
using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Devices
{
	/// <summary>Large signal model for <see cref="Resistor" /> device.</summary>
	public class LargeSignalResistor : TwoTerminalLargeSignalDevice<Resistor>
	{
		private readonly ConductanceStamper stamper;
		private readonly VoltageProxy voltage;

		public LargeSignalResistor(Resistor definitionDevice) : base(definitionDevice)
		{
			stamper = new ConductanceStamper();
			voltage = new VoltageProxy();
		}

		/// <summary>Resistance of the device in ohms.</summary>
		public double Resistance => DefinitionDevice.Resistance;

		/// <summary>Performs necessary initialization of the device, like mapping to the equation system.</summary>
		/// <param name="adapter">The equation system builder.</param>
		/// <param name="context">Context of current simulation.</param>
		public override void Initialize(IEquationSystemAdapter adapter, ISimulationContext context)
		{
			stamper.Register(adapter, Anode, Cathode);
			voltage.Register(adapter, Anode, Cathode);
		}

		/// <summary>
		///   Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
		///   called once every Newton-Raphson iteration.
		/// </summary>
		/// <param name="context">Context of current simulation.</param>
		public override void ApplyModelValues(ISimulationContext context)
		{
			stamper.Stamp(1 / Resistance);
		}

		/// <summary>This method is called each time an equation is solved.</summary>
		/// <param name="context">Context of current simulation.</param>
		public override void OnEquationSolution(ISimulationContext context)
		{
			Voltage = voltage.GetValue();
			Current = Voltage / Resistance;
		}
	}
}