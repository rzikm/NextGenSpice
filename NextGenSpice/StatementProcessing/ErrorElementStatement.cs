using System;
using System.Collections.Generic;
using NextGenSpice.Core.Circuit;

namespace NextGenSpice
{
    public class ErrorElementStatement : ElementStatement
    {

        private readonly List<ErrorInfo> errorInfos;

        public ErrorElementStatement(List<ErrorInfo> errorInfos)
        {
            this.errorInfos = errorInfos;
        }

        public override bool CanApply(ParsingContext ctx)
        {
            return false;
        }

        public override IEnumerable<ErrorInfo> GetErrors()
        {
            return errorInfos;
        }

        public override void Apply(ParsingContext ctx)
        {
            throw new InvalidOperationException();
        }
    }
}