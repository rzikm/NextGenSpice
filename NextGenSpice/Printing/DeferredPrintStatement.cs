using System.Collections.Generic;
using System.IO;
using System.Linq;
using NextGenSpice.Core.Parser;
using NextGenSpice.Core.Parser.Statements.Deferring;
using NextGenSpice.Core.Parser.Utils;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpice.Printing
{
    /// <summary>Class representing intermediate state of a .PRINT statement to be processed once all devices are known.</summary>
    public class DeferredPrintStatement : DeferredStatement
    {
        private readonly string analysisType;
        private readonly List<ErrorInfo> errors;
        private readonly string name;
        private readonly string stat;
        private readonly Token token;
        private PrintStatement<LargeSignalCircuitModel> printStatement;

        public DeferredPrintStatement(Token token, string analysisType)
        {
            this.token = token;
            this.analysisType = analysisType;
            var s = token.Value;
            var parStart = s.IndexOf('(');
            var parEnd = s.LastIndexOf(')');

            stat = token.Value.Substring(0, parStart);
            name = token.Value.Substring(parStart + 1, parEnd - parStart - 1);
            errors = new List<ErrorInfo>();
        }

        /// <summary>Returns true if all prerequisites for the statements have been fulfilled and statement is ready to be applied.</summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool CanApply(ParsingContext context)
        {
            errors.Clear();
            var device =
                context.CircuitBuilder.Devices.FirstOrDefault(el => el.Name == name); // a two terminal device

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
                        errors.Add(token.ToErrorInfo(SpiceParserError.NotANode, n1));
                        success = false;
                    }

                    token.LineColumn += n1.Length + 1;
                    if (!context.SymbolTable.TryGetNodeIndex(n2, out var i2))
                    {
                        errors.Add(token.ToErrorInfo(SpiceParserError.NotANode, n2));
                        success = false;
                    }

                    if (success)
                        printStatement = new NodeVoltageDifferencePrintStatement(name, i1, i2);
                }
                else
                {
                    errors.Add(token.ToErrorInfo(SpiceParserError.NotANodeOrDevice));
                }
            }
            else // only circuit devices with stat other than "V"
            {
                if (device != null)
                    printStatement = new DevicePrintStatement(stat, name, token);
                else
                    errors.Add(token.ToErrorInfo(SpiceParserError.NotAnDevice));
            }

            return printStatement != null;
        }

        /// <summary>Returns set of errors due to which this stetement cannot be processed.</summary>
        /// <returns></returns>
        public override IEnumerable<ErrorInfo> GetErrors()
        {
            return errors;
        }

        /// <summary>Applies the statement in the given context.</summary>
        /// <param name="context"></param>
        public override void Apply(ParsingContext context)
        {
            printStatement.AnalysisType = analysisType;
            context.OtherStatements.Add(printStatement);
        }
    }

    public class DevicePrintStatement : PrintStatement<LargeSignalCircuitModel>
    {
        private readonly string name;
        private readonly string stat;
        private readonly Token t;
        private IDeviceStatsProvider provider;

        public DevicePrintStatement(string stat, string name, Token t)
        {
            this.stat = stat;
            this.name = name;
            this.t = t;
        }

        /// <summary>Information about what kind of data are handled by this print statement.</summary>
        public override string Header => $"{stat}({name})";

        /// <summary>Prints value of handled by this print statement into given TextWriter.</summary>
        /// <param name="output">Output TextWriter where to write.</param>
        public override void PrintValue(TextWriter output)
        {
            output.Write(provider.GetValue());
        }

        /// <summary>Initializes print statement for given circuit model and returns set of errors that occured (if any).</summary>
        /// <param name="circuitModel">Current model of the circuit.</param>
        /// <returns>Set of errors that errored (if any).</returns>
        public override IEnumerable<ErrorInfo> Initialize(LargeSignalCircuitModel circuitModel)
        {
            var model = circuitModel.GetDevice(name);
            provider = model.GetDeviceStatsProviders().SingleOrDefault(pr => pr.StatName == stat);
            var errorInfos = provider == null
                ? new[]
                {
                    ErrorInfo.Create(SpiceParserError.NoPrintProvider, 0, 0, stat, name)
                }
                : Enumerable.Empty<ErrorInfo>();
            return errorInfos;
        }
    }
}