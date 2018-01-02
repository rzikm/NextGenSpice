using System.Collections.Generic;

namespace NextGenSpice
{
    public abstract class DeferredStatement
    {
        public abstract bool CanApply(ParsingContext context);
        public abstract IEnumerable<ErrorInfo> GetErrors();
        public abstract void Apply(ParsingContext context);
    }
}