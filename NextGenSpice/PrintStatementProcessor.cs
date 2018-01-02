using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NextGenSpice
{
    public class PrintStatementProcessor : StatementProcessor
    {
        private readonly IDictionary<string, IPrintStatementProcessor> processors;

        public PrintStatementProcessor()
        {
            MaxArgs = int.MaxValue;
            MinArgs = 2;
            processors = new ConcurrentDictionary<string, IPrintStatementProcessor>();

            Register(new LsPrintStatementProcessor("TRAN"));
            Register(new LsPrintStatementProcessor("DC"));
        }

        public void Register(IPrintStatementProcessor proc)
        {
            processors.Add(proc.AnalysisTypeIdentifer, proc);
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