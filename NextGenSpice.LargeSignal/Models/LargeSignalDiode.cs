using System;
using NextGenSpice.Core;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Devices.Parameters;
using NextGenSpice.LargeSignal.NumIntegration;
using NextGenSpice.LargeSignal.Stamping;
using NextGenSpice.Numerics.Equations;
using static NextGenSpice.LargeSignal.Models.DeviceHelpers;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Large signal model for <see cref="DiodeDevice" /> device.</summary>
    public class LargeSignalDiode : TwoTerminalLargeSignalDevice<DiodeDevice>
    {
        private double capacitanceTreshold; // cached treshold values based by model.
        private readonly CapacitorStamper capacitorStamper;

        private double gmin; // minimal slope of the I-V characteristic of the diode.
        private double ic; // current through the capacitor that models junction capacitance

        // flags if initial condition for given subdevice should be applied
        private bool initialConditionCapacitor;
        private bool initialConditionDiode;
        private double smallBiasTreshold; // cached treshold for diode model characteristic
        private readonly DiodeStamper stamper;

        private double vc; // voltage across the capacitor that models junction capacitance
        private readonly VoltageProxy voltage;
        private double vt; // thermal voltage based on diode model values.

        public LargeSignalDiode(DiodeDevice definitionDevice) : base(definitionDevice)
        {
            stamper = new DiodeStamper();
            capacitorStamper = new CapacitorStamper();
            voltage = new VoltageProxy();

            Voltage = definitionDevice.VoltageHint;
        }

        /// <summary>Diode model parameters.</summary>
        private DiodeParams Parameters => DefinitionDevice.Parameters;

        /// <summary>Integration method used for modifying inner state of the device.</summary>
        private IIntegrationMethod IntegrationMethod { get; set; }

        /// <summary>Performs necessary initialization of the device, like mapping to the equation system.</summary>
        /// <param name="adapter">The equation system builder.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void Initialize(IEquationSystemAdapter adapter, ISimulationContext context)
        {
            stamper.Register(adapter, Anode, Cathode);
            capacitorStamper.Register(adapter, Anode, Cathode);
            voltage.Register(adapter, Anode, Cathode);

            gmin = Parameters.MinimalResistance ?? context.CircuitParameters.MinimalResistance;
            initialConditionCapacitor = true;
            initialConditionDiode = true;
            IntegrationMethod = context.CircuitParameters.IntegrationMethodFactory.CreateInstance();

            Voltage = DefinitionDevice.VoltageHint;

            vt = Parameters.EmissionCoefficient * PhysicalConstants.Boltzmann *
                 PhysicalConstants.CelsiusToKelvin(Parameters.NominalTemperature) /
                 PhysicalConstants.DevicearyCharge;

            smallBiasTreshold = -5 * vt;
            capacitanceTreshold = Parameters.ForwardBiasDepletionCapacitanceCoefficient * Parameters.JunctionPotential;
        }

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
        ///     called once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(ISimulationContext context)
        {
            var vd = voltage.GetValue() - Parameters.SeriesResistance * Current;
            ApplyLinearizedModel(context, vd);
        }

        /// <summary>Applies model values before first DC bias has been established for the first time.</summary>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyInitialCondition(ISimulationContext context)
        {
            var vd = voltage.GetValue();
            if (initialConditionDiode)
            {
                vd = DefinitionDevice.VoltageHint;
                initialConditionDiode = false; // use hint only for the very first iteration
            }

            vd -= Parameters.SeriesResistance * Current;

            ApplyLinearizedModel(context, vd);
        }

        /// <summary>This method is called each time an equation is solved.</summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnEquationSolution(ISimulationContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>Applies linarized diode model to the equation system.</summary>
        /// <param name="equations"></param>
        /// <param name="context"></param>
        /// <param name="vd"></param>
        private void ApplyLinearizedModel(ISimulationContext context, double vd)
        {
            var (id, geq, cd) = GetModelValues(vd);
            var ieq = id - geq * vd;

            // Diode
            stamper.Stamp(geq, -ieq);

            // Capacitance
            var (cieq, cgeq) = IntegrationMethod.GetEquivalents(cd / context.TimeStep);

            if (initialConditionCapacitor) // initial condition
                capacitorStamper.Stamp(0, 0);
            else capacitorStamper.Stamp(cieq, cgeq);

            Voltage = vd;
            Current = id + ic;
        }

        /// <summary>
        ///     Notifies model class that DC bias for given timepoint is established (i.e after Newton-Raphson iterations
        ///     converged).
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnDcBiasEstablished(ISimulationContext context)
        {
            base.OnDcBiasEstablished(context);
            ic = capacitorStamper.GetCurrent();
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
            double vcrit = PnCriticalVoltage(iss, vt);
            vd = PnLimitVoltage(vd, Voltage, vt, vcrit);

            if (vd >= smallBiasTreshold)
            {
                PnJunction(iss, vd, vt, out id, out geq);
                id += vd * gmin;
                geq += gmin;
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