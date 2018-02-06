using NextGenSpice.Parser.Statements.Deferring;
using NextGenSpice.Utils;

namespace NextGenSpice.Parser.Statements.Printing
{
    /// <summary>
    ///     Class for handling .PRINT statement for both TRAN and OP simulations.
    /// </summary>
    public class LsPrintStatementHandler : IPrintStatementHandler
    {
        protected LsPrintStatementHandler(string analysisTypeIdentifer)
        {
            AnalysisTypeIdentifer = analysisTypeIdentifer;
        }

        /// <summary>
        ///     Uppercase identifier of the analysis type of this handler.
        /// </summary>
        public string AnalysisTypeIdentifer { get; }

        /// <summary>
        ///     Processes the .PRINT statement.
        /// </summary>
        /// <param name="tokens">Tokens of the statement.</param>
        /// <param name="context">Current parsing context.</param>
        public void ProcessPrintStatement(Token[] tokens, ParsingContext context)
        {
            for (var i = 2; i < tokens.Length; i++)
            {
                var t = tokens[i];
                var s = t.Value;
                var parStart = s.IndexOf('(');
                var parEnd = s.LastIndexOf(')');
                
                // expected token in format <Stat>(element), V(node), V(node1,node2)
                if (parStart < 1 || parEnd < s.Length -1 || s.Length <= 3)
                    context.Errors.Add(tokens[i].ToErrorInfo($"Unsupported .PRINT statement format: '{s}'."));
                else
                {
                    context.DeferredStatements.Add(new DeferredPrintStatement(t, AnalysisTypeIdentifer));
                }
            }
        }

        /// <summary>
        ///     Creates instance of LsPrintStatementHandler for handling TRAN analysis type.
        /// </summary>
        /// <returns></returns>
        public static LsPrintStatementHandler CreateTran()
        {
            return new LsPrintStatementHandler("TRAN");
        }


        /// <summary>
        ///     Creates instance of LsPrintStatementHandler for handling OP analysis type.
        /// </summary>
        /// <returns></returns>
        public static LsPrintStatementHandler CreateOp()
        {
            return new LsPrintStatementHandler("OP");
        }
    }
}