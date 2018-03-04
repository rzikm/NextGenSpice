using System.Linq;
using System.Text.RegularExpressions;
using NextGenSpice.Utils;

namespace NextGenSpice.Parser.Statements
{
    /// <summary>
    ///     Class for handling statements that specify initial node voltages.
    /// </summary>
    public class InitialConditionStatement : StatementProcessor
    {
        private readonly Regex argRegex;

        public InitialConditionStatement()
        {
            argRegex = new Regex(@"[vV]\([^)]+\)=(.*)", RegexOptions.Compiled);
            MinArgs = 1;
        }

        /// <summary>
        ///     Statement discriminator, that this class can handle.
        /// </summary>
        public override string Discriminator => ".IC";

        /// <summary>
        ///     Processes given statement.
        /// </summary>
        /// <param name="tokens">All tokens of the statement.</param>
        protected override void DoProcess(Token[] tokens)
        {
            foreach (var token in tokens.Skip(1)) // skip ".IC" token
            {
                var s = token.Value;

                var matches = argRegex.Match(s);

                if (!matches.Success)
                {
                    Context.Errors.Add(token.ToErrorInfo(SpiceParserError.InvalidIcArgument));
                    continue;
                }

                var nodeName = matches.Captures[0].Value;
                if (!Context.SymbolTable.TryGetOrCreateNode(nodeName, out var nodeIndex))
                {
                    Context.Errors.Add(token.ToErrorInfo(SpiceParserError.NotANode));
                    nodeIndex = -1;
                    continue;
                }

                var value = new Token
                {
                    LineNumber = token.LineNumber,
                    LineColumn = token.LineColumn + matches.Captures[1].Index,
                    Value = matches.Captures[1].Value
                }.GetNumericValue(Context.Errors);

                if (nodeIndex >= 0) Context.CircuitBuilder.SetNodeVoltage(nodeIndex, value);
            }
        }
    }
}