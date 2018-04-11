using System;
using NextGenSpice.Core;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Elements.Parameters;
using NextGenSpice.Core.Equations;
using NextGenSpice.Core.NumIntegration;
using NextGenSpice.LargeSignal.Stamping;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Large signal model for <see cref="DiodeElement" /> device.</summary>
    public class LargeSignalDiodeModel : TwoNodeLargeSignalModel<DiodeElement>
    {
        private double capacitanceTreshold; // cached treshold values based by model.

        private int capacitorBranch; // variable for capacitor branch current

        private LargeSignalCapacitorStamper capacitorStamper;

        private double gmin; // minimal slope of the I-V characteristic of the diode.
        private double ic; // current through the capacitor that models junction capacitance

        // flags if initial condition for given subdevice should be applied
        private bool initialConditionCapacitor;
        private bool initialConditionDiode;
        private double smallBiasTreshold; // cached treshold for diode model characteristic

        private double vc; // voltage across the capacitor that models junction capacitance
        private double vt; // thermal voltage based on diode model values.

        public LargeSignalDiodeModel(DiodeElement definitionElement) : base(definitionElement)
        {
        }

        /// <summary>Diode model parameters.</summary>
        private DiodeModelParams Parameters => DefinitionElement.Parameters;

        /// <summary>Integration method used for modifying inner state of the device.</summary>
        private IIntegrationMethod IntegrationMethod { get; set; }

        /// <summary>Specifies how often the model should be updated.</summary>
        public override ModelUpdateMode UpdateMode => ModelUpdateMode.Always;

        /// <summary>
        ///     Allows models to register additional vairables to the linear system equations. E.g. branch current variables.
        ///     And perform other necessary initialization
        /// </summary>
        /// <param name="builder">The equation system builder.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void Initialize(IEquationSystemBuilder builder, ISimulationContext context)
        {
            base.Initialize(builder, context);
            capacitorBranch = builder.AddVariable();
            capacitorStamper = new LargeSignalCapacitorStamper(Anode, Cathode, capacitorBranch);

            gmin = Parameters.MinimalResistance ?? context.CircuitParameters.MinimalResistance;
            initialConditionCapacitor = true;
            initialConditionDiode = true;
            IntegrationMethod = context.CircuitParameters.IntegrationMethodFactory.CreateInstance();

            Voltage = DefinitionElement.VoltageHint;

            vt = Parameters.EmissionCoefficient * PhysicalConstants.Boltzmann *
                 PhysicalConstants.CelsiusToKelvin(Parameters.NominalTemperature) /
                 PhysicalConstants.ElementaryCharge;

            smallBiasTreshold = -5 * vt;
            capacitanceTreshold = Parameters.ForwardBiasDepletionCapacitanceCoefficient * Parameters.JunctionPotential;
        }

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
        ///     called once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            var vd = context.GetSolutionForVariable(DefinitionElement.Anode) -
                     context.GetSolutionForVariable(DefinitionElement.Cathode) - Parameters.SeriesResistance * Current;
            ApplyLinearizedModel(equations, context, vd);
        }

        /// <summary>Applies model values before first DC bias has been established for the first time.</summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyInitialCondition(IEquationEditor equations, ISimulationContext context)
        {
            var vd = context.GetSolutionForVariable(DefinitionElement.Anode) -
                     context.GetSolutionForVariable(DefinitionElement.Cathode);
            if (initialConditionDiode)
            {
                vd = DefinitionElement.VoltageHint;
                initialConditionDiode = false; // use hint only for the very first iteration
            }
            else
            {
                vd -= Parameters.SeriesResistance * Current;
            }

            ApplyLinearizedModel(equations, context, vd);
        }

        /// <summary>Applies linarized diode model to the equation system.</summary>
        /// <param name="equations"></param>
        /// <param name="context"></param>
        /// <param name="vd"></param>
        private void ApplyLinearizedModel(IEquationEditor equations, ISimulationContext context, double vd)
        {
            var (id, geq, cd) = GetModelValues(vd);
            var ieq = id - geq * vd;

            // Diode
            equations
                .AddConductance(Anode, Cathode, geq)
                .AddCurrent(Cathode, Anode, ieq);

            // Capacitance
            var (cieq, cgeq) = IntegrationMethod.GetEquivalents(cd / context.TimeStep);

            if (initialConditionCapacitor) // initial condition
                capacitorStamper.StampInitialCondition(equations, null);
            else capacitorStamper.Stamp(equations, cieq, cgeq);

            Voltage = vd;
            Current = id + ic;
        }

        /// <summary>
        ///     Notifies model class that DC bias for given timepoint is established. This method can be used for processing
        ///     circuit equation solution for current timepoint.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnDcBiasEstablished(ISimulationContext context)
        {
            base.OnDcBiasEstablished(context);
            ic = context.GetSolutionForVariable(capacitorBranch);
            if (Math.Abs(context.TimeStep) < double.Epsilon) // set initial condition for the capacitor
                ic = Current;
            vc = Voltage;

            IntegrationMethod.SetState(ic, vc);
            initialConditionCapacitor = false; // capacitor no longer needs initial condition
        }

        /// <summary>Gets values for the device model based on voltage across the diode.</summary>
        /// <param name="vd">Voltage across the diode.</param>
        /// <returns></returns>
        private (double id, double geq, double cd) GetModelValues(double vd)
        {
            var m = Parameters.JunctionGradingCoefficient;
            var vj = Parameters.JunctionPotential;
            var fc = Parameters.ForwardBiasDepletionCapacitanceCoefficient;
            var tt = Parameters.TransitTime;
            var iss = Parameters.SaturationCurrent;
            var jc = Parameters.JunctionCapacitance;
            var bv = Parameters.ReverseBreakdownVoltage;

            double id, geq;
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

            var cd = -tt * geq;

            if (vd < capacitanceTreshold)
                cd += jc / Math.Pow(1 - vd / vj, m);
            else
                cd += jc / Math.Pow(1 - fc, 1 + m) * (1 - fc * (1 + m) + m * vd / vj);

            return (id, geq, cd);
        }
    }
}