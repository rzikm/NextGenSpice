using System.Collections.Concurrent;
using System.Collections.Generic;
using NextGenSpice.Parser.Statements.Printing;
using NextGenSpice.Utils;

namespace NextGenSpice.Parser.Statements
{
    /// <summary>Class responsible for processing .PRINT [analysis type] [...data] statements.</summary>
    public class PrintStatementProcessor : DotStatementProcessor
    {
        private readonly IDictionary<string, IPrintStatementHandler> processors;

        public PrintStatementProcessor()
        {
            MaxArgs = int.MaxValue;
            MinArgs = 2;
            processors = new ConcurrentDictionary<string, IPrintStatementHandler>();
        }

        /// <summary>Statement discriminator, that this class can handle.</summary>
        public override string Discriminator => ".PRINT";

        /// <summary>Adds a handler for a certain analysis type.</summary>
        /// <param name="handler">The handler.</param>
        public void AddHandler(IPrintStatementHandler handler)
        {
            processors.Add(handler.AnalysisTypeIdentifer, handler);
        }

        /// <summary>Processes given statement.</summary>
        /// <param name="tokens">All tokens of the statement.</param>
        protected override void DoProcess(Token[] tokens)
        {
            var analysisType = tokens[1].Value;
            if (!processors.TryGetValue(analysisType, out var proc))
            {
                Context.Errors.Add(tokens[1].ToErrorInfo(SpiceParserError.UnknownAnalysisType));
                return;
            }

            proc.ProcessPrintStatement(tokens, Context);
        }
    }
}