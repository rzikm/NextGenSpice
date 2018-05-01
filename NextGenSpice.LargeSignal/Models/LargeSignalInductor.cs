using NextGenSpice.Core.Devices;
using NextGenSpice.LargeSignal.NumIntegration;
using NextGenSpice.LargeSignal.Stamping;
using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Large signal model for <see cref="InductorDevice" /> device.</summary>
    public class LargeSignalInductor : TwoTerminalLargeSignalDevice<InductorDevice>
    {
        private readonly InductorStamper stamper;
        private readonly VoltageProxy voltage;

        private bool firstDcPoint;

        public LargeSignalInductor(InductorDevice definitionDevice) : base(definitionDevice)
        {
            voltage = new VoltageProxy();
            stamper = new InductorStamper();
        }

        /// <summary>Integration method used for modifying inner state of the device.</summary>
        private IIntegrationMethod IntegrationMethod { get; set; }


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
            voltage.Register(adapter, Anode, Cathode);
            firstDcPoint = true;
            IntegrationMethod = context.CircuitParameters.IntegrationMethodFactory.CreateInstance();
        }

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
        ///     called once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(ISimulationContext context)
        {
            if (firstDcPoint) // initial dc bias
            {
                stamper.StampInitialCondition(DefinitionDevice.InitialCurrent);
            }
            else
            {
                var (veq, req) = IntegrationMethod.GetEquivalents(DefinitionDevice.Inductance / context.TimeStep);
                stamper.Stamp(-veq, req);
            }
        }

        /// <summary>This method is called each time an equation is solved.</summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnEquationSolution(ISimulationContext context)
        {
            Current = stamper.GetCurrent();
            Voltage = voltage.GetValue();
        }

        /// <summary>
        ///     Notifies model class that DC bias for given timepoint is established (i.e after Newton-Raphson iterations
        ///     converged).
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnDcBiasEstablished(ISimulationContext context)
        {
            base.OnDcBiasEstablished(context);
            IntegrationMethod.SetState(Voltage, Current);
            firstDcPoint = false;
        }
    }
}