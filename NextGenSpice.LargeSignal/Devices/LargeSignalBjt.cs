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
        private readonly VoltageProxy voltageBc;

        private readonly VoltageProxy voltageBe;

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
            voltageBe = new VoltageProxy();
            voltageBc = new VoltageProxy();

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

            voltageBc.Register(adapter, bprimeNode, cprimeNode);
            voltageBe.Register(adapter, bprimeNode, eprimeNode);

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


            var vbe = VoltageBaseEmitter;
            var vbc = VoltageBaseCollector;

            // calculate junction currents
            var (ibe, gbe) = DeviceHelpers.PnBJT(iS, vbe, nF * vT, gmin);
            var (iben, gben) = DeviceHelpers.PnBJT(iSe, vbe, nE * vT, 0);

            var (ibc, gbc) = DeviceHelpers.PnBJT(iS, vbc, nR * vT, gmin);
            var (ibcn, gbcn) = DeviceHelpers.PnBJT(iSc, vbc, nC * vT, 0);

            // base charge calculation
            var q1 = 1 / (1 - vbc / vAf - vbe / vAr);
            var q2 = ibe / iKf + ibc / iKr;

            var sqrt = Math.Sqrt(1 + 4 * q2);
            var qB = q1 / 2 * (1 + sqrt);

            var dQdbUbe = q1 * (qB / vAr + gbe / (iKf * sqrt));
            var dQbdUbc = q1 * (qB / vAf + gbc / (iKr * sqrt));

            // excess phase missing
            var ic = (ibe - ibc) / qB - ibc / bR - ibcn;
            var ib = ibe / bF + iben + ibc / bR + ibcn;
            var gpi = gbe / bF + gben;
            var gmu = gbc / bR + gbcn;
            var go = (gbc + (ibe - ibc) * dQbdUbc / qB) / qB;
            var gm = (gbe - (ibe - ibc) * dQdbUbe / qB) / qB - go;

            // terminal currents
            var ceqbe = polarity * (ic + ib - vbe * (gm + go + gpi) + vbc * go);
            var ceqbc = polarity * (-ic + vbe * (gm + go) - vbc * (gmu + go));

            CurrentBase = ib;
            CurrentCollector = ic;
            CurrentEmitter = -ib - ic;
            CurrentBaseEmitter = ibe;
            CurrentBaseCollector = ibc;
            Transconductance = gm;
            OutputConductance = go;
            ConductancePi = gpi;
            ConductanceMu = gmu;

            stamper.Stamp(gpi, gmu, gm, -go, ceqbe, ceqbc);
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

            // critical voltages to prevent numerical overflow
            var vbecrit = DeviceHelpers.PnCriticalVoltage(iS, nF * vT);
            var vbccrit = DeviceHelpers.PnCriticalVoltage(iS, nR * vT);

            var vvbe = voltageBe.GetValue() * polarity;
            var vvbc = voltageBc.GetValue() * polarity;

            var (vbe, limited) = DeviceHelpers.PnLimitVoltage(vvbe, VoltageBaseEmitter, nF * vT, vbecrit);
            var (vbc, limited2) = DeviceHelpers.PnLimitVoltage(vvbc, VoltageBaseCollector, nR * vT, vbccrit);

            // calculate current deltas
            var delvbe = vbe - VoltageBaseEmitter;
            var delvbc = vbc - VoltageBaseCollector;
            var cchat = CurrentCollector + (Transconductance + OutputConductance) * delvbe -
                        (OutputConductance + ConductanceMu) * delvbc;
            var cbhat = CurrentBase + ConductancePi * delvbe + ConductanceMu * delvbc;
            var cc = CurrentCollector;
            var cb = CurrentBase;

            var reltol = context.SimulationParameters.RelativeTolerance;
            var abstol = context.SimulationParameters.AbsoluteTolerance;

            // request another iteration if not converged
            if (limited || limited2 ||
                !MathHelper.InTollerance(cchat, cc, abstol, reltol) ||
                !MathHelper.InTollerance(cbhat, cb, abstol, reltol))
                context.ReportNotConverged(this);

            // update voltages
            VoltageBaseEmitter = vbe;
            VoltageBaseCollector = vbc;
            VoltageCollectorEmitter = vbc - vbe;
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