using NextGenSpice.Core;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Devices.Parameters;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal.Stamping;
using NextGenSpice.Numerics.Equations;
using System;
using System.Collections.Generic;
using System.Net;
using NextGenSpice.Numerics;

namespace NextGenSpice.LargeSignal.Devices
{
    /// <summary>Large signal model for <see cref="Bjt" /> device.</summary>
    public class LargeSignalBjt : LargeSignalDeviceBase<Bjt>
    {
        private readonly BjtTransistorStamper stamper;

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
        }


        /// <summary>Performs necessary initialization of the device, like mapping to the equation system.</summary>
        /// <param name="adapter">The equation system builder.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void Initialize(IEquationSystemAdapter adapter, ISimulationContext context)
        {
            stamper.Register(adapter, Base, Collector, Emitter);

            vbc.Register(adapter, Base, Collector);
            vbe.Register(adapter, Base, Emitter);

            vT = PhysicalConstants.Boltzmann *
                 PhysicalConstants.CelsiusToKelvin(Parameters.NominalTemperature) /
                 PhysicalConstants.DevicearyCharge;

            // set init condition to help convergence
            var iS = Parameters.SaturationCurrent;
            var nF = Parameters.ForwardEmissionCoefficient;
            var nR = Parameters.ReverseEmissionCoefficient;
            VoltageBaseCollector = DeviceHelpers.PnCriticalVoltage(iS, nF * vT);
//            VoltageBaseEmitter = DeviceHelpers.PnCriticalVoltage(iS, nR * vT);

            CacheModelParams();
        }

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
        ///     called once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(ISimulationContext context)
        {
            // cache params
            vT = PhysicalConstants.Boltzmann *
                 PhysicalConstants.CelsiusToKelvin(Parameters.NominalTemperature) /
                 PhysicalConstants.DevicearyCharge;

            var iS = Parameters.SaturationCurrent;
            var iSe = Parameters.EmitterSaturationCurrent;
            var iSc = Parameters.CollectorSaturationCurrent;

            var nF = Parameters.ForwardEmissionCoefficient;
            var nR = Parameters.ReverseEmissionCoefficient;
            var nE = Parameters.EmitterSaturationCoefficient;
            var nC = Parameters.CollectorSaturationCoefficient;

            var bF = Parameters.ForwardBeta;
            var bR = Parameters.ReverseBeta;

            var vAf = Parameters.ForwardEarlyVoltage;
            var vAr = Parameters.ReverseEarlyVoltage;

            var iKf = Parameters.ForwardCurrentCorner;
            var iKr = Parameters.ReverseCurrentCorner;

            var gmin = Parameters.MinimalResistance ?? context.SimulationParameters.MinimalResistance;

            var polarity = Parameters.IsPnp ? 1 : -1;

            var Ube = VoltageBaseEmitter;
            var Ubc = VoltageBaseCollector;

            // calculate values according to Gummel-Poon model
            // for details see http://qucs.sourceforge.net/tech/node70.html

            DeviceHelpers.PnJunction(iS, Ube, nF * vT, out var iF, out var gif);
            DeviceHelpers.PnJunction(iSe, Ube, nE * vT, out var iBEn, out var gBEn);
            gif += gmin;
            
            var iBEi = iF / bF;
            var gBEi = gif / bF;

            var iBE = iBEi + iBEn;
            var gpi = gBEi + gBEn;
            CurrentBaseEmitter = iBE;

            DeviceHelpers.PnJunction(iS, Ubc, nR * vT, out var iR, out var gir);
            DeviceHelpers.PnJunction(iSc, Ubc, nC * vT, out var iBCn, out var gBCn);

            var iBCi = iR / bR;
            var gBCi = gir / bR;

            var iBC = iBCi + iBCn;
            var gmu = gBCi + gBCn;
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

            var go = -gmr;
            var go2 = -gmr;
            var gm = gmf + gmr;
            
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

            Transconductance = gm;
            OutputConductance = go;
            ConductancePi = gpi;
            ConductanceMu = gmu;

            stamper.Stamp(gpi, gmu, gm, -go, iB, iC, iE);
        }

        /// <summary>This method is called each time an equation is solved.</summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnEquationSolution(ISimulationContext context)
        {
            var iS = Parameters.SaturationCurrent;

            var nF = Parameters.ForwardEmissionCoefficient;
            var nR = Parameters.ReverseEmissionCoefficient;

            var UbeCrit = DeviceHelpers.PnCriticalVoltage(iS, nF * vT);
            var UbcCrit = DeviceHelpers.PnCriticalVoltage(iS, nR * vT);

            var Ube = DeviceHelpers.PnLimitVoltage(vbe.GetValue(), VoltageBaseEmitter, nF * vT, UbeCrit);
            var Ubc = DeviceHelpers.PnLimitVoltage(vbc.GetValue(), VoltageBaseCollector, nR * vT, UbcCrit);

            var delvbe = Ube - VoltageBaseEmitter;
            var delvbc = Ubc - VoltageBaseCollector;
            var cchat = CurrentCollector + (Transconductance + OutputConductance) * delvbe - (OutputConductance + ConductanceMu) * delvbc;
            var cbhat = CurrentBase + ConductancePi * delvbe + ConductanceMu * delvbc;
            var cc = CurrentCollector;
            var cb = CurrentBase;

            var reltol = context.SimulationParameters.RelativeTolerance;
            var abstol = context.SimulationParameters.AbsolutTolerane;

            if (!MathHelper.InTollerance(cchat, cc, abstol, reltol) ||
                !MathHelper.InTollerance(cbhat, cb, abstol, reltol))
            {
                context.ReportNotConverged(this);
            }

            // update voltages
            VoltageBaseEmitter = Ube;
            VoltageBaseCollector = Ubc;
            VoltageBaseCollector = Ubc - Ube;
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
    }
}