using System.Linq;
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

            if (context.SymbolTable.TryGetModel<T>(name, out _))
            {
                context.Errors.Add(tokens[1]
                    .ToErrorInfo(SpiceParserError.ModelAlreadyExists));
                return; // no additional processing required
            }

            Mapper.Target = CreateDefaultModel();
            foreach (var token in tokens.Skip(3)) // skip .MODEL <model name> <discriminator> tokens
            {
                // parameters are must be in key-value pairs <parameter name>=<value> (without whitespace)
                var index = token.Value.IndexOf('=');

                if (index <= 0 || index >= token.Value.Length - 1) // no '=' 
                {
                    context.Errors.Add(
                        token.ToErrorInfo(SpiceParserError.InvalidModelParameter));
                    continue;
                }

                var paramName = token.Value.Substring(0, index);

                // check validity of the parameter name
                if (!Mapper.HasKey(paramName))
                    context.Errors.Add(token.ToErrorInfo(SpiceParserError.UnknownParameter, paramName));

                // reuse token instance for parsing the value part of the pair
                token.LineColumn += index + 1; // modify offset to get correct error location.
                token.Value = token.Value.Substring(index + 1);

                if (Mapper.HasKey(paramName)) Mapper.Set(paramName, token.GetNumericValue(context.Errors));
            }

            context.SymbolTable.AddModel(Mapper.Target, name);
            Mapper.Target = default(T); // free memory
        }

        /// <summary>
        ///     Creates new instance of model parameter class with default values.
        /// </summary>
        /// <returns></returns>
        object IModelStatementHandler.CreateDefaultModel()
        {
            return CreateDefaultModel();
        }

        /// <summary>
        ///     Creates new instance of parameter class for this device model.
        /// </summary>
        /// <returns></returns>
        protected abstract T CreateDefaultModel();
    }
}