using System;
using System.Collections.Generic;
using NextGenSpice.Core;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Devices.Parameters;
using NextGenSpice.LargeSignal.Stamping;
using NextGenSpice.Numerics.Equations;

using static NextGenSpice.LargeSignal.Models.DeviceHelpers;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Large signal model for <see cref="BjtDevice" /> device.</summary>
    internal class LargeSignalBjt : LargeSignalDeviceBase<BjtDevice>
    {
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
        private readonly BjtStamper stamper;

        private double vAf;
        private double vAr;
        private readonly VoltageProxy vbe;
        private readonly VoltageProxy vbc;

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
        public BjtParams Parameters => DefinitionDevice.Parameters;

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
                 PhysicalConstants.CelsiusToKelvin(27) /
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

            var Ube = vbe.GetValue() * polarity;
            var Ubc = vbc.GetValue() * polarity;
            var Uce = Ube - Ubc;

            var (gBE, gBC, gitr, gitf, iT) = CalculateModelValues();

            var ieqB = CurrentBaseEmitter - Ube * gBE;
            var ieqC = CurrentBaseCollector - Ubc * gBC;
            var ieqE = iT - Ube * gitf + Uce * gitr;

            CurrentBase = CurrentBaseEmitter + CurrentBaseCollector;

            stamper.Stamp(gBE, gBC, gitr, gitf, (-ieqB - ieqC) * polarity, (ieqC - ieqE) * polarity,
                (ieqB + ieqE) * polarity);
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


        private (double gBE, double gBC, double gitr, double gitf, double iT) CalculateModelValues()
        {
            // for details see http://qucs.sourceforge.net/tech/node70.html

            var UbeCrit = PnCriticalVoltage(iS, nF * vT);
            var UbcCrit = PnCriticalVoltage(iS, nR * vT);

            var Ube = vbe.GetValue();
            var Ubc = vbc.GetValue();

            //            VoltageBaseEmitter = Ube = pnVoltage(Ube, Ube, nF * vT, UbeCrit);
            VoltageBaseEmitter = Ube = PnLimitVoltage(Ube, VoltageBaseEmitter, nF * vT, UbeCrit);
            //            VoltageBaseCollector = Ubc = pnVoltage(Ubc, Ubc, nR * vT, UbcCrit);
            VoltageBaseCollector = Ubc = PnLimitVoltage(Ubc, VoltageBaseCollector, nR * vT, UbcCrit);

            double iF, gif;
            PnJunction(iS, Ube, nF * vT, out iF, out gif);
            double iBEn, gBEn;
            PnJunction(iSe, Ube, nE * vT, out iBEn, out gBEn);

            double iBEi = iF / bF;
            double gBEi = gif / bF;

            double iBE = iBEi + iBEn;
            double gBE = gBEi + gBEn;
            CurrentBaseEmitter = iBE;

            double iR, gir;
            PnJunction(iS, Ubc, nR * vT, out iR, out gir);
            double iBCn, gBCn;
            PnJunction(iSc, Ubc, nC * vT, out iBCn, out gBCn);

            double iBCi = iR / bR;
            double gBCi = gir / bR;

            double iBC = iBCi + iBCn;
            double gBC = gBCi + gBCn;
            CurrentBaseCollector = iBC;

            var q1 = 1 / (1 - Ubc / vAf - Ube / vAr); // shouldn't it be * vaf, *var
            var q2 = iF / iKf + iR / iKr;

            var sqrt = Math.Sqrt(1 + 4 * q2);
            var qB = q1 / 2 * (1 + sqrt);

            var dQdbUbe = q1 * (qB / vAr + gif / (iKf * sqrt)); // shouldn't it be * vaf, *var
            var dQbdUbc = q1 * (qB / vAf + gir / (iKr * sqrt));

            var iT = (iF - iR) / qB;

            var gitf = (gif - iT * dQdbUbe) / qB;
            var gitr = (gir - iT * dQbdUbc) / qB;

            var go = -gitr;
            var gm = gitf - go;

            // calculate terminal currents
            CurrentCollector = iT - 1 / bR * iR;
            CurrentEmitter = -iT - 1 / bF * iF;

            return (gBE, gBC, gitr, gitf, iT);
        }


        //
        //        private void calcDC()
        //        {
        //
        //            // fetch device model parameters
        //            double Is = getScaledProperty("Is");
        //            double Nf = getPropertyDouble("Nf");
        //            double Nr = getPropertyDouble("Nr");
        //            double Vaf = getPropertyDouble("Vaf");
        //            double Var = getPropertyDouble("Var");
        //            double Ikf = getScaledProperty("Ikf");
        //            double Ikr = getScaledProperty("Ikr");
        //            double Bf = getScaledProperty("Bf");
        //            double Br = getScaledProperty("Br");
        //            double Ise = getScaledProperty("Ise");
        //            double Isc = getScaledProperty("Isc");
        //            double Ne = getPropertyDouble("Ne");
        //            double Nc = getPropertyDouble("Nc");
        //            double Rb = getScaledProperty("Rb");
        //            double Rbm = getScaledProperty("Rbm");
        //            double Irb = getScaledProperty("Irb");
        //            double T = getPropertyDouble("Temp");
        //
        //            double pol = polarity;
        //
        //            double Ut, Q1, Q2;
        //            double Iben, Ibcn, Ibei, Ibci, Ibc, gbe, gbc, gtiny;
        //            double IeqB, IeqC, IeqE, IeqS, UbeCrit, UbcCrit;
        //            double gm, go;
        //
        //            var Ube = VoltageBaseEmitter * polarity;
        //            var Ubc = VoltageBaseCollector * polarity;
        //
        //            // critical voltage necessary for bad start values
        //            UbeCrit = pnCriticalVoltage(Is, Nf * vT);
        //            UbcCrit = pnCriticalVoltage(Is, Nr * vT);
        //            UbePrev = Ube = pnVoltage(Ube, UbePrev, vT * Nf, UbeCrit);
        //            UbcPrev = Ubc = pnVoltage(Ubc, UbcPrev, vT * Nr, UbcCrit);
        //
        //            double Uce = Ube - Ubc;
        //
        //            // base-emitter diodes
        //            gtiny = Ube < -10 * vT * Nf ? (Is + Ise) : 0;
        //
        //
        //            pnJunctionBIP(Ube, Is, vT * Nf, If, gif);
        //
        //            Ibei = If / Bf;
        //            gbei = gif / Bf;
        //
        //            pnJunctionBIP(Ube, Ise, vT * Ne, Iben, gben);
        //            Iben += gtiny * Ube;
        //
        //            Ibe = Ibei + Iben;
        //            gbe = gbei + gben;
        //
        //            // base-collector diodes
        //            gtiny = Ubc < -10 * vT * Nr ? (Is + Isc) : 0;
        //
        //            pnJunctionBIP(Ubc, Is, vT * Nr, Ir, gir);
        //            Ibci = Ir / Br;
        //            gbci = gir / Br;
        //            pnJunctionBIP(Ubc, Isc, vT * Nc, Ibcn, gbcn);
        //            Ibcn += gtiny * Ubc;
        //            gbcn += gtiny;
        //            Ibc = Ibci + Ibcn;
        //            gbc = gbci + gbcn;
        //
        //            // compvTe base charge quantities
        //            Q1 = 1 / (1 - Ubc * Vaf - Ube * Var);
        //            Q2 = If * Ikf + Ir * Ikr;
        //            double SArg = 1.0 + 4.0 * Q2;
        //            double Sqrt = SArg > 0 ? qucs::sqrt(SArg) : 1;
        //            Qb = Q1 * (1 + Sqrt) / 2;
        //            dQbdUbe = Q1 * (Qb * Var + gif * Ikf / Sqrt);
        //            dQbdUbc = Q1 * (Qb * Vaf + gir * Ikr / Sqrt);
        //
        //            // If and gif will be later used also for the capacitance/charge calculations
        //            // Values compvTed from the excess phase rovTine should be used only
        //            //   for compvTing the companion model current and conductance
        //            double Ifx = If;
        //            double gifx = gif;
        //            // during transient analysis only
        //            if (doTR)
        //            {
        //                // calculate excess phase influence
        //                Ifx /= Qb;
        //                excessPhase(cexState, Ifx, gifx);
        //                Ifx *= Qb;
        //            }
        //
        //            // compvTe transfer current
        //            It = (Ifx - Ir) / Qb;
        //
        //            // compvTe forward and backward transconductance
        //            gitf = (+gifx - It * dQbdUbe) / Qb;
        //            gitr = (-gir - It * dQbdUbc) / Qb;
        //
        //            // compvTe old SPICE values
        //            go = -gitr;
        //            gm = +gitf - go;
        //            setOperatingPoint("gm", gm);
        //            setOperatingPoint("go", go);
        //
        //            // calculate current-dependent base resistance
        //            if (Rbm != 0.0)
        //            {
        //                if (Irb != 0.0)
        //                {
        //                    nr_double_t a, b, z;
        //                    a = (Ibci + Ibcn + Ibei + Iben) / Irb;
        //                    a = std::max(a, NR_TINY); // enforce positive values
        //                    z = (qucs::sqrt(1 + 144 / sqr(pi) * a) - 1) / 24 * sqr(pi) / qucs::sqrt(a);
        //                    b = qucs::tan(z);
        //                    Rbb = Rbm + 3 * (Rb - Rbm) * (b - z) / z / sqr(b);
        //                }
        //                else
        //                {
        //                    Rbb = Rbm + (Rb - Rbm) / Qb;
        //                }
        //                rb->setScaledProperty("R", Rbb);
        //                rb->calcDC();
        //            }
        //
        //            // compvTe avTonomic current sources
        //            IeqB = Ibe - Ube * gbe;
        //            IeqC = Ibc - Ubc * gbc;
        //#if NEWSGP
        //  IeqE = It - Ube * gitf - Ubc * gitr;
        //#else
        //            IeqE = It - Ube * gm - Uce * go;
        //#endif
        //            IeqS = 0;
        //            setI(NODE_B, (-IeqB - IeqC) * pol);
        //            setI(NODE_C, (+IeqC - IeqE - IeqS) * pol);
        //            setI(NODE_E, (+IeqB + IeqE) * pol);
        //            setI(NODE_S, (+IeqS) * pol);
        //
        //            // apply admittance matrix elements
        //#if NEWSGP
        //  setY (NODE_B, NODE_B, gbc + gbe);
        //  setY (NODE_B, NODE_C, -gbc);
        //  setY (NODE_B, NODE_E, -gbe);
        //  setY (NODE_B, NODE_S, 0);
        //  setY (NODE_C, NODE_B, -gbc + gitf + gitr);
        //  setY (NODE_C, NODE_C, gbc - gitr);
        //  setY (NODE_C, NODE_E, -gitf);
        //  setY (NODE_C, NODE_S, 0);
        //  setY (NODE_E, NODE_B, -gbe - gitf - gitr);
        //  setY (NODE_E, NODE_C, gitr);
        //  setY (NODE_E, NODE_E, gbe + gitf);
        //  setY (NODE_E, NODE_S, 0);
        //  setY (NODE_S, NODE_B, 0);
        //  setY (NODE_S, NODE_C, 0);
        //  setY (NODE_S, NODE_E, 0);
        //  setY (NODE_S, NODE_S, 0);
        //#else
        //            setY(NODE_B, NODE_B, gbc + gbe);
        //            setY(NODE_B, NODE_C, -gbc);
        //            setY(NODE_B, NODE_E, -gbe);
        //            setY(NODE_B, NODE_S, 0);
        //            setY(NODE_C, NODE_B, -gbc + gm);
        //            setY(NODE_C, NODE_C, go + gbc);
        //            setY(NODE_C, NODE_E, -go - gm);
        //            setY(NODE_C, NODE_S, 0);
        //            setY(NODE_E, NODE_B, -gbe - gm);
        //            setY(NODE_E, NODE_C, -go);
        //            setY(NODE_E, NODE_E, gbe + go + gm);
        //            setY(NODE_E, NODE_S, 0);
        //            setY(NODE_S, NODE_B, 0);
        //            setY(NODE_S, NODE_C, 0);
        //            setY(NODE_S, NODE_E, 0);
        //            setY(NODE_S, NODE_S, 0);
        //        }
    }
}