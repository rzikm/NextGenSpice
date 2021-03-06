using System;
using System.Collections.Generic;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Parser.Utils;

namespace NextGenSpice.Parser.Statements.Deferring
{
	/// <summary>Class representing device statement for simple devices (without a model)</summary>
	public class SimpleDeviceDeferredStatement : DeferredStatement
	{
		private readonly Action<CircuitBuilder> builderFunc;

		public SimpleDeviceDeferredStatement(ParsingScope scope, Action<CircuitBuilder> builderFunc) : base(scope)
		{
			this.builderFunc = builderFunc;
		}

		/// <summary>Returns true if all prerequisites for the statements have been fulfilled and statement is ready to be applied.</summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override bool CanApply()
		{
			return true;
		}

		/// <summary>Calling this function always results in InvalidOperationException as this statement can always be processed.</summary>
		/// <returns></returns>
		public override IEnumerable<SpiceParserError> GetErrors()
		{
			throw new InvalidOperationException();
		}

		/// <summary>Applies the statement in the given context.</summary>
		/// <param name="context"></param>
		public override void Apply()
		{
			base.Apply();
			builderFunc(Scope.CircuitBuilder);
		}
	}
}