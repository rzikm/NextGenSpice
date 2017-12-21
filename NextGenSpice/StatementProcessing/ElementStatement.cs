using System.Collections.Generic;
using NextGenSpice.Core.Circuit;

namespace NextGenSpice
{
    public abstract class ElementStatement : DeferredStatement
    {
    }

    public abstract class DeferredStatement
    {
        public abstract bool CanApply(ParsingContext context);
        public abstract IEnumerable<ErrorInfo> GetErrors();
        public abstract void Apply(ParsingContext context);
    }
}