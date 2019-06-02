using System.Collections.Generic;
using NextGenSpice.Parser.Utils;

namespace NextGenSpice.Parser.Statements.Deferring
{
	/// <summary>
	///   Base class for all statements that cannot be evaluated immediately (e.g. .model statement might not be
	///   processed yet)
	/// </summary>
	public abstract class DeferredStatement
	{
		protected ParsingScope Scope;

		protected DeferredStatement(ParsingScope scope)
		{
			Scope = scope;
		}

		/// <summary>Returns true if all prerequisites for the statements have been fulfilled and statement is ready to be applied.</summary>
		/// <returns></returns>
		public abstract bool CanApply();

		/// <summary>Returns set of errors due to which this stetement cannot be processed.</summary>
		/// <returns></returns>
		public abstract IEnumerable<SpiceParserError> GetErrors();

		/// <summary>Applies the statement in the given context.</summary>
		public virtual void Apply()
		{
			Scope.Statements.Remove(this);
		}
	}
}