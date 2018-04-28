using System.Collections.Generic;
using System.Linq;

using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Devices.Parameters;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;
using NextGenSpice.Numerics.Equations;
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
        private class ShockleyDiodeModelStatementHandler : ModelStatementHandlerBase<ShockleyDiodeParams>
        {
            public ShockleyDiodeModelStatementHandler()
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
        }

        public class LargeSignalShockleyDiode : TwoTerminalLargeSignalDevice<ShockleyDiode>
        {
            public LargeSignalShockleyDiode(ShockleyDiode definitionDevice) : base(definitionDevice)
            {
            }

            public override void Initialize(IEquationSystemAdapter adapter, ISimulationContext context)
            {
                throw new System.NotImplementedException();
            }

            public override void ApplyModelValues(ISimulationContext context)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}