using System.Collections.Generic;
using NextGenSpice.Core.Elements;
using NextGenSpice.Parser.Statements.Deferring;
using NextGenSpice.Parser.Statements.Models;
using NextGenSpice.Utils;

namespace NextGenSpice.Parser.Statements.Devices
{
    /// <summary>
    /// Class that handles diode element statements.
    /// </summary>
    public class DiodeStatementProcessor : ElementStatementProcessor
    {
        /// <summary>
        /// Class that handles diode element model statements.
        /// </summary>
        class DiodeModelStatementHandler : IModelStatementHandler
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
            /// Discriminator of model type.
            /// </summary>
            public string Discriminator => "D";

            /// <summary>
            /// Processes the .MODEL statement in given context.
            /// </summary>
            /// <param name="tokens"></param>
            /// <param name="context"></param>
            public void Process(Token[] tokens, ParsingContext context)
            {
                var name = tokens[1].Value;
                var symbolTableModel = context.SymbolTable.Models[DeviceType.Diode];
                
                //TODO: Should model names be unique across model types (Diode, PNP etc.)?
                if (symbolTableModel.ContainsKey(name))
                {
                    context.Errors.Add(tokens[1].ToErrorInfo($"There already exists model with name '{name} for this device type."));
                    return; // no additional processing required
                }
                
                mapper.Target = new DiodeModelParams();

                foreach (var token in Helper.Retokenize(tokens, context.Errors))
                {
                    // parameters are must be in key-value pairs <parameter name>=<value> (without whitespace)
                    var index = token.Value.IndexOf('=');

                    if (index <= 0 || index >= token.Value.Length - 1) // no '=' 
                    {
                        context.Errors.Add(token.ToErrorInfo($"Model parameters must be in form <parameter name>=<value>."));
                        continue;
                    }

                    var paramName = token.Value.Substring(0, index);

                    // check validity of the parameter name
                    if (!mapper.HasKey(paramName))
                    {
                        context.Errors.Add(token.ToErrorInfo($"Unknown model parameter name '{paramName}'."));
                    }

                    // reuse token instance for parsing the value part of the pair
                    token.LineColumn += index + 1; // modify offset to get correct error location.
                    token.Value = token.Value.Substring(index + 1);
                    
                    if (mapper.HasKey(paramName)) mapper.Set(paramName, token.GetNumericValue(context.Errors)); 
                }

                symbolTableModel[name] = mapper.Target;
                mapper.Target = null; // free memory
            }
        }

        /// <summary>
        /// Discriminator of the element type this processor can parse.
        /// </summary>
        public override char Discriminator => 'D';

        /// <summary>
        /// Processes given set of statements.
        /// </summary>
        /// <param name="tokens"></param>
        protected override void DoProcess(Token[] tokens)
        {
            if (tokens.Length != 4) // name, +N, -N, model
            {
                InvalidNumberOfArguments(tokens[0]);
            }

            var name = DeclareElement(tokens[0]);
            var nodes = GetNodeIndices(tokens, 1, 2);
            // cannot check for model existence yet, defer checking for model later


            if (Errors == 0)
            {
                var modelToken = tokens[3];
                var symbolTableModel = Context.SymbolTable.Models[DeviceType.Diode]; // make local variable to be captured inside lambda
                Context.DeferredStatements.Add(
                    new ModeledElementStatement<DiodeModelParams>(
                        (par, cb) => cb.AddElement(nodes, new DiodeElement(par, name)),
                        () => (DiodeModelParams)symbolTableModel.GetValueOrDefault(modelToken.Value), // deferred evaluation.
                        modelToken));
            }
        }

        /// <summary>
        /// Gets list of model statement handlers that are responsible to parsing respective models of this device.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<IModelStatementHandler> GetModelStatementHandlers()
        {
            return new IModelStatementHandler[] { new DiodeModelStatementHandler() };
        }
    }
}