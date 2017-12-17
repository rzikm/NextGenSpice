using System.Collections.Generic;
using NextGenSpice.Core.Circuit;

namespace NextGenSpice
{
    public abstract class ElementStatement
    {
        public abstract bool CanApply();
        public abstract IEnumerable<ErrorInfo> GetErrors();
        public abstract void Apply(CircuitBuilder builder);
    }
}