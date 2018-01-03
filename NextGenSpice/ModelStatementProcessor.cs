using System;
using System.Collections.Generic;
using System.Linq;

namespace NextGenSpice
{
    class ModelStatementProcessor : StatementProcessor
    {
        private readonly Dictionary<string, IModelStatementHandler> handlers;

        public ModelStatementProcessor()
        {
            MaxArgs = Int32.MaxValue;
            MinArgs = 2;

            handlers = new Dictionary<string, IModelStatementHandler>();
        }

        public void AddHandler(IModelStatementHandler handler)
        {
            handlers.Add(handler.Discriminator, handler);
        }

        public override string Discriminator => ".MODEL";
        protected override void DoProcess(Token[] tokens)
        {
            var discriminatorToken = tokens[2];
            if (!handlers.TryGetValue(discriminatorToken.Value, out var handler))
            {
                Context.Errors.Add(discriminatorToken.ToErrorInfo($"No device model has discriminator '{discriminatorToken.Value}'"));
                return;
            }

            handler.Process(tokens, Context);
        }
    }
}