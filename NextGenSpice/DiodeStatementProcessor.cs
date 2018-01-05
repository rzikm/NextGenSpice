using System.Collections.Generic;
using NextGenSpice.Core.Elements;

namespace NextGenSpice
{
    public class DiodeStatementProcessor : ElementStatementProcessor
    {
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
                mapper.Map(p => p.JunctionCapacitance, "CJ0");
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

            public string Discriminator => "D";
            public void Process(Token[] tokens, ParsingContext context)
            {
                var name = tokens[1].Value;
                var symbolTableModel = context.SymbolTable.Models[ModelType.Diode];

                if (symbolTableModel.ContainsKey(name))
                {
                    context.Errors.Add(tokens[1].ToErrorInfo($"There already exists model with name '{name} for this device type."));
                    return;
                }
                
                mapper.Target = new DiodeModelParams();

                foreach (var token in Helper.Retokenize(tokens, context.Errors))
                {
                    var index = token.Value.IndexOf('=');

                    if (index <= 0 || index >= token.Value.Length - 1)
                    {
                        context.Errors.Add(token.ToErrorInfo($"Model parameters must be in form <parameter name>=<value>."));
                        continue;
                    }

                    var paramName = token.Value.Substring(0, index);

                    if (!mapper.HasKey(paramName))
                    {
                        context.Errors.Add(token.ToErrorInfo($"Unknown model parameter name '{paramName}'."));
                    }
                    token.LineColumn += index + 1;
                    token.Value = token.Value.Substring(index + 1);
                    
                    if (mapper.HasKey(paramName)) mapper.Set(paramName, token.GetNumericValue(context.Errors));
                }

                symbolTableModel[name] = mapper.Target;
            }
        }

        public override char Discriminator => 'D';
        protected override void DoProcess(Token[] tokens)
        {
            if (tokens.Length != 4)
            {
                InvalidNumberOfArguments(tokens[0]);
            }

            var name = DeclareElement(tokens[0]);
            var nodes = GetNodeIndices(tokens, 1, 2);


            if (Errors == 0)
            {
                var modelToken = tokens[3];
                var symbolTableModel = Context.SymbolTable.Models[ModelType.Diode];
                Context.DeferredStatements.Add(
                    new ModeledElementStatement<DiodeModelParams>(
                        (par, cb) => cb.AddElement(nodes, new DiodeElement(par, name)),
                        () => (DiodeModelParams)symbolTableModel.GetValueOrDefault(modelToken.Value),
                        modelToken));
            }
        }

        public override IEnumerable<IModelStatementHandler> GetModelStatementHandlers()
        {
            return new IModelStatementHandler[] { new DiodeModelStatementHandler() };
        }
    }
}