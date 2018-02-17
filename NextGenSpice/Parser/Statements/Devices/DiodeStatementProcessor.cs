using System.Collections.Generic;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Elements.Parameters;
using NextGenSpice.Parser.Statements.Deferring;
using NextGenSpice.Parser.Statements.Models;
using NextGenSpice.Utils;

namespace NextGenSpice.Parser.Statements.Devices
{
    /// <summary>
    ///     Class that handles diode element statements.
    /// </summary>
    public class DiodeStatementProcessor : ElementStatementProcessor
    {
        /// <summary>
        ///     Discriminator of the element type this processor can parse.
        /// </summary>
        public override char Discriminator => 'D';

        /// <summary>
        ///     Processes given set of statements.
        /// </summary>
        /// <param name="tokens"></param>
        protected override void DoProcess(Token[] tokens)
        {
            if (tokens.Length != 4) // name, +N, -N, model
                InvalidNumberOfArguments(tokens[0]);

            var name = DeclareElement(tokens[0]);
            var nodes = GetNodeIndices(tokens, 1, 2);
            // cannot check for model existence yet, defer checking for model later

            if (Errors == 0)
            {
                var modelToken = tokens[3];
                Context.DeferredStatements.Add(
                    new ModeledElementDeferedStatement<DiodeModelParams>(
                        (par, cb) => cb.AddElement(nodes, new DiodeElement(par, name)), modelToken));
            }
        }

        /// <summary>
        ///     Gets list of model statement handlers that are responsible to parsing respective models of this device.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<IModelStatementHandler> GetModelStatementHandlers()
        {
            return new IModelStatementHandler[] {new DiodeModelStatementHandler()};
        }

        /// <summary>
        ///     Class that handles diode element model statements.
        /// </summary>
        private class DiodeModelStatementHandler : ModelStatementHandlerBase<DiodeModelParams>,
            IModelStatementHandler
        {
            private readonly ParameterMapper<DiodeModelParams> mapper;

            public DiodeModelStatementHandler()
            {
                mapper = new ParameterMapper<DiodeModelParams>();
                mapper.Map(p => p.SaturationCurrent, "IS");
                mapper.Map(p => p.SeriesResistance, "RS");
                mapper.Map(p => p.EmissionCoefficient, "N");
                mapper.Map(p => p.TransitTime, "TT");
                mapper.Map(p => p.JunctionCapacitance, "CJO");
                mapper.Map(p => p.JunctionPotential, "VJ");
                mapper.Map(p => p.JunctionGradingCoefficient, "M");
                mapper.Map(p => p.ActivationEnergy, "EG");
                mapper.Map(p => p.SaturationCurrentTemperatureExponent, "XTI");
                mapper.Map(p => p.FlickerNoiseCoefficient, "KF");
                mapper.Map(p => p.FlickerNoiseExponent, "AF");
                mapper.Map(p => p.ForwardBiasDepletionCapacitanceCoefficient, "FC");
                mapper.Map(p => p.ReverseBreakdownVoltage, "BV");
                mapper.Map(p => p.ReverseBreakdownCurrent, "IBV");
            }

            /// <summary>
            ///     Mapper for mapping parsed parameters onto properties.
            /// </summary>
            protected override ParameterMapper<DiodeModelParams> Mapper => mapper;

            /// <summary>
            ///     Discriminator of handled model type.
            /// </summary>
            public override string Discriminator => "D";

            /// <summary>
            ///     Creates new instance of parameter class for this device model.
            /// </summary>
            /// <returns></returns>
            protected override DiodeModelParams CreateDefaultModel()
            {
                return new DiodeModelParams();
            }
        }
    }
}