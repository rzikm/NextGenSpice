using System;
using System.Collections.Generic;
using NextGenSpice.Core.Circuit;

namespace NextGenSpice
{
    public class SimpleElementStatement : ElementStatement
    {
        private readonly Action<CircuitBuilder> builderFunc;

        public SimpleElementStatement(Action<CircuitBuilder> builderFunc)
        {
            this.builderFunc = builderFunc;
        }

        public override bool CanApply()
        {
            return true;
        }

        public override IEnumerable<ErrorInfo> GetErrors()
        {
            throw new InvalidOperationException();
        }

        public override void Apply(CircuitBuilder builder)
        {
            builderFunc(builder);
        }
    }
}