using System.Collections.Generic;
using NextGenSpice.Utils;

namespace NextGenSpice.Parser.Statements.Deferring
{
    /// <summary>
    ///     Base class for all statements that cannot be evaluated immediately (e.g. .model statement might not be processed
    ///     yet)
    /// </summary>
    public abstract class DeferredStatement
    {
        /// <summary>
        ///     Returns true if all prerequisites for the statements have been fulfilled and statement is ready to be applied.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract bool CanApply(ParsingContext context);

        /// <summary>
        ///     Returns set of errors due to which this stetement cannot be processed.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<ErrorInfo> GetErrors();

        /// <summary>
        ///     Applies the statement in the given context.
        /// </summary>
        /// <param name="context"></param>
        public abstract void Apply(ParsingContext context);
    }
}