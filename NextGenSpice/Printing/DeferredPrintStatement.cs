using System.Collections.Generic;
using System.Linq;
using NextGenSpice.LargeSignal;
using NextGenSpice.Parser;
using NextGenSpice.Parser.Statements.Deferring;
using NextGenSpice.Parser.Utils;

namespace NextGenSpice.Printing
{
    /// <summary>Class representing intermediate state of a .PRINT statement to be processed once all devices are known.</summary>
    public class DeferredPrintStatement : DeferredStatement
    {
        private readonly string analysisType;
        private readonly List<SpiceParserError> errors;
        private readonly string name;
        private readonly string stat;
        private readonly Token token;
        private PrintStatement<LargeSignalCircuitModel> printStatement;

        public DeferredPrintStatement(ParsingScope scope, Token token, string analysisType):base(scope)
        {
            this.token = token;
            this.analysisType = analysisType;
            var s = token.Value;
            var parStart = s.IndexOf('(');
            var parEnd = s.LastIndexOf(')');

            stat = token.Value.Substring(0, parStart);
            name = token.Value.Substring(parStart + 1, parEnd - parStart - 1);
            errors = new List<SpiceParserError>();
        }

        /// <summary>Returns true if all prerequisites for the statements have been fulfilled and statement is ready to be applied.</summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool CanApply()
        {
            errors.Clear();
            var device =
                context.CircuitBuilder.Devices.FirstOrDefault(el => el.Tag as string == name); // a two terminal device

            if (stat == "V") // output voltage
            {
                int i;
                if (context.SymbolTable.TryGetNodeIndex(name, out var id)) // a node
                {
                    printStatement = new NodeVoltagePrintStatement(name, id);
                }
                else if (device != null) // an device
                {
                    printStatement = new DevicePrintStatement(stat, name, token);
                }
                else if ((i = name.IndexOf(',')) > 0 && i < name.Length - 1) // two nodes
                {
                    var n1 = name.Substring(0, i);
                    var n2 = name.Substring(i + 1);

                    var success = true;

                    // reuse token instance for error reporting
                    token.LineColumn++;
                    if (!context.SymbolTable.TryGetNodeIndex(n1, out var i1))
                    {
                        errors.Add(token.ToError(Parser.SpiceParserErrorCode.NotANode, n1));
                        success = false;
                    }

                    token.LineColumn += n1.Length + 1;
                    if (!context.SymbolTable.TryGetNodeIndex(n2, out var i2))
                    {
                        errors.Add(token.ToError(Parser.SpiceParserErrorCode.NotANode, n2));
                        success = false;
                    }

                    if (success)
                        printStatement = new NodeVoltageDifferencePrintStatement(name, i1, i2);
                }
                else
                {
                    errors.Add(token.ToError(Parser.SpiceParserErrorCode.NotANodeOrDevice));
                }
            }
            else // only circuit devices with stat other than "V"
            {
                if (device != null)
                    printStatement = new DevicePrintStatement(stat, name, token);
                else
                    errors.Add(token.ToError(Parser.SpiceParserErrorCode.NotAnDevice));
            }

            return printStatement != null;
        }

        /// <summary>Applies the statement in the given context.</summary>
        /// <param name="context"></param>
        public override void Apply()
        {
            base.Apply();

            printStatement.AnalysisType = analysisType;
            context.OtherStatements.Add(printStatement);
        }

        /// <summary>Returns set of errors due to which this stetement cannot be processed.</summary>
        /// <returns></returns>
        public override IEnumerable<SpiceParserError> GetErrors()
        {
            return errors;
        }
    }
}