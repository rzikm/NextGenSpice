﻿using NextGenSpice.Core.Devices;
using NextGenSpice.LargeSignal.NumIntegration;
using NextGenSpice.LargeSignal.Stamping;
using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Large signal model for <see cref="CapacitorDevice" /> device.</summary>
    public class LargeSignalCapacitor : TwoTerminalLargeSignalDevice<CapacitorDevice>
    {
        private readonly CapacitorStamper stamper;
        private readonly VoltageProxy voltage;

        private bool firtDcPoint;

        public LargeSignalCapacitor(CapacitorDevice definitionDevice) : base(definitionDevice)
        {
            voltage = new VoltageProxy();
            stamper = new CapacitorStamper();

        }

        /// <summary>Integration method used for modifying inner state of the device.</summary>
        private IIntegrationMethod IntegrationMethod { get; set; }

        /// <summary>Performs necessary initialization of the device, like mapping to the equation system.</summary>
        /// <param name="adapter">The equation system builder.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void Initialize(IEquationSystemAdapter adapter, ISimulationContext context)
        {
            stamper.Register(adapter, Anode, Cathode);
            voltage.Register(adapter, Anode, Cathode);
            firtDcPoint = true;
            IntegrationMethod = context.CircuitParameters.IntegrationMethodFactory.CreateInstance();
        }

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
        ///     called once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(ISimulationContext context)
        {
            var (ieq, geq) = IntegrationMethod.GetEquivalents(DefinitionDevice.Capacity / context.TimeStep);
            stamper.Stamp(ieq, geq);
        }

        /// <summary>Applies model values before first DC bias has been established for the first time.</summary>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyInitialCondition(ISimulationContext context)
        {
            if (DefinitionDevice.InitialVoltage.HasValue)
                stamper.Stamp(DefinitionDevice.InitialVoltage.Value, 1);
            else
                stamper.Stamp(0, 0); // open circuit
        }

        /// <summary>This method is called each time an equation is solved.</summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnEquationSolution(ISimulationContext context)
        {
        }

        /// <summary>
        ///     Notifies model class that DC bias for given timepoint is established (i.e after Newton-Raphson iterations
        ///     converged).
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnDcBiasEstablished(ISimulationContext context)
        {
            base.OnDcBiasEstablished(context);
            Current = stamper.GetCurrent();
            Voltage = voltage.GetValue();
            IntegrationMethod.SetState(Current, Voltage);
        }
    }
}