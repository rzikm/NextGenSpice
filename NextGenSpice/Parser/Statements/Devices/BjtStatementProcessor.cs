using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Elements.Parameters;
using NextGenSpice.Parser.Statements.Deferring;
using NextGenSpice.Parser.Statements.Models;
using NextGenSpice.Utils;

namespace NextGenSpice.Parser.Statements.Devices
{
    /// <summary>Class that handles Homo-Junction Bipolar Transistor element statements.</summary>
    public class BjtStatementProcessor : ElementStatementProcessor
    {
        public BjtStatementProcessor()
        {
            MinArgs = 4;
            MaxArgs = 5;
        }

        /// <summary>Discriminator of the element type this processor can parse.</summary>
        public override char Discriminator => 'Q';

        /// <summary>Processes given set of statements.</summary>
        protected override void DoProcess()
        {
            var name = ElementName;
            var nodes = RawStatement.Length == 5
                ? GetNodeIndices(1, 3).Concat(new[] {0}).ToArray() // substrate node not specified.
                : GetNodeIndices(1, 4);

            // cannot check for model existence yet, defer checking for model later
            if (Errors == 0)
            {
                var modelToken = RawStatement.Last(); // capture

                Context.DeferredStatements.Add(
                    new ModeledElementDeferedStatement<BjtModelParams>(
                        (par, cb) => cb.AddElement(nodes, new BjtElement(par, name)), // deferred evaluation.
                        modelToken));
            }
        }

        /// <summary>Gets list of model statement handlers that are responsible to parsing respective models of this device.</summary>
        /// <returns></returns>
        public override IEnumerable<IModelStatementHandler> GetModelStatementHandlers()
        {
            return new IModelStatementHandler[]
                {new BjtModelStatementHandler(true), new BjtModelStatementHandler(false),};
        }


        /// <summary>Class that handles Homo-Junction Bipolar Trannsistor element model statements.</summary>
        private class BjtModelStatementHandler : ModelStatementHandlerBase<BjtModelParams>
        {
            private readonly bool isPnp;

            public BjtModelStatementHandler(bool isPnp)
            {
                this.isPnp = isPnp;
                Discriminator = isPnp ? "PNP" : "NPN";
                var mapper = new ParameterMapper<BjtModelParams>();

                mapper.Map(x => x.SaturationCurrent, "IS");

                mapper.Map(x => x.ForwardBeta, "BF");
                mapper.Map(x => x.ForwardEmissionCoefficient, "NF");
                mapper.Map(x => x.ForwardEarlyVoltage, "VAF");
                mapper.Map(x => x.ForwardCurrentCorner, "IKF");
                mapper.Map(x => x.EmitterSaturationCurrent, "ISE");
                mapper.Map(x => x.EmitterSaturationCoefficient, "NE");
                mapper.Map(x => x.ReverseBeta, "BR");
                mapper.Map(x => x.ReverseEmissionCoefficient, "NR");
                mapper.Map(x => x.ReverseEarlyVoltage, "VAR");
                mapper.Map(x => x.ReverseCurrentCorner, "IKR");
                mapper.Map(x => x.CollectorSaturationCurrent, "ISC");
                mapper.Map(x => x.CollectorSaturationCoefficient, "NC");
                mapper.Map(x => x.BaseResistance, "RB");
                mapper.Map(x => x.CurrentBaseResistanceMidpoint, "IRB");

                mapper.Map(x => x.MinimumBaseResistance, "RBM");
                mapper.Map(x => x.EmitterResistance, "RE");
                mapper.Map(x => x.CollectorResistance, "RC");
                mapper.Map(x => x.EmitterCapacitance, "CJE");
                mapper.Map(x => x.EmitterPotential, "VJE");
                mapper.Map(x => x.EmitterExponentialFactor, "MJE");
                mapper.Map(x => x.ForwardTransitTime, "TF");
                mapper.Map(x => x.CurrentBaseResistanceMidpoint, "XTF");
                mapper.Map(x => x.VbcDependenceOfTransitTime, "VTF");
                mapper.Map(x => x.ForwardTransitHighCurrent, "ITF");
                //                mapper.Map(x => x., "PTF");
                mapper.Map(x => x.CollectorCapacitance, "CJC");
                mapper.Map(x => x.CollectorPotential, "VJC");
                mapper.Map(x => x.CollectorExponentialFactor, "MJC");
                mapper.Map(x => x.CurrentBaseResistanceMidpoint, "XCJC");
                mapper.Map(x => x.ReverseTransitTime, "TR");
                mapper.Map(x => x.SubstrateCapacitance, "CJS");
                mapper.Map(x => x.SubstrateExponentialFactor, "MJS");
                mapper.Map(x => x.TemperatureExponentBeta, "XTB");
                mapper.Map(x => x.EnergyGap, "EG");
                mapper.Map(x => x.TemperatureExponentSaturationCurrent, "XTI");
                mapper.Map(x => x.FlickerNoiseCoeffitient, "KF");
                mapper.Map(x => x.FlickerNoiseExponent, "AF");
                mapper.Map(x => x.ForwardBiasDepletionCoefficient, "FC");
                mapper.Map(x => x.NominalTemperature, "TNOM");

                Mapper = mapper;
            }

            /// <summary>Mapper for mapping parsed parameters onto properties.</summary>
            protected override ParameterMapper<BjtModelParams> Mapper { get; }

            /// <summary>Discriminator of handled model type.</summary>
            public override string Discriminator { get; }

            /// <summary>Creates new instance of parameter class for this device model.</summary>
            /// <returns></returns>
            protected override BjtModelParams CreateDefaultModel()
            {
                return new BjtModelParams() {IsPnp = isPnp};
            }
        }
    }
}