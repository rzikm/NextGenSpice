using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NextGenSpice
{
    public class PrintStatementProcessor : StatementProcessor
    {
        private readonly IDictionary<string, IPrintStatementHandler> processors;

        public PrintStatementProcessor()
        {
            MaxArgs = int.MaxValue;
            MinArgs = 2;
            processors = new ConcurrentDictionary<string, IPrintStatementHandler>();

            AddHandler(new LsPrintStatementHandler("TRAN"));
            AddHandler(new LsPrintStatementHandler("DC"));
        }

        public void AddHandler(IPrintStatementHandler handler)
        {
            processors.Add(handler.AnalysisTypeIdentifer, handler);
        }

        public override string Discriminator => ".PRINT";
        protected override void DoProcess(Token[] tokens)
        {
            var analysisType = tokens[1].Value;
            if (!processors.TryGetValue(analysisType, out var proc))
            {
                Context.Errors.Add(tokens[1].ToErrorInfo($"Unrecognized analysis type: '{analysisType}'."));
                return;
            }
            
            proc.ProcessPrintStatement(tokens, Context);
        }
    }
}