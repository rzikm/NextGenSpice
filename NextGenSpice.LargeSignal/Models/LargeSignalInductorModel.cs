using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.LargeSignal.NumIntegration;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>
    ///     Large signal model for <see cref="InductorElement" /> device.
    /// </summary>
    public class LargeSignalInductorModel : TwoNodeLargeSignalModel<InductorElement>
    {
        private int branchVariable;

        public LargeSignalInductorModel(InductorElement definitionElement) : base(definitionElement)
        {
            IntegrationMethod = new TrapezoidalIntegrationMethod();
        }

        /// <summary>
        ///     Integration method used for modifying inner state of the device.
        /// </summary>
        private IIntegrationMethod IntegrationMethod { get; }

        /// <summary>
        ///     If true, the device behavior is not linear is not constant and the
        ///     <see cref="ILargeSignalDeviceModel.ApplyModelValues" /> function is
        ///     called every iteration during nonlinear solving.
        /// </summary>
        public override bool IsNonlinear => false;

        /// <summary>
        ///     If true, the device behavior is not constant over time and the
        ///     <see cref="ILargeSignalDeviceModel.ApplyModelValues" /> function is called
        ///     every timestep.
        /// </summary>
        public override bool IsTimeDependent => true;

        /// <summary>
        ///     Allows models to register additional vairables to the linear system equations. E.g. branch current variables.
        /// </summary>
        /// <param name="builder">The equation system builder.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void RegisterAdditionalVariables(IEquationSystemBuilder builder, ISimulationContext context)
        {
            branchVariable = builder.AddVariable();
            base.RegisterAdditionalVariables(builder, context);
        }

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is called
        ///     once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            var (veq, req) = IntegrationMethod.GetEquivalents(DefinitionElement.Inductance / context.TimeStep);

            equations.AddVoltage(Anode, Kathode, branchVariable, -veq);
            equations.AddMatrixEntry(branchVariable, branchVariable, -req);
        }

        /// <summary>
        ///     Applies model values before first DC bias has been established for the first time.
        /// </summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyInitialCondition(IEquationEditor equations, ISimulationContext context)
        {
            if (DefinitionElement.InitialCurrent.HasValue)
                equations.AddCurrent(Anode, Kathode, DefinitionElement.InitialCurrent.Value);
            else
                equations.AddVoltage(Anode, Kathode, branchVariable, 0);
        }

        /// <summary>
        ///     Notifies model class that DC bias for given timepoint is established. This method can be used for processing
        ///     circuit equation solution
        ///     for current timepoint.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnDcBiasEstablished(ISimulationContext context)
        {
            base.OnDcBiasEstablished(context);
            Current = context.GetSolutionForVariable(branchVariable);
            Voltage = context.GetSolutionForVariable(Anode) - context.GetSolutionForVariable(Kathode);

            IntegrationMethod.SetState(Voltage, Current);
        }
    }
}