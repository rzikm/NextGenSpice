using System;
using System.Collections.Generic;
using NextGenSpice.Core;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Devices.Parameters;
using NextGenSpice.Numerics.Equations;
using NextGenSpice.Numerics.Equations.Eq;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Large signal model for <see cref="BjtDevice" /> device.</summary>
    internal class LargeSignalBjt : LargeSignalDeviceBase<BjtDevice>
    {
        private VoltageProxy vbe, vbc;
        private BjtStamper stamper;

        private double bF;
        private double bR;

        private double iKf;
        private double iKr;
        private double iS;
        private double iSc;
        private double iSe;
        private double nC;
        private double nE;

        private double nF;
        private double nR;

        private double polarity; // PNP vs NPN

        private double vAf;
        private double vAr;

        private double vT; // thermal voltage

        public LargeSignalBjt(BjtDevice definitionDevice) : base(definitionDevice)
        {
            stamper = new BjtStamper();
            vbe = new VoltageProxy();
            vbc = new VoltageProxy();
        }

        /// <summary>Node connected to collector terminal of the transistor.</summary>
        public int Collector => DefinitionDevice.Collector;

        /// <summary>Node connected to base terminal of the transistor.</summary>
        public int Base => DefinitionDevice.Base;

        /// <summary>Node connected to emitter terminal of the transistor.</summary>
        public int Emitter => DefinitionDevice.Emitter;

        /// <summary>Node connected to substrate terminal of the transistor.</summary>
        public int Substrate => DefinitionDevice.Substrate;

        /// <summary>Set of parameters for this device model.</summary>
        public BjtModelParams Parameters => DefinitionDevice.Parameters;

        /// <summary>Specifies how often the model should be updated.</summary>
        public override ModelUpdateMode UpdateMode => ModelUpdateMode.Always;

        /// <summary>Current flowing through the base terminal.</summary>
        public double CurrentBase { get; private set; }

        /// <summary>Current flowing through the collector terminal.</summary>
        public double CurrentCollector { get; private set; }

        /// <summary>Current flowing through the emitter terminal.</summary>
        public double CurrentEmitter { get; private set; }

        /// <summary>Current flowing from base terminal to collector terminal.</summary>
        public double CurrentBaseCollector { get; private set; }

        /// <summary>Current flowing from base terminal to emitter terminal.</summary>
        public double CurrentBaseEmitter { get; private set; }

        /// <summary>Voltage between base and collector terminal.</summary>
        public double VoltageBaseCollector { get; private set; }

        /// <summary>Voltage between base and emitter terminal.</summary>
        public double VoltageBaseEmitter { get; private set; }

        /// <summary>Voltage between collector and emitter terminal.</summary>
        public double VoltageCollectorEmitter { get; private set; }

        private void CacheModelParams()
        {
            vT = PhysicalConstants.Boltzmann *
                 PhysicalConstants.CelsiusToKelvin(Parameters.NominalTemperature) /
                 PhysicalConstants.DevicearyCharge;

            iS = Parameters.SaturationCurrent;
            iSe = Parameters.EmitterSaturationCurrent;
            iSc = Parameters.CollectorSaturationCurrent;

            nF = Parameters.ForwardEmissionCoefficient;
            nR = Parameters.ReverseEmissionCoefficient;
            nE = Parameters.EmitterSaturationCoefficient;
            nC = Parameters.CollectorSaturationCoefficient;

            bF = Parameters.ForwardBeta;
            bR = Parameters.ReverseBeta;

            vAf = Parameters.ForwardEarlyVoltage;
            vAr = Parameters.ReverseEarlyVoltage;

            iKf = Parameters.ForwardCurrentCorner;
            iKr = Parameters.ReverseCurrentCorner;

            polarity = Parameters.IsPnp ? 1 : -1;
        }


        /// <summary>Performs necessary initialization of the device, like mapping to the equation system.</summary>
        /// <param name="adapter">The equation system builder.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void Initialize(IEquationSystemAdapter adapter, ISimulationContext context)
        {
            stamper.Register(adapter, Base, Collector, Emitter);
            vbc.Register(adapter, Base, Collector);
            vbe.Register(adapter, Base, Emitter);

            CacheModelParams();
        }

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
        ///     called once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(ISimulationContext context)
        {
            // calculate values according to Gummel-Poon model
            VoltageBaseEmitter = vbe.GetValue();
            VoltageBaseCollector = vbc.GetValue();
            VoltageCollectorEmitter = VoltageBaseEmitter - VoltageBaseCollector;

            var (gBe, gBc, gMf, gMr, iT) = CalculateModelValues();

            var iBeeq = CurrentBaseEmitter - gBe * VoltageBaseEmitter;
            var iBceq = CurrentBaseCollector - gBc * VoltageBaseCollector;
            var iCeeq = iT - gMf * VoltageBaseEmitter - gMr * VoltageBaseCollector;

            CurrentBase = CurrentBaseEmitter + CurrentBaseCollector;

            stamper.Stamp(gBe, gBc, gMf, gMr, (-iBeeq - iBceq) * polarity, (iBceq - iCeeq) * polarity, (iBeeq + iCeeq) * polarity);

        }

        /// <summary>
        ///     Gets provider instance for specified attribute value or null if no provider for requested parameter exists.
        ///     For example "I" for the current flowing throught the two terminal device.
        /// </summary>
        /// <returns>IPrintValueProvider for specified attribute.</returns>
        public override IEnumerable<IDeviceStatsProvider> GetDeviceStatsProviders()
        {
            return new[]
            {
                new SimpleDeviceStatsProvider("IB", () => CurrentBase),
                new SimpleDeviceStatsProvider("IC", () => CurrentCollector),
                new SimpleDeviceStatsProvider("IE", () => CurrentEmitter),
                new SimpleDeviceStatsProvider("IBE", () => CurrentBaseEmitter),
                new SimpleDeviceStatsProvider("IBC", () => CurrentBaseCollector),
                new SimpleDeviceStatsProvider("VBE", () => VoltageBaseEmitter),
                new SimpleDeviceStatsProvider("VBC", () => VoltageBaseCollector),
                new SimpleDeviceStatsProvider("VCE", () => VoltageCollectorEmitter)
            };
        }


        private (double gBe, double gBc, double gMf, double gMr, double iT) CalculateModelValues()
        {
            // for details see http://qucs.sourceforge.net/tech/node70.html

            var iF = DiodeCurrent(iS, VoltageBaseEmitter, nF);
            var iR = DiodeCurrent(iS, VoltageBaseCollector, nR);

            var q1 = 1 / (1 - VoltageBaseCollector / vAf - VoltageBaseEmitter / vAr);
            var q2 = iF / iKf + iR / iKr;

            var qB = q1 / 2 * (1 + Math.Sqrt(1 + 4 * q2));

            CurrentBaseEmitter = iF / bF + DiodeCurrent(iSe, VoltageBaseEmitter, nE);
            var gBei = iS * Slope(VoltageBaseEmitter, nF) / (nF * vT * bF);
            var gBe = gBei + iSe * Slope(VoltageBaseEmitter, nE) / (nE * vT);

            CurrentBaseCollector = iR / bR + DiodeCurrent(iSc, VoltageBaseCollector, nC);
            var gBci = iS * Slope(VoltageBaseCollector, nR) / (nR * vT * bR);
            var gBc = gBci + iSc * Slope(VoltageBaseCollector, nC) / (nC * vT);

            var iT = (iF - iR) / qB;

            var gIf = gBei * bF;
            var dQbdVbe = q1 * (qB / vAr + gIf / (iKf * Math.Sqrt(1 + 4 * q2)));

            var gIr = gBci * bR;
            var dQbdVbc = q1 * (qB / vAf + gIr / (iKr * Math.Sqrt(1 + 4 * q2)));

            var gMf = 1 / qB * (gIf - iT * dQbdVbe);
            var gMr = 1 / qB * (-gIr - iT * dQbdVbc);

            // calculate terminal currents
            CurrentCollector = iT - 1 / bR * iR;
            CurrentEmitter = -iT - 1 / bF * iF;

            return (gBe, gBc, gMf, gMr, iT);
        }

        private double DiodeCurrent(double saturationCurrent, double voltage, double emissionCoef)
        {
            return saturationCurrent * (Slope(voltage, emissionCoef) - 1);
        }

        private double Slope(double voltage, double emissionCoef)
        {
            return Math.Exp(voltage / (emissionCoef * vT));
        }
    }


    public class BjtStamper
    {
        private IEquationSystemCoefficientProxy nbb;
        private IEquationSystemCoefficientProxy nbc;
        private IEquationSystemCoefficientProxy nbe;
        private IEquationSystemCoefficientProxy ncb;
        private IEquationSystemCoefficientProxy ncc;
        private IEquationSystemCoefficientProxy nce;
        private IEquationSystemCoefficientProxy neb;
        private IEquationSystemCoefficientProxy nec;
        private IEquationSystemCoefficientProxy nee;

        private IEquationSystemCoefficientProxy nb;
        private IEquationSystemCoefficientProxy nc;
        private IEquationSystemCoefficientProxy ne;

        public void Register(IEquationSystemAdapter adapter, int nBase, int nCollector, int nEmitter)
        {
            nbb = adapter.GetMatrixCoefficientProxy(nBase, nBase);
            nbc = adapter.GetMatrixCoefficientProxy(nBase, nCollector);
            nbe = adapter.GetMatrixCoefficientProxy(nBase, nEmitter);
            ncb = adapter.GetMatrixCoefficientProxy(nCollector, nBase);
            ncc = adapter.GetMatrixCoefficientProxy(nCollector, nCollector);
            nce = adapter.GetMatrixCoefficientProxy(nCollector, nEmitter);
            neb = adapter.GetMatrixCoefficientProxy(nEmitter, nBase);
            nec = adapter.GetMatrixCoefficientProxy(nEmitter, nCollector);
            nee = adapter.GetMatrixCoefficientProxy(nEmitter, nEmitter);

            nb = adapter.GetRightHandSideCoefficientProxy(nBase);
            nc = adapter.GetRightHandSideCoefficientProxy(nCollector);
            ne = adapter.GetRightHandSideCoefficientProxy(nEmitter);
        }

        public void Stamp(double gBe, double gBc, double gMf, double gMr, double iB, double iC, double iE)
        {
            nbb.Add(gBe + gBc);
            nbc.Add(-gBc);
            nbe.Add(-gBe);

            ncb.Add(-gBc + gMf + gMr);
            ncc.Add(gBc - gMr);
            nce.Add(-gMf);

            neb.Add(-gBe - gMf - gMr);
            nec.Add(+gMr);
            nee.Add(gBe + gMf);

            nb.Add(iB);
            nc.Add(iC);
            ne.Add(iE);
        }
    }
}