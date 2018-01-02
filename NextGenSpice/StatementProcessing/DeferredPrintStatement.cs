using System.Collections.Generic;
using System.Linq;

namespace NextGenSpice
{
    public class DeferredPrintStatement : DeferredStatement
    {
        private readonly Token token;
        private readonly string analysisType;
        private readonly string name;
        private readonly List<ErrorInfo> errors;
        private LsPrintStatement printStatement;

        public DeferredPrintStatement(Token token, string analysisType)
        {
            this.token = token;
            this.analysisType = analysisType;
            name = token.Value.Substring(2, token.Value.Length - 3);
            errors = new List<ErrorInfo>();
        }

        public override bool CanApply(ParsingContext context)
        {
            errors.Clear();

            var c = token.Value[0];

            var element = context.CircuitBuilder.Elements.FirstOrDefault(el => el.Name == name); // a two terminal device

            if (c == 'V')
            {
                if (context.SymbolTable.NodeIndices.TryGetValue(name, out var id)) // a node
                {
                    printStatement = new NodeVoltagePrintStatement(name, id);
                }
                else if (element != null && element.ConnectedNodes.Count == 2)
                {
                    printStatement = new ElementVoltagePrintStatement(name);
                }
                else
                {
                    errors.Add(token.ToErrorInfo($"'{name}' is not a node or a two terminal element."));
                }
            }
            else
            {
                if (element != null && element.ConnectedNodes.Count == 2)
                {
                    printStatement = new ElementCurrentPrintStatement(name);
                }
                else
                {
                    errors.Add(token.ToErrorInfo($"'{name}' is not a two terminal element."));
                }
            }

            return printStatement != null;
        }

        public override IEnumerable<ErrorInfo> GetErrors()
        {
            return errors;
        }

        public override void Apply(ParsingContext context)
        {
            printStatement.AnalysisType = analysisType;
            context.PrintStatements.Add(printStatement);
        }
    }
}