using NextGenSpice.Utils;

namespace NextGenSpice.Parser.Statements
{
    /// <summary>
    /// Class for parsing arbitrary .[Keyword] statements including their parameters.
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    public abstract class SimpleStatementProcessor<TParam> : StatementProcessor
    {
        /// <summary>
        /// instance of ParameterMapper for this statements parameters
        /// </summary>
        protected ParameterMapper<TParam> Mapper { get; }

        public SimpleStatementProcessor()
        {
            Mapper = new ParameterMapper<TParam>();
        }

        /// <summary>
        /// Initializes mapper target (instance hodling the param values), including default parameters.
        /// </summary>
        protected abstract void InitMapper();


        /// <summary>
        /// Processes given statement.
        /// </summary>
        /// <param name="tokens"></param>
        protected override void DoProcess(Token[] tokens)
        {
            InitMapper();
            for (int i = 1; i < tokens.Length; i++)
            {
                Mapper.Set(i, tokens[i].GetNumericValue(Context.Errors));
            }
            UseParam();
        }

        /// <summary>
        /// Final action for processing the statement
        /// </summary>
        protected abstract void UseParam();
    }
}