using System.Collections.Generic;
using System.Linq;

namespace NextGenSpice
{
    /// <summary>
    /// Class representing intermediate state of a .PRINT statement to be processed once all devices are known.
    /// </summary>
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

        /// <summary>
        /// Returns true if all prerequisites for the statements have been fulfilled and statement is ready to be applied.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool CanApply(ParsingContext context)
        {
            errors.Clear();
            var c = token.Value[0];
            var element = context.CircuitBuilder.Elements.FirstOrDefault(el => el.Name == name); // a two terminal device

            if (c == 'V') // output voltage
            {
                if (context.SymbolTable.NodeIndices.TryGetValue(name, out var id)) // a node
                {
                    printStatement = new NodeVoltagePrintStatement(name, id);
                }
                else if (element != null && element.ConnectedNodes.Count == 2) // an element
                {
                    printStatement = new ElementVoltagePrintStatement(name);
                }
                else
                {
                    errors.Add(token.ToErrorInfo($"'{name}' is not a node or a two terminal element."));
                }
            }
            else // output current (c == 'I'), other states should not occur due to logic in PrintStatementProcessor
            {
                if (element != null && element.ConnectedNodes.Count == 2) // only certain elements are supported
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

        /// <summary>
        /// Returns set of errors due to which this stetement cannot be processed.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<ErrorInfo> GetErrors()
        {
            return errors;
        }

        /// <summary>
        /// Applies the statement in the given context.
        /// </summary>
        /// <param name="context"></param>
        public override void Apply(ParsingContext context)
        {
            printStatement.AnalysisType = analysisType;
            context.PrintStatements.Add(printStatement);
        }
    }
}