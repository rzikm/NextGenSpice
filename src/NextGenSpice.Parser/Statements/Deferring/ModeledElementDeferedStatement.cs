using System;
using System.Collections.Generic;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Parser.Utils;

namespace NextGenSpice.Parser.Statements.Deferring
{
	/// <summary>Statement responsible for adding models into the circuit definition.</summary>
	/// <typeparam name="TModel"></typeparam>
	public class ModeledDeviceDeferedStatement<TModel> : DeferredStatement
	{
		private readonly Action<TModel, CircuitBuilder> addFunc;
		private readonly Token modelNameToken;

		private TModel model;

		public ModeledDeviceDeferedStatement(ParsingScope scope, Action<TModel, CircuitBuilder> addFunc,
			Token modelNameToken) : base(scope)
		{
			this.addFunc = addFunc;
			this.modelNameToken = modelNameToken;
		}

		/// <summary>Returns true if all prerequisites for the statements have been fulfilled and statement is ready to be applied.</summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override bool CanApply()
		{
			return Scope.SymbolTable.TryGetModel(modelNameToken.Value, out model);
		}

		/// <summary>Returns set of errors due to which this stetement cannot be processed.</summary>
		/// <returns></returns>
		public override IEnumerable<SpiceParserError> GetErrors()
		{
			return new[] {modelNameToken.ToError(SpiceParserErrorCode.NoSuchModel)};
		}

		/// <summary>Applies the statement in the given context.</summary>
		/// <param name="context"></param>
		public override void Apply()
		{
			base.Apply();
			addFunc(model, Scope.CircuitBuilder);
		}
	}
}