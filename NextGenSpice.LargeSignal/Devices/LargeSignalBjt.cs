﻿using NextGenSpice.Core;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Devices.Parameters;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal.Stamping;
using NextGenSpice.Numerics.Equations;
using System;
using System.Collections.Generic;
using System.Net;

namespace NextGenSpice.LargeSignal.Devices
{
    /// <summary>Large signal model for <see cref="Bjt" /> device.</summary>
    public class LargeSignalBjt : LargeSignalDeviceBase<Bjt>
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

        private double gmin;

        private double polarity; // PNP vs NPN
        private readonly BjtTransistorStamper stamper;

        private double vAf;
        private double vAr;
        private readonly VoltageProxy vbe;
        private readonly VoltageProxy vbc;

        private double vT; // thermal voltage

        public LargeSignalBjt(Bjt definitionDevice) : base(definitionDevice)
        {
            stamper = new BjtTransistorStamper();
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

        public double Transconductance { get; private set; }

        public double OutputConductance { get; private set; }

        public double ConductancePi { get; private set; }

        public double ConductanceMu { get; private set; }

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

            gmin = 1e-12;

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
            // for details see http://qucs.sourceforge.net/tech/node70.html


            var Ube = VoltageBaseEmitter;
            var Ubc = VoltageBaseCollector;
            var Uce = Ubc - Ube;

            double iF, gif;
            DeviceHelpers.PnJunction(iS, Ube, nF * vT, out iF, out gif);
            double iBEn, gBEn;
            DeviceHelpers.PnJunction(iSe, Ube, nE * vT, out iBEn, out gBEn);

            double iBEi = iF / bF;
            double gBEi = gif / bF;

            double iBE = iBEi + iBEn;
            double gBE = gBEi + gBEn;
            CurrentBaseEmitter = iBE;

            double iR, gir;
            DeviceHelpers.PnJunction(iS, Ubc, nR * vT, out iR, out gir);
            double iBCn, gBCn;
            DeviceHelpers.PnJunction(iSc, Ubc, nC * vT, out iBCn, out gBCn);

            double iBCi = iR / bR;
            double gBCi = gir / bR;

            double iBC = iBCi + iBCn;
            double gBC = gBCi + gBCn;
            CurrentBaseCollector = iBC;

            var q1 = 1 / (1 - Ubc / vAf - Ube / vAr);
            var q2 = iF / iKf + iR / iKr;

            var sqrt = Math.Sqrt(1 + 4 * q2);
            var qB = q1 / 2 * (1 + sqrt);

            var dQdbUbe = q1 * (qB / vAr + gif / (iKf * sqrt)); 
            var dQbdUbc = q1 * (qB / vAf + gir / (iKr * sqrt));

            var iT = (iF - iR) / qB;

            var gmf = (gif - iT * dQdbUbe) / qB;
            var gmr = (gir - iT * dQbdUbc) / qB;

            var go = -gmr - gmin;
            var gm = gmf + gmr;

            var gpi = gBE + gmin;
            var gmu = gBC + gmin;

            // calculate terminal currents
            CurrentCollector = iT - 1 / bR * iR;
            CurrentEmitter = -iT - 1 / bF * iF;

            var cc = CurrentCollector;
            var cb = CurrentBaseEmitter;

            var ceqbe = polarity * (cc + cb - Ube * (gm + go + gpi) + Ubc * go);
            var ceqbc = polarity * (-cc + Ube * (gm + go) - Ubc * (gmu + go));

            CurrentBase = CurrentBaseEmitter + CurrentBaseCollector;

            var iC = -ceqbc;
            var iB = ceqbe + ceqbc;
            var iE = -ceqbe;

            stamper.Stamp(gpi, gmu, gm, -go, iB , iC, iE );
        }

        /// <summary>This method is called each time an equation is solved.</summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnEquationSolution(ISimulationContext context)
        {
            var UbeCrit = DeviceHelpers.PnCriticalVoltage(iS, nF * vT);
            var UbcCrit = DeviceHelpers.PnCriticalVoltage(iS, nR * vT);

            var Ube = vbe.GetValue();
            var Ubc = vbc.GetValue();

            var delvbe = Ube - VoltageBaseEmitter;
            var delvbc = Ubc - VoltageBaseCollector;
            var cchat = CurrentCollector + (Transconductance + OutputConductance) * delvbe - (OutputConductance + ConductanceMu) * delvbc;
            var cbhat = CurrentBase + ConductancePi * delvbe + ConductanceMu * delvbc;
            var cc = CurrentCollector;
            var cb = CurrentBase;

            var reltor = context.SimulationParameters.RelativeTolerance;
            var abstol = context.SimulationParameters.AbsolutTolerane;
            
            //  compare and set flag
            var tol = reltor * Math.Max(Math.Abs(cchat), Math.Abs(cc)) + abstol;
            if (Math.Abs(cchat - cc) > tol)
            {
                context.ReportNotConverged(this);
            }

            tol = reltor * Math.Max(Math.Abs(cbhat), Math.Abs(cb)) + abstol;
            if (Math.Abs(cbhat - cb) > tol)
            {
                context.ReportNotConverged(this);
            }

            // update voltages
            VoltageBaseEmitter = DeviceHelpers.PnLimitVoltage(Ube, VoltageBaseEmitter, nF * vT, UbeCrit);
            VoltageBaseCollector = DeviceHelpers.PnLimitVoltage(Ubc, VoltageBaseCollector, nR * vT, UbcCrit);
        }

        /// <summary>
        ///     Gets provider instance for specified attribute value or null if no provider for requested parameter exists.
        ///     For example "I" for the current flowing throught the two terminal device.
        /// </summary>
        /// <returns>IPrintValueProvider for specified attribute.</returns>
        public override IEnumerable<IDeviceStateProvider> GetDeviceStateProviders()
        {
            return new[]
            {
                new SimpleDeviceStateProvider("IB", () => CurrentBase),
                new SimpleDeviceStateProvider("IC", () => CurrentCollector),
                new SimpleDeviceStateProvider("IE", () => CurrentEmitter),
                new SimpleDeviceStateProvider("IBE", () => CurrentBaseEmitter),
                new SimpleDeviceStateProvider("IBC", () => CurrentBaseCollector),
                new SimpleDeviceStateProvider("VBE", () => VoltageBaseEmitter),
                new SimpleDeviceStateProvider("VBC", () => VoltageBaseCollector),
                new SimpleDeviceStateProvider("VCE", () => VoltageCollectorEmitter)
            };
        }


        private (double gBE, double gBC, double gitr, double gitf, double iT) CalculateModelValues()
        {
            // for details see http://qucs.sourceforge.net/tech/node70.html

            var UbeCrit = DeviceHelpers.PnCriticalVoltage(iS, nF * vT);
            var UbcCrit = DeviceHelpers.PnCriticalVoltage(iS, nR * vT);

            var Ube = vbe.GetValue();
            var Ubc = vbc.GetValue();

            VoltageBaseEmitter = Ube = DeviceHelpers.PnLimitVoltage(Ube, VoltageBaseEmitter, nF * vT, UbeCrit);
            VoltageBaseCollector = Ubc = DeviceHelpers.PnLimitVoltage(Ubc, VoltageBaseCollector, nR * vT, UbcCrit);

            double iF, gif;
            DeviceHelpers.PnJunction(iS, Ube, nF * vT, out iF, out gif);
            double iBEn, gBEn;
            DeviceHelpers.PnJunction(iSe, Ube, nE * vT, out iBEn, out gBEn);

            double iBEi = iF / bF;
            double gBEi = gif / bF;

            double iBE = iBEi + iBEn;
            double gBE = gBEi + gBEn;
            CurrentBaseEmitter = iBE;

            double iR, gir;
            DeviceHelpers.PnJunction(iS, Ubc, nR * vT, out iR, out gir);
            double iBCn, gBCn;
            DeviceHelpers.PnJunction(iSc, Ubc, nC * vT, out iBCn, out gBCn);

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

            var gmf = (gif - iT * dQdbUbe) / qB;
            var gmr = (gir - iT * dQbdUbc) / qB;

            var go = -gmr - gmin;
            var gm = gmf + gmr;

            var gpi = gBE + gmin;
            var gmu = gBC + gmin;

            // calculate terminal currents
            CurrentCollector = iT - 1 / bR * iR;
            CurrentEmitter = -iT - 1 / bF * iF;

            //            return (gpi, gmu, gmf, gmr, iT);
            return (gpi, gmu, gm, -go, iT);
        }
    }
}