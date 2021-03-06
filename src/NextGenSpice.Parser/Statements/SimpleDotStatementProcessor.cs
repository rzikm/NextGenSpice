using NextGenSpice.Parser.Utils;

namespace NextGenSpice.Parser.Statements
{
	/// <summary>Class for parsing arbitrary .[Keyword] statements including their parameters.</summary>
	/// <typeparam name="TParam"></typeparam>
	public abstract class SimpleDotStatementProcessor<TParam> : DotStatementProcessor
	{
		public SimpleDotStatementProcessor()
		{
			Mapper = new ParameterMapper<TParam>();
		}

		/// <summary>instance of ParameterMapper for this statements parameters</summary>
		protected ParameterMapper<TParam> Mapper { get; }

		/// <summary>Initializes mapper target (instance hodling the param values), including default parameters.</summary>
		protected abstract void InitMapper();


		/// <summary>Processes given statement.</summary>
		/// <param name="tokens"></param>
		protected override void DoProcess(Token[] tokens)
		{
			InitMapper();
			for (var i = 1; i < tokens.Length; i++)
				Mapper.Set(i - 1, tokens[i].GetNumericValue(Context.Errors));
			UseParam();
			Mapper.Target = default(TParam); // free memory;
		}

		/// <summary>Final action for processing the statement</summary>
		protected abstract void UseParam();
	}
}