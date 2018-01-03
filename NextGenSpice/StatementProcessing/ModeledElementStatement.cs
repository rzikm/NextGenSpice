using System;
using System.Collections.Generic;
using NextGenSpice.Core.Circuit;

namespace NextGenSpice
{
    public class ModeledElementStatement<TModel> : ElementStatement
    {
        private readonly Action<TModel, CircuitBuilder> builderFunc;
        private readonly Func<TModel> modelFactory;
        private readonly Token token;

        private TModel model;

        public ModeledElementStatement(Action<TModel, CircuitBuilder> builderFunc, Func<TModel> modelFactory, Token token)
        {
            this.builderFunc = builderFunc;
            this.modelFactory = modelFactory;
            this.token = token;
        }

        public override bool CanApply(ParsingContext context)
        {
            return (model = modelFactory()) == null;
        }

        public override IEnumerable<ErrorInfo> GetErrors()
        {
            return new[] { token.ToErrorInfo($"There is no model named '{token.Value}' for this device type.") };
        }

        public override void Apply(ParsingContext context)
        {
            builderFunc(model, context.CircuitBuilder);
        }
    }
}