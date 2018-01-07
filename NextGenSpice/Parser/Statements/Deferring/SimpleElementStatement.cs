using System;
using System.Collections.Generic;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Utils;

namespace NextGenSpice.Parser.Statements.Deferring
{
    /// <summary>
    /// Class representing element statement for simple elements (without a model)
    /// </summary>
    public class SimpleElementStatement : DeferredStatement
    {
        private readonly Action<CircuitBuilder> builderFunc;

        public SimpleElementStatement(Action<CircuitBuilder> builderFunc)
        {
            this.builderFunc = builderFunc;
        }

        /// <summary>
        /// Returns true if all prerequisites for the statements have been fulfilled and statement is ready to be applied.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool CanApply(ParsingContext ctx)
        {
            return true;
        }

        /// <summary>
        /// Calling this function always results in InvalidOperationException as this statement can always be processed.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<ErrorInfo> GetErrors()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Applies the statement in the given context.
        /// </summary>
        /// <param name="context"></param>
        public override void Apply(ParsingContext ctx)
        {
            builderFunc(ctx.CircuitBuilder);
        }
    }
}