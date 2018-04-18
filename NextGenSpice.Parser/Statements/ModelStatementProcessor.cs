using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Parser.Statements.Models;
using NextGenSpice.Parser.Utils;

namespace NextGenSpice.Parser.Statements
{
    /// <summary>Class responsible for processing .MODEL statements.</summary>
    internal class ModelStatementProcessor : DotStatementProcessor
    {
        private readonly Dictionary<string, IModelStatementHandler> handlers;

        public ModelStatementProcessor()
        {
            MaxArgs = int.MaxValue;
            MinArgs = 2;

            handlers = new Dictionary<string, IModelStatementHandler>();
        }

        /// <summary>Statement discriminator, that this class can handle.</summary>
        public override string Discriminator => ".MODEL";

        /// <summary>Adds handler for a specific device model type.</summary>
        /// <param name="handler"></param>
        public void AddHandler(IModelStatementHandler handler)
        {
            handlers.Add(handler.Discriminator, handler);
        }

        /// <summary>Processes given statement.</summary>
        /// <param name="tokens">All tokens of the statement.</param>
        protected override void DoProcess(Token[] tokens)
        {
            tokens = tokens.Take(2).Concat(Helper.Retokenize(tokens, 2)).ToArray();
            var discriminatorToken = tokens[2];
            if (!handlers.TryGetValue(discriminatorToken.Value, out var handler))
            {
                Context.Errors.Add(
                    discriminatorToken.ToErrorInfo(SpiceParserError.UnknownDeviceModelDiscriminator));
                return;
            }

            handler.Process(tokens, Context);
        }

        public void RegisterDefaultModels(ParsingContext ctx)
        {
            foreach (var kvp in handlers)
            {
                var handler = kvp.Value;
                var model = handler.CreateDefaultModel();
                ctx.SymbolTable.AddModel(model.GetType(), model, handler.Discriminator);
            }
        }
    }
}