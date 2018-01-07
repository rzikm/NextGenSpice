using System;
using System.Collections.Generic;
using NextGenSpice.Parser.Statements.Models;
using NextGenSpice.Utils;

namespace NextGenSpice.Parser.Statements
{
    /// <summary>
    /// Class responsible for processing .MODEL statements.
    /// </summary>
    class ModelStatementProcessor : StatementProcessor
    {
        private readonly Dictionary<string, IModelStatementHandler> handlers;

        public ModelStatementProcessor()
        {
            MaxArgs = Int32.MaxValue;
            MinArgs = 2;

            handlers = new Dictionary<string, IModelStatementHandler>();
        }

        /// <summary>
        /// Adds handler for a specific device model type.
        /// </summary>
        /// <param name="handler"></param>
        public void AddHandler(IModelStatementHandler handler)
        {
            handlers.Add(handler.Discriminator, handler);
        }

        /// <summary>
        /// Statement discriminator, that this class can handle.
        /// </summary>
        public override string Discriminator => ".MODEL";

        /// <summary>
        /// Processes given statement.
        /// </summary>
        /// <param name="tokens">All tokens of the statement.</param>
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