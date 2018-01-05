using System;

namespace NextGenSpice
{
    public abstract class StatementProcessor : IStatementProcessor
    {
        public StatementProcessor()
        {
            MaxArgs = Int32.MaxValue;
        }
        public abstract string Discriminator { get; }

        protected ParsingContext Context { get; private set; }

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
        /// <param name="tokens"></param>
        protected abstract void DoProcess(Token[] tokens);
        protected int MinArgs { get; set; }
        protected int MaxArgs { get; set; }
    }
}