namespace NextGenSpice.Parser.Statements.Printing
{
    /// <summary>
    /// Defines methods for handling .PRINT [analysis type] [args...] statements.
    /// </summary>
    public interface IPrintStatementHandler
    {
        /// <summary>
        /// Uppercase identifier of the analysis type of this handler.
        /// </summary>
        string AnalysisTypeIdentifer { get; }
        
        /// <summary>
        /// Processes the .PRINT statement.
        /// </summary>
        /// <param name="tokens">Tokens of the statement.</param>
        /// <param name="context">Current parsing context.</param>
        void ProcessPrintStatement(Token[] tokens, ParsingContext context);
    }
}