using System;
using System.Collections.Generic;
using NextGenSpice.Core;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Devices.Parameters;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal.Stamping;
using NextGenSpice.Numerics;
using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Devices
{
    /// <summary>Large signal model for <see cref="Bjt" /> device.</summary>
    public class LargeSignalBjt : LargeSignalDeviceBase<Bjt>
    {
        private readonly BjtTransistorStamper stamper;
        private readonly VoltageProxy vbc;

        private readonly VoltageProxy vbe;

        private readonly ConductanceStamper gb;
        private readonly ConductanceStamper gc;
        private readonly ConductanceStamper ge;

        private int bprimeNode;
        private int cprimeNode;
        private int eprimeNode;

        private double vT; // thermal voltage

        public LargeSignalBjt(Bjt definitionDevice) : base(definitionDevice)
        {
            stamper = new BjtTransistorStamper();
            vbe = new VoltageProxy();
            vbc = new VoltageProxy();

            gb = new ConductanceStamper();
            gc = new ConductanceStamper();
            ge = new ConductanceStamper();
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

        /// <summary>Transconductance computed for the current timepoint.</summary>
        public double Transconductance { get; private set; }

        /// <summary>Output conductance computed for the current timepoint.</summary>
        public double OutputConductance { get; private set; }

        /// <summary>Computed conductance between the base and emitter terminals.</summary>
        public double ConductancePi { get; private set; }

        /// <summary>Computed conductance between the base and collector terminals.</summary>
        public double ConductanceMu { get; private set; }


        /// <summary>Allows devices to register any additional variables.</summary>
        /// <param name="adapter">The equation system builder.</param>
        public override void RegisterAdditionalVariables(IEquationSystemAdapter adapter)
        {
            base.RegisterAdditionalVariables(adapter);

            bprimeNode = Parameters.BaseResistance > 0 ? adapter.AddVariable() : Base;
            cprimeNode = Parameters.CollectorResistance > 0 ? adapter.AddVariable() : Collector;
            eprimeNode = Parameters.EmitterCapacitance > 0 ? adapter.AddVariable() : Emitter;
        }

        /// <summary>Performs necessary initialization of the device, like mapping to the equation system.</summary>
        /// <param name="adapter">The equation system builder.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void Initialize(IEquationSystemAdapter adapter, ISimulationContext context)
        {
            stamper.Register(adapter, bprimeNode, cprimeNode, eprimeNode);

            vbc.Register(adapter, bprimeNode, cprimeNode);
            vbe.Register(adapter, bprimeNode, eprimeNode);

            gb.Register(adapter, bprimeNode, Base);
            gc.Register(adapter, cprimeNode, Collector);
            ge.Register(adapter, eprimeNode, Emitter);

            vT = PhysicalConstants.Boltzmann *
                 PhysicalConstants.CelsiusToKelvin(Parameters.NominalTemperature) /
                 PhysicalConstants.DevicearyCharge;


            VoltageBaseEmitter = DeviceHelpers.PnCriticalVoltage(Parameters.SaturationCurrent, vT);
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

            var polarity = Parameters.IsPnp ? -1 : +1;

            var ggb = Parameters.BaseResistance > 0 ? 1 / Parameters.BaseResistance : 0;
            var ggc = Parameters.CollectorResistance > 0 ? 1 / Parameters.CollectorResistance : 0;
            var gge = Parameters.EmitterCapacitance > 0 ? 1 / Parameters.EmitterCapacitance : 0;


            var Ube = VoltageBaseEmitter;
            var Ubc = VoltageBaseCollector;

            var (CurrentBe, CondBe) = DeviceHelpers.PnBJT(iS, Ube, nF * vT, gmin);
            var (iBEn, gBEn) = DeviceHelpers.PnBJT(iSe, Ube, nE * vT, 0);

            var (CurrentBc, CondBc) = DeviceHelpers.PnBJT(iS, Ubc, nR * vT, gmin);
            var (iBCn, gBCn) = DeviceHelpers.PnBJT(iSc, Ubc, nR * vT, 0);

            //
            //            var iBE = iBEi + iBEn;
            //            var gpi = CondBe/ bF + gBEn;
            //            CurrentBaseEmitter = iBE;




            //            DeviceHelpers.PnJunction(iS, Ubc, nR * vT, out var iR, out var gir);
            //            DeviceHelpers.PnJunction(iSc, Ubc, nC * vT, out var iBCn, out var gBCn);
            //            gir += gmin;
            //            var iBC = iBCi + iBCn;
            //            var gmu = CondBc / bR + gBCn;
            //            CurrentBaseCollector = iBC;

            var q1 = 1 / (1 - Ubc / vAf - Ube / vAr);
            var q2 = CurrentBe / iKf + CurrentBc / iKr;

            var sqrt = Math.Sqrt(1 + 4 * q2);
            var qB = q1 / 2 * (1 + sqrt);

            var dQdbUbe = q1 * (qB / vAr + CondBe / (iKf * sqrt));
            var dQbdUbc = q1 * (qB / vAf + CondBc / (iKr * sqrt));

            // excess phase missing
            var cc = 0.0;
            var cex = CurrentBe;
            var gex = CondBe;

            cc = cc + (cex - CurrentBc) / qB - CurrentBc / bR - iBCn;
            var cb = CurrentBe / bF + iBEn + CurrentBc / bR + iBCn;

            //            var iT = (iF - iR) / qB;
            //
            //            var gmf = (gif - iT * dQdbUbe) / qB;
            //            var gmr = (gir - iT * dQbdUbc) / qB;


            var gpi = CondBe / bF + gBEn;
            var gmu = CondBc / bR + gBCn;
            var go = (CondBc + (cex - CurrentBc) * dQbdUbc / qB) / qB;
            var gm = (gex - (cex - CurrentBc) * dQdbUbe / qB) / qB - go;


            //            var go = -gmr;
            //            var gm = gmf + gmr;

            // calculate terminal currents
            //            CurrentCollector = iT - 1 / bR * iR;
            //            CurrentEmitter = -iT - 1 / bF * iF;
            //            CurrentBase = CurrentBaseEmitter + CurrentBaseCollector;

            var ceqbe = polarity * (cc + cb - Ube * (gm + go + gpi) + Ubc * go);
            var ceqbc = polarity * (-cc + Ube * (gm + go) - Ubc * (gmu + go));

            //            var ibeeq = iBE - gpi * Ube;
            //            var ibceq = iBC - gmu * Ubc;
            //            var iceeq = iT - gmf * Ube + gmr * Ubc;

            var ibeeq = ceqbe;
            var ibceq = ceqbc;
            var iceeq = 0;

            CurrentBase = cb;
            CurrentCollector = cc;
            CurrentEmitter = -cb - cc;
            CurrentBaseEmitter = CurrentBe;
            CurrentBaseCollector = CurrentBc;



            Transconductance = gm;
            OutputConductance = go;
            ConductancePi = gpi;
            ConductanceMu = gmu;

            stamper.Stamp(gpi, gmu, gm, -go, ibeeq, ibceq, iceeq);
//            stamper.Stamp(gpi, gmu, gm, go, ibeeq * polarity, ibceq * polarity, iceeq * polarity);
            gb.Stamp(ggb);
            ge.Stamp(gge);
            gc.Stamp(ggc);
        }

        /// <summary>This method is called each time an equation is solved.</summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnEquationSolution(ISimulationContext context)
        {
            var iS = Parameters.SaturationCurrent;

            var nF = Parameters.ForwardEmissionCoefficient;
            var nR = Parameters.ReverseEmissionCoefficient;
            var polarity = Parameters.IsPnp ? -1 : +1;


            var UbeCrit = DeviceHelpers.PnCriticalVoltage(iS, nF * vT);
            var UbcCrit = DeviceHelpers.PnCriticalVoltage(iS, nR * vT);

            var vvbe = vbe.GetValue() * polarity;
            var vvbc = vbc.GetValue() * polarity;

            var (Ube, limited) = DeviceHelpers.PnLimitVoltage(vvbe, VoltageBaseEmitter, nF * vT, UbeCrit);
            var (Ubc, limited2) = DeviceHelpers.PnLimitVoltage(vvbc, VoltageBaseCollector, nR * vT, UbcCrit);

            var delvbe = Ube - VoltageBaseEmitter;
            var delvbc = Ubc - VoltageBaseCollector;
            var cchat = CurrentCollector + (Transconductance + OutputConductance) * delvbe -
                        (OutputConductance + ConductanceMu) * delvbc;
            var cbhat = CurrentBase + ConductancePi * delvbe + ConductanceMu * delvbc;
            var cc = CurrentCollector;
            var cb = CurrentBase;

            var reltol = context.SimulationParameters.RelativeTolerance;
            var abstol = context.SimulationParameters.AbsoluteTolerance;

            if (limited || limited2 ||
                !MathHelper.InTollerance(cchat, cc, abstol, reltol) ||
                !MathHelper.InTollerance(cbhat, cb, abstol, reltol))
                context.ReportNotConverged(this);

            // update voltages
            VoltageBaseEmitter = Ube;
            VoltageBaseCollector = Ubc;
            VoltageCollectorEmitter = Ubc - Ube;
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