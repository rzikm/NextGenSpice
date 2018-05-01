﻿using System;
using System.Collections.Generic;
using System.Linq;

using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Devices.Parameters;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Devices;
using NextGenSpice.LargeSignal.Stamping;
using NextGenSpice.Numerics.Equations;
using NextGenSpice.Parser;
using NextGenSpice.Parser.Statements.Deferring;
using NextGenSpice.Parser.Statements.Devices;
using NextGenSpice.Parser.Statements.Models;
using NextGenSpice.Parser.Utils;


namespace SandboxRunner
{
    public class DiodeImplExample
    {
        public class ShockleyDiodeParams
        {
            public double SaturationCurrent { get; set; } = 1e-14;
            public double ThermalVoltage { get; set; } = 25.8563e-3;
            public double IdealityCoefficient { get; set; } = 1;
        }

        // requires .Core.Devices namespace
        public class ShockleyDiode : TwoTerminalCircuitDevice
        {
            public ShockleyDiodeParams Param { get; set; }

            public ShockleyDiode(ShockleyDiodeParams param, object tag = null) : base(tag)
            {
                Param = param;
            }
        }

        // requires NextGenSpice.Parser.Statements.Devices; namespace
        private class ShockleyDiodeModelHandler : DeviceModelHandlerBase<ShockleyDiodeParams>
        {
            public ShockleyDiodeModelHandler()
            {
                Map(p => p.SaturationCurrent, "IS");
                Map(p => p.ThermalVoltage, "VT");
                Map(p => p.IdealityCoefficient, "N");
            }

            public override string Discriminator => "SHOCKLEY";

            protected override ShockleyDiodeParams CreateDefaultModel()
            {
                return new ShockleyDiodeParams();
            }
        }


        public class ShockleyDiodeStatementProcessor : DeviceStatementProcessor
        {
            public override char Discriminator => 'S';

            public ShockleyDiodeStatementProcessor()
            {
                MinArgs = MaxArgs = 3;
            }

            protected override void DoProcess()
            {
                var name = DeviceName; // use local variable for capture
                var nodes = GetNodeIndices(1, 2);
                // cannot check for model existence yet, defer checking for model later

                if (Errors == 0)
                {
                    var modelToken = RawStatement[3];
                    Context.DeferredStatements.Add(
                        new ModeledDeviceDeferedStatement<ShockleyDiodeParams>(Context.CurrentScope,
                            (par, cb) => cb.AddDevice(nodes, new ShockleyDiode(par, name)), modelToken));
                }
            }

            public override IEnumerable<IDeviceModelHandler> GetModelStatementHandlers()
            {
                return new[] { new ShockleyDiodeModelHandler() };
            }
        }


        public static void RegisteringTheHandler()
        {
            var parser =  SpiceNetlistParser.WithDefaults();
            parser.RegisterDevice(new ShockleyDiodeStatementProcessor());

            // ...
        }

        // requires NextGenSpice.Numerics.Equations namespace
        public class LargeSignalShockleyDiode : TwoTerminalLargeSignalDevice<ShockleyDiode>
        {
            // classes encapsulating the work with equation system coefficient proxies
            // requires NextGenSpice.LargeSignal.Stamping namespace
            private VoltageProxy voltage; // used to get voltage across the diode

            // stamping equivalent circuit model
            private CurrentStamper currentStamper;
            private ConductanceStamper conductanceStamper;

            public LargeSignalShockleyDiode(ShockleyDiode definitionDevice) : base(definitionDevice)
            {
                voltage = new VoltageProxy();
                currentStamper = new CurrentStamper();
                conductanceStamper = new ConductanceStamper();
            }

            public override void Initialize(IEquationSystemAdapter adapter, ISimulationContext context)
            {
                // get proxies
                voltage.Register(adapter, Anode, Cathode);
                currentStamper.Register(adapter, Anode, Cathode);
                conductanceStamper.Register(adapter, Anode, Cathode);
            }

            public override void ApplyModelValues(ISimulationContext context)
            {
                var Is = DefinitionDevice.Param.SaturationCurrent;
                var Vt = DefinitionDevice.Param.ThermalVoltage;
                var n = DefinitionDevice.Param.IdealityCoefficient;

                var Vd = voltage.GetValue();
                // calculates current through the diode and it's derivative
                DeviceHelpers.PnJunction(Is, Vd, Vt * n, out var Id, out var Geq);

                // stamp the equivalent circuit
                var Ieq = Id - Geq * Vd;
                conductanceStamper.Stamp(Geq);
                currentStamper.Stamp(Ieq);
            }

            /// <summary>This method is called each time an equation is solved.</summary>
            /// <param name="context">Context of current simulation.</param>
            public override void OnEquationSolution(ISimulationContext context)
            {
                
            }
        }
    }
}