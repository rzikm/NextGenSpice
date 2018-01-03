using System.Collections.Generic;
using NextGenSpice.Core.Circuit;

namespace NextGenSpice
{
    public abstract class ElementStatement : DeferredStatement
    {
        protected List<ErrorInfo> Errors { get; }

        public ElementStatement()
        {
            Errors = new List<ErrorInfo>();
        }
    }
}