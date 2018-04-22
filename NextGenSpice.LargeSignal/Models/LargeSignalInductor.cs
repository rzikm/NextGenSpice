using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Devices;
using NextGenSpice.LargeSignal.NumIntegration;
using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Large signal model for <see cref="InductorDevice" /> device.</summary>
    public class LargeSignalInductor : TwoTerminalLargeSignalDevice<InductorDevice>
    {
        private int branchVariable;
        private LargeSignalInductorStamper stamper;

        public LargeSignalInductor(InductorDevice definitionDevice) : base(definitionDevice)
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
            branchVariable = builder.AddVariable();
            base.Initialize(builder, context);
            stamper = new LargeSignalInductorStamper(Anode, Cathode, branchVariable);
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
            var (veq, req) = IntegrationMethod.GetEquivalents(DefinitionDevice.Inductance / context.TimeStep);
            stamper.Stamp(equations, veq, req);
        }

        /// <summary>Applies model values before first DC bias has been established for the first time.</summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyInitialCondition(IEquationEditor equations, ISimulationContext context)
        {
            stamper.StampInitialCondition(equations, DefinitionDevice.InitialCurrent);
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
            Voltage = context.GetSolutionForVariable(Anode) - context.GetSolutionForVariable(Cathode);

            IntegrationMethod.SetState(Voltage, Current);
        }
    }
}