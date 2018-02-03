using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.LargeSignal.NumIntegration;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>
    ///     Large signal model for <see cref="CapacitorElement" /> device.
    /// </summary>
    public class LargeSignalCapacitorModel : TwoNodeLargeSignalModel<CapacitorElement>
    {
        private int branchVariable;

        public LargeSignalCapacitorModel(CapacitorElement definitionElement) : base(definitionElement)
        {
            IntegrationMethod = new GearIntegrationMethod(2);
//            IntegrationMethod = new AdamsMoultonIntegrationMethod(4);
//            IntegrationMethod = new BackwardEulerIntegrationMethod();
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
            base.RegisterAdditionalVariables(builder, context);
            branchVariable = builder.AddVariable();
        }

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is called
        ///     once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            var (ieq, geq) = IntegrationMethod.GetEquivalents(DefinitionElement.Capacity / context.TimeStep);

            equations.AddMatrixEntry(branchVariable, Anode, geq);
            equations.AddMatrixEntry(branchVariable, Kathode, -geq);
            AddBranchCurrent(equations, ieq);
        }

        /// <summary>
        ///     Adds current flowing through the capacitor to the equation system.
        /// </summary>
        /// <param name="equations"></param>
        /// <param name="ieq"></param>
        private void AddBranchCurrent(IEquationEditor equations, double ieq)
        {
            equations.AddMatrixEntry(Anode, branchVariable, 1);
            equations.AddMatrixEntry(Kathode, branchVariable, -1);

            equations.AddMatrixEntry(branchVariable, branchVariable, -1);

            equations.AddRightHandSideEntry(branchVariable, ieq);
        }

        /// <summary>
        ///     Applies model values before first DC bias has been established for the first time.
        /// </summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyInitialCondition(IEquationEditor equations, ISimulationContext context)
        {
            if (DefinitionElement.InitialVoltage.HasValue) // model using voltage source
                equations.AddVoltage(Anode, Kathode, branchVariable, DefinitionElement.InitialVoltage.Value);
            else // model as open circuit
                AddBranchCurrent(equations, 0);
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

            var vc = context.GetSolutionForVariable(DefinitionElement.Anode) -
                     context.GetSolutionForVariable(DefinitionElement.Kathode);
            Voltage = vc;

            IntegrationMethod.SetState(Current, Voltage);
        }
    }
}