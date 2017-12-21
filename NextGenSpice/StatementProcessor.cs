﻿using System;

namespace NextGenSpice
{
    public abstract class StatementProcessor
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
            
            if (tokens.Length - 1 > MinArgs)
                ctx.Errors.Add(firstToken.ToErrorInfo($"Too many arguments for {firstToken.Value} statement."));
            
            DoProcess(tokens);

            Context = null;
        }
        protected abstract void DoProcess(Token[] tokens);
        protected int MinArgs { get; set; }
        protected int MaxArgs { get; set; }
    }
}