using System;

namespace NextGenSpice
{
    /// <summary>
    /// Main base class for implementing handlers for .[keyword] statements.
    /// </summary>
    public abstract class StatementProcessor : IStatementProcessor
    {
        public StatementProcessor()
        {
            MaxArgs = Int32.MaxValue;
        }

        /// <summary>
        /// Statement discriminator, that this class can handle.
        /// </summary>
        public abstract string Discriminator { get; }

        /// <summary>
        /// Instance of ParsingContext corresponding to currently parsed input file.
        /// </summary>
        protected ParsingContext Context { get; private set; }

        /// <summary>
        /// Processes the statement.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="ctx"></param>
        public void Process(Token[] tokens, ParsingContext ctx)
        {
            Context = ctx;

            var firstToken = tokens[0];
            if (tokens.Length - 1 < MinArgs)
                ctx.Errors.Add(firstToken.ToErrorInfo($"Too few arguments for {firstToken.Value} statement."));
            
            if (tokens.Length - 1 > MaxArgs)
                ctx.Errors.Add(firstToken.ToErrorInfo($"Too many arguments for {firstToken.Value} statement."));
            
            DoProcess(tokens);

            Context = null;
        }

        /// <summary>
        /// Processes given statement.
        /// </summary>
        /// <param name="tokens">All tokens of the statement.</param>
        protected abstract void DoProcess(Token[] tokens);

        /// <summary>
        /// Minimum number of arguments for statement handled by this class.
        /// </summary>
        protected int MinArgs { get; set; }
        /// <summary>
        /// Maximum number of arguments for statement handled by this class.
        /// </summary>
        protected int MaxArgs { get; set; }
    }
}