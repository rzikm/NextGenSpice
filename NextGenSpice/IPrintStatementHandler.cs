namespace NextGenSpice
{
    public interface IPrintStatementHandler
    {
        string AnalysisTypeIdentifer { get; }
        void ProcessPrintStatement(Token[] tokens, ParsingContext context);
    }
}