﻿using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Devices;
using NextGenSpice.LargeSignal.Stamping;
using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Devices
{
	/// <summary>Large signal model for <see cref="VoltageSource" /> device.</summary>
	public class LargeSignalVoltageSource : TwoTerminalLargeSignalDevice<VoltageSource>
	{
		private readonly VoltageStamper stamper;

		public LargeSignalVoltageSource(VoltageSource definitionDevice) :
			base(definitionDevice)
		{
			stamper = new VoltageStamper();
			Behavior = definitionDevice.Behavior;
		}

		/// <summary>Strategy class specifying behavior of this source.</summary>
		private InputSourceBehavior Behavior { get; }

		/// <summary>Index of branch variable which holds current flowing through the voltage source.</summary>
		public int BranchVariable => stamper.BranchVariable;

		/// <summary>Allows devices to register any additional variables.</summary>
		/// <param name="adapter">The equation system builder.</param>
		public override void RegisterAdditionalVariables(IEquationSystemAdapter adapter)
		{
			base.RegisterAdditionalVariables(adapter);
			stamper.RegisterVariable(adapter);
		}

		/// <summary>Performs necessary initialization of the device, like mapping to the equation system.</summary>
		/// <param name="adapter">The equation system builder.</param>
		/// <param name="context">Context of current simulation.</param>
		public override void Initialize(IEquationSystemAdapter adapter, ISimulationContext context)
		{
			stamper.Register(adapter, Anode, Cathode);
		}

		/// <summary>
		///   Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
		///   called once every Newton-Raphson iteration.
		/// </summary>
		/// <param name="context">Context of current simulation.</param>
		public override void ApplyModelValues(ISimulationContext context)
		{
			Voltage = Behavior.GetValue(context.TimePoint);
			stamper.Stamp(Voltage);
		}

		/// <summary>This method is called each time an equation is solved.</summary>
		/// <param name="context">Context of current simulation.</param>
		public override void OnEquationSolution(ISimulationContext context)
		{
			Current = stamper.GetCurrent();
		}
	}
}