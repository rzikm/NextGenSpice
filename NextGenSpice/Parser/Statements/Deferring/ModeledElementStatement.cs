using System;
using System.Collections.Generic;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Utils;

namespace NextGenSpice.Parser.Statements.Deferring
{
    /// <summary>
    ///     Statement responsible for adding models into the circuit definition.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class ModeledElementStatement<TModel> : DeferredStatement
    {
        private readonly Action<TModel, CircuitBuilder> builderFunc;
        private readonly Func<TModel> modelFactory;
        private readonly Token token;

        private TModel model;

        public ModeledElementStatement(Action<TModel, CircuitBuilder> builderFunc, Func<TModel> modelFactory,
            Token token)
        {
            this.builderFunc = builderFunc;
            this.modelFactory = modelFactory;
            this.token = token;
        }

        /// <summary>
        ///     Returns true if all prerequisites for the statements have been fulfilled and statement is ready to be applied.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool CanApply(ParsingContext context)
        {
            return (model = modelFactory()) != null;
        }

        /// <summary>
        ///     Returns set of errors due to which this stetement cannot be processed.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<ErrorInfo> GetErrors()
        {
            return new[] {token.ToErrorInfo($"There is no model named '{token.Value}' for this device type.")};
        }

        /// <summary>
        ///     Applies the statement in the given context.
        /// </summary>
        /// <param name="context"></param>
        public override void Apply(ParsingContext context)
        {
            builderFunc(model, context.CircuitBuilder);
        }
    }
}