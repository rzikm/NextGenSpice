namespace NextGenSpice
{
    public class LsPrintStatementProcessor : IPrintStatementProcessor
    {
        public LsPrintStatementProcessor(string analysisTypeIdentifer)
        {
            AnalysisTypeIdentifer = analysisTypeIdentifer;
        }

        public string AnalysisTypeIdentifer { get; }
        public void ProcessPrintStatement(Token[] tokens, ParsingContext context)
        {
            for (int i = 2; i < tokens.Length; i++)
            {
                var t = tokens[i];
                var s = t.Value;

                // expected token in format V(element), I(element), V(node)
                if (s.Length > 3 && (s[0] == 'I' || s[0] == 'V') && s[1] == '(' && s[s.Length - 1] == ')')
                {
                    context.DeferredStatements.Add(new DeferredPrintStatement(t, AnalysisTypeIdentifer));
                }
                else
                {
                    context.Errors.Add(tokens[i].ToErrorInfo($"Unsupported .PRINT statement format: '{s}'."));
                }
            }
        }
    }
}