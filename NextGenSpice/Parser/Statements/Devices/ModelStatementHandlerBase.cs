using NextGenSpice.Parser.Statements.Models;
using NextGenSpice.Utils;

namespace NextGenSpice.Parser.Statements.Devices
{
    /// <summary>
    ///     Base class implementing basic functionality of parsing SPICE .MODEL parameters.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ModelStatementHandlerBase<T> : IModelStatementHandler
    {
        /// <summary>
        ///     Mapper for mapping parsed parameters onto properties.
        /// </summary>
        protected abstract ParameterMapper<T> Mapper { get; }

        /// <summary>
        ///     Type of the device that handled models are for.
        /// </summary>
        protected abstract DeviceType DeviceType { get; }

        /// <summary>
        ///     Discriminator of handled model type.
        /// </summary>
        public abstract string Discriminator { get; }

        /// <summary>
        ///     Processes the .MODEL statement in given context.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="context"></param>
        public void Process(Token[] tokens, ParsingContext context)
        {
            var name = tokens[1].Value;
            var symbolTableModel = context.SymbolTable.Models[DeviceType];

            //TODO: Should model names be unique across model types (Diode, PNP etc.)?
            if (symbolTableModel.ContainsKey(name))
            {
                context.Errors.Add(tokens[1]
                    .ToErrorInfo($"There already exists model with name '{name} for this device type."));
                return; // no additional processing required
            }

            CreateModelParams();

            foreach (var token in Helper.Retokenize(tokens, context.Errors))
            {
                // parameters are must be in key-value pairs <parameter name>=<value> (without whitespace)
                var index = token.Value.IndexOf('=');

                if (index <= 0 || index >= token.Value.Length - 1) // no '=' 
                {
                    context.Errors.Add(
                        token.ToErrorInfo($"Model parameters must be in form <parameter name>=<value>."));
                    continue;
                }

                var paramName = token.Value.Substring(0, index);

                // check validity of the parameter name
                if (!Mapper.HasKey(paramName))
                    context.Errors.Add(token.ToErrorInfo($"Unknown model parameter name '{paramName}'."));

                // reuse token instance for parsing the value part of the pair
                token.LineColumn += index + 1; // modify offset to get correct error location.
                token.Value = token.Value.Substring(index + 1);

                if (Mapper.HasKey(paramName)) Mapper.Set(paramName, token.GetNumericValue(context.Errors));
            }

            symbolTableModel[name] = Mapper.Target;
            Mapper.Target = default(T); // free memory
        }

        /// <summary>
        ///     Creates new instance of parameter class for this device model.
        /// </summary>
        /// <returns></returns>
        protected abstract T CreateModelParams();
    }
}