namespace NextGenSpice
{
    public interface IPrintStatementProcessor
    {
        string AnalysisTypeIdentifer { get; }
        void ProcessPrintStatement(Token[] tokens, ParsingContext context);
    }
}