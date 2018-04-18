using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Equations;
using NextGenSpice.Core.NumIntegration;
using NextGenSpice.LargeSignal.Stamping;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Large signal model for <see cref="CapacitorDevice" /> device.</summary>
    public class LargeSignalCapacitorModel : TwoTerminalLargeSignalDeviceModel<CapacitorDevice>
    {
        private int branchVariable;
        private LargeSignalCapacitorStamper stamper;

        public LargeSignalCapacitorModel(CapacitorDevice definitionDevice) : base(definitionDevice)
        {
        }

        /// <summary>Integration method used for modifying inner state of the device.</summary>
        private IIntegrationMethod IntegrationMethod { get; set; }

        /// <summary>Specifies how often the model should be updated.</summary>
        public override ModelUpdateMode UpdateMode => ModelUpdateMode.TimePoint;

        /// <summary>
        ///     Allows models to register additional vairables to the linear system equations. E.g. branch current variables.
        ///     And perform other necessary initialization
        /// </summary>
        /// <param name="builder">The equation system builder.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void Initialize(IEquationSystemBuilder builder, ISimulationContext context)
        {
            base.Initialize(builder, context);
            branchVariable = builder.AddVariable();
            stamper = new LargeSignalCapacitorStamper(Anode, Cathode, branchVariable);
            IntegrationMethod = context.CircuitParameters.IntegrationMethodFactory.CreateInstance();
        }

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
        ///     called once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            var (ieq, geq) = IntegrationMethod.GetEquivalents(DefinitionDevice.Capacity / context.TimeStep);
            stamper.Stamp(equations, ieq, geq);
        }

        /// <summary>Applies model values before first DC bias has been established for the first time.</summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyInitialCondition(IEquationEditor equations, ISimulationContext context)
        {
            stamper.StampInitialCondition(equations, DefinitionDevice.InitialVoltage);
        }

        /// <summary>
        ///     Notifies model class that DC bias for given timepoint is established. This method can be used for processing
        ///     circuit equation solution for current timepoint.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnDcBiasEstablished(ISimulationContext context)
        {
            base.OnDcBiasEstablished(context);
            Current = context.GetSolutionForVariable(branchVariable);

            var vc = context.GetSolutionForVariable(DefinitionDevice.Anode) -
                     context.GetSolutionForVariable(DefinitionDevice.Cathode);
            Voltage = vc;

            IntegrationMethod.SetState(Current, Voltage);
        }
    }
}