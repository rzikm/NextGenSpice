using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Devices.Parameters;
using NextGenSpice.Parser.Statements.Deferring;
using NextGenSpice.Parser.Statements.Models;
using NextGenSpice.Parser.Utils;

namespace NextGenSpice.Parser.Statements.Devices
{
    /// <summary>Class that handles Homo-Junction Bipolar Transistor device statements.</summary>
    public class BjtStatementProcessor : DeviceStatementProcessor
    {
        public BjtStatementProcessor()
        {
            MinArgs = 4;
            MaxArgs = 5;
        }

        /// <summary>Discriminator of the device type this processor can parse.</summary>
        public override char Discriminator => 'Q';

        /// <summary>Processes given set of statements.</summary>
        protected override void DoProcess()
        {
            var name = DeviceName;
            var nodes = RawStatement.Length == 5
                ? GetNodeIds(1, 3).Concat(new[] {0}).ToArray() // substrate node not specified.
                : GetNodeIds(1, 4);

            // cannot check for model existence yet, defer checking for model later
            if (Errors == 0)
            {
                var modelToken = RawStatement.Last(); // capture

                Context.DeferredStatements.Add(
                    new ModeledDeviceDeferedStatement<BjtParams>(Context.CurrentScope,
                        (par, cb) => cb.AddDevice(nodes, new Bjt(par, name)), // deferred evaluation.
                        modelToken));
            }
        }

        /// <summary>Gets list of model statement handlers that are responsible to parsing respective models of this device.</summary>
        /// <returns></returns>
        public override IEnumerable<IDeviceModelHandler> GetModelStatementHandlers()
        {
            return new IDeviceModelHandler[]
                {new BjtModelHandler(true), new BjtModelHandler(false),};
        }


        /// <summary>Class that handles Homo-Junction Bipolar Trannsistor device model statements.</summary>
        private class BjtModelHandler : DeviceModelHandlerBase<BjtParams>
        {
            private readonly bool isPnp;

            public BjtModelHandler(bool isPnp)
            {
                this.isPnp = isPnp;
                Discriminator = isPnp ? "PNP" : "NPN";

                Map(x => x.SaturationCurrent, "IS");

                Map(x => x.ForwardBeta, "BF");
                Map(x => x.ForwardEmissionCoefficient, "NF");
                Map(x => x.ForwardEarlyVoltage, "VAF");
                Map(x => x.ForwardCurrentCorner, "IKF");
                Map(x => x.EmitterSaturationCurrent, "ISE");
                Map(x => x.EmitterSaturationCoefficient, "NE");
                Map(x => x.ReverseBeta, "BR");
                Map(x => x.ReverseEmissionCoefficient, "NR");
                Map(x => x.ReverseEarlyVoltage, "VAR");
                Map(x => x.ReverseCurrentCorner, "IKR");
                Map(x => x.CollectorSaturationCurrent, "ISC");
                Map(x => x.CollectorSaturationCoefficient, "NC");
                Map(x => x.BaseResistance, "RB");
                Map(x => x.CurrentBaseResistanceMidpoint, "IRB");

                Map(x => x.MinimumBaseResistance, "RBM");
                Map(x => x.EmitterResistance, "RE");
                Map(x => x.CollectorResistance, "RC");
                Map(x => x.EmitterCapacitance, "CJE");
                Map(x => x.EmitterPotential, "VJE");
                Map(x => x.EmitterExponentialFactor, "MJE");
                Map(x => x.ForwardTransitTime, "TF");
                Map(x => x.CurrentBaseResistanceMidpoint, "XTF");
                Map(x => x.VbcDependenceOfTransitTime, "VTF");
                Map(x => x.ForwardTransitHighCurrent, "ITF");

                Map(x => x.CollectorCapacitance, "CJC");
                Map(x => x.CollectorPotential, "VJC");
                Map(x => x.CollectorExponentialFactor, "MJC");
                Map(x => x.CurrentBaseResistanceMidpoint, "XCJC");
                Map(x => x.ReverseTransitTime, "TR");
                Map(x => x.SubstrateCapacitance, "CJS");
                Map(x => x.SubstratePotential, "VJS");
                Map(x => x.SubstrateExponentialFactor, "MJS");
                Map(x => x.TemperatureExponentBeta, "XTB");
                Map(x => x.EnergyGap, "EG");
                Map(x => x.TemperatureExponentSaturationCurrent, "XTI");
                Map(x => x.FlickerNoiseCoeffitient, "KF");
                Map(x => x.FlickerNoiseExponent, "AF");
                Map(x => x.ForwardBiasDepletionCoefficient, "FC");
                Map(x => x.NominalTemperature, "TNOM");
            }

            /// <summary>Discriminator of handled model type.</summary>
            public override string Discriminator { get; }

            /// <summary>Creates new instance of parameter class for this device model.</summary>
            /// <returns></returns>
            protected override BjtParams CreateDefaultModel()
            {
                return new BjtParams() {IsPnp = isPnp};
            }
        }
    }
}