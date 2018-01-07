namespace NextGenSpice
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

                // expected token in format V(element), I(element), V(node)
                if (s.Length > 3 && (s[0] == 'I' || s[0] == 'V') && s[1] == '(' && s[s.Length - 1] == ')')
                    context.DeferredStatements.Add(new DeferredPrintStatement(t, AnalysisTypeIdentifer));
                else
                    context.Errors.Add(tokens[i].ToErrorInfo($"Unsupported .PRINT statement format: '{s}'."));
            }
        }

        /// <summary>
        /// Creates instance of LsPrintStatementHandler for handling TRAN analysis type.
        /// </summary>
        /// <returns></returns>
        public static LsPrintStatementHandler CreateTran()
        {
            return new LsPrintStatementHandler("TRAN");
        }


        /// <summary>
        /// Creates instance of LsPrintStatementHandler for handling OP analysis type.
        /// </summary>
        /// <returns></returns>
        public static LsPrintStatementHandler CreateOp()
        {
            return new LsPrintStatementHandler("OP");
        }
    }
}