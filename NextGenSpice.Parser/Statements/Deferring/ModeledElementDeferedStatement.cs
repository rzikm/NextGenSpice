﻿using System;
using System.Collections.Generic;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Parser.Utils;

namespace NextGenSpice.Parser.Statements.Deferring
{
    /// <summary>Statement responsible for adding models into the circuit definition.</summary>
    /// <typeparam name="TModel"></typeparam>
    public class ModeledDeviceDeferedStatement<TModel> : DeferredStatement
    {
        private readonly Action<TModel, CircuitBuilder> builderFunc;
        private readonly Token modelNameToken;

        private TModel model;

        public ModeledDeviceDeferedStatement(Action<TModel, CircuitBuilder> builderFunc, Token modelNameToken)
        {
            this.builderFunc = builderFunc;
            this.modelNameToken = modelNameToken;
        }

        /// <summary>Returns true if all prerequisites for the statements have been fulfilled and statement is ready to be applied.</summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool CanApply(ParsingContext context)
        {
            return context.SymbolTable.TryGetModel(modelNameToken.Value, out model);
        }

        /// <summary>Returns set of errors due to which this stetement cannot be processed.</summary>
        /// <returns></returns>
        public override IEnumerable<Utils.SpiceParserError> GetErrors()
        {
            return new[] {modelNameToken.ToError(SpiceParserErrorCode.NoSuchModel)};
        }

        /// <summary>Applies the statement in the given context.</summary>
        /// <param name="context"></param>
        public override void Apply(ParsingContext context)
        {
            builderFunc(model, context.CircuitBuilder);
        }
    }
}