using System;
using NextGenSpice.Core;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.LargeSignal.NumIntegration;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>
    ///     Large signal model for <see cref="DiodeElement" /> device.
    /// </summary>
    public class LargeSignalDiodeModel : TwoNodeLargeSignalModel<DiodeElement>
    {
        private readonly double capacitanceTreshold; // cached treshold values based by model.

        private readonly double smallBiasTreshold; // cached treshold for diode model characteristic
        private readonly double vt; // thermal voltage based on diode model values.
        private int capacitorBranch; // variable for capacitor branch current

        private double gmin; // minimal slope of the I-V characteristic of the diode.

        private double ic; // current through the capacitor that models junction capacitance
        private double vc; // voltage across the capacitor that models junction capacitance

        public LargeSignalDiodeModel(DiodeElement definitionElement) : base(definitionElement)
        {
//            IntegrationMethod = new AdamsMoultonIntegrationMethod(2);
            IntegrationMethod = new GearIntegrationMethod(2);
//            IntegrationMethod = new TrapezoidalIntegrationMethod();
//            IntegrationMethod = new BackwardEulerIntegrationMethod();
//            IntegrationMethod = new AdamsMoultonIntegrationMethod(4);

            Voltage = Parameters.Vd;

            vt = Parameters.EmissionCoefficient * PhysicalConstants.Boltzmann *
                 PhysicalConstants.CelsiusToKelvin(Parameters.Temperature) /
                 PhysicalConstants.ElementaryCharge;

            smallBiasTreshold = -5 * vt;
            capacitanceTreshold = Parameters.ForwardBiasDepletionCapacitanceCoefficient * Parameters.JunctionPotential;
        }

        /// <summary>
        ///     Diode model parameters.
        /// </summary>
        private DiodeModelParams Parameters => DefinitionElement.Parameters;

        /// <summary>
        ///     Integration method used for modifying inner state of the device.
        /// </summary>
        private IIntegrationMethod IntegrationMethod { get; }

        /// <summary>
        ///     If true, the device behavior is not linear is not constant and the
        ///     <see cref="ILargeSignalDeviceModel.ApplyModelValues" /> function is
        ///     called every iteration during nonlinear solving.
        /// </summary>
        public override bool IsNonlinear => true;

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
            capacitorBranch = builder.AddVariable();
            gmin = Parameters.MinimalResistance ?? context.CircuitParameters.MinimalResistance;
        }

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is called
        ///     once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            var vd = context.GetSolutionForVariable(DefinitionElement.Anode) -
                     context.GetSolutionForVariable(DefinitionElement.Kathode) - Parameters.SeriesResistance * Current;
            ApplyLinearizedModel(equations, context, vd);
        }

        /// <summary>
        ///     Applies model values before first DC bias has been established for the first time.
        /// </summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyInitialCondition(IEquationEditor equations, ISimulationContext context)
        {
            var vd = context.GetSolutionForVariable(DefinitionElement.Anode) -
                     context.GetSolutionForVariable(DefinitionElement.Kathode);
            if (vd == 0) vd = Parameters.Vd;
            else
                vd -= Parameters.SeriesResistance * Current
                    ;
            ApplyLinearizedModel(equations, context, vd);
        }

        /// <summary>
        ///     Applies linarized diode model to the equation system.
        /// </summary>
        /// <param name="equations"></param>
        /// <param name="context"></param>
        /// <param name="vd"></param>
        private void ApplyLinearizedModel(IEquationEditor equations, ISimulationContext context, double vd)
        {
            var (id, geq, cd) = GetModelValues(vd);
            var ieq = id - geq * vd;

            // Diode
            equations
                .AddConductance(Anode, Kathode, geq)
                .AddCurrent(Kathode, Anode, ieq)
                ;

            // Capacitance
            var (cgeq, cieq) = IntegrationMethod.GetEquivalents(cd / context.TimeStep);

            if (context.TimeStep > 0) // do not apply capacitor during the initial condition (model as open circuit)
            {
                equations.AddMatrixEntry(capacitorBranch, Anode, cgeq);
                equations.AddMatrixEntry(capacitorBranch, Kathode, -cgeq);
                equations.AddRightHandSideEntry(capacitorBranch, cieq);
            }
            equations.AddMatrixEntry(Anode, capacitorBranch, +1);
            equations.AddMatrixEntry(Kathode, capacitorBranch, -1);
            equations.AddMatrixEntry(capacitorBranch, capacitorBranch, -1);

            Voltage = vd;
            Current = id + ic;
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
            ic = context.GetSolutionForVariable(capacitorBranch);
            if (context.TimeStep == 0) // set initial condition for the capacitor
                ic = Current;
            vc = Voltage;

            IntegrationMethod.SetState(ic, vc);
        }

        /// <summary>
        ///     Gets values for the device model based on voltage across the diode.
        /// </summary>
        /// <param name="vd"></param>
        /// <returns></returns>
        private (double, double, double) GetModelValues(double vd)
        {
            var m = Parameters.JunctionGradingCoefficient;
            var vj = Parameters.JunctionPotential;
            var fc = Parameters.ForwardBiasDepletionCapacitanceCoefficient;
            var tt = Parameters.TransitTime;
            var iss = Parameters.SaturationCurrent;
            var jc = Parameters.JunctionCapacitance;
            var bv = Parameters.ReverseBreakdownVoltage;

            double id, geq, cd;
            if (vd >= smallBiasTreshold)
            {
                id = iss * (Math.Exp(vd / vt) - 1) + vd * gmin;
                geq = iss * Math.Exp(vd / vt) / vt + gmin;
            }
            else if (vd > -bv)
            {
                id = -iss;
                geq = gmin;
            }
            else
            {
                id = -iss * (Math.Exp(-(bv + vd) / vt) - 1 + bv / vt);
                geq = iss * Math.Exp(-(bv + vd) / vt) / vt;
            }

            cd = -tt * geq;

            if (vd < capacitanceTreshold)
                cd += jc / Math.Pow(1 - vd / vj, m);
            else
                cd += jc / Math.Pow(1 - fc, 1 + m) * (1 - fc * (1 + m) + m * vd / vj);

            return (id, geq, cd);
        }
    }
}