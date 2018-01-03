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
            MinArgs = 3;

            handlers = new Dictionary<string, IModelStatementHandler>();
        }

        public void AddHandler(IModelStatementHandler handler)
        {
            handlers.Add(handler.Discriminator, handler);
        }

        public override string Discriminator => ".MODEL";
        protected override void DoProcess(Token[] tokens)
        {
            if (!handlers.TryGetValue(tokens[1].Value, out var handler))
            {
                Context.Errors.Add(tokens[1].ToErrorInfo($"No device model has discriminator '{tokens[1].Value}'"));
                return;
            }

            handler.Process(tokens.Skip(2).ToArray(), Context);
        }
    }
}