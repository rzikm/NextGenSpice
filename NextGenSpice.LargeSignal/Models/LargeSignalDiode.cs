using System;
using NextGenSpice.Core;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Devices.Parameters;
using NextGenSpice.LargeSignal.NumIntegration;
using NextGenSpice.LargeSignal.Stamping;
using NextGenSpice.Numerics.Equations;
using NextGenSpice.Numerics.Equations.Eq;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Large signal model for <see cref="DiodeDevice" /> device.</summary>
    public class LargeSignalDiode : TwoTerminalLargeSignalDevice<DiodeDevice>
    {
        private DiodeStamper stamper;
        private CapacitorStamper capacitorStamper;
        private VoltageProxy voltage;

        private double capacitanceTreshold; // cached treshold values based by model.

        private double gmin; // minimal slope of the I-V characteristic of the diode.
        private double ic; // current through the capacitor that models junction capacitance

        // flags if initial condition for given subdevice should be applied
        private bool initialConditionCapacitor;
        private bool initialConditionDiode;
        private double smallBiasTreshold; // cached treshold for diode model characteristic

        private double vc; // voltage across the capacitor that models junction capacitance
        private double vt; // thermal voltage based on diode model values.

        public LargeSignalDiode(DiodeDevice definitionDevice) : base(definitionDevice)
        {
            stamper = new DiodeStamper();
            capacitorStamper = new CapacitorStamper();
            voltage = new VoltageProxy();
        }

        /// <summary>Diode model parameters.</summary>
        private DiodeModelParams Parameters => DefinitionDevice.Parameters;

        /// <summary>Integration method used for modifying inner state of the device.</summary>
        private IIntegrationMethod IntegrationMethod { get; set; }

        /// <summary>Specifies how often the model should be updated.</summary>
        public override ModelUpdateMode UpdateMode => ModelUpdateMode.Always;

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
        ///     Notifies model class that DC bias for given timepoint is established. This method can be used for processing
        ///     circuit equation solution for current timepoint.
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

    class DiodeStamper
    {
        private ConductanceStamper cond = new ConductanceStamper();
        private CurrentStamper current = new CurrentStamper();

        public void Register(IEquationSystemAdapter adapter, int anode, int cathode)
        {
            cond.Register(adapter, anode, cathode);
            current.Register(adapter, cathode, anode); // current faces the other way
        }

        public void Stamp(double geq, double ieq)
        {
            cond.Stamp(geq);
            current.Stamp(ieq);
        }
    }
}