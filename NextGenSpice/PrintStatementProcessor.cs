namespace NextGenSpice
{
    public class PrintStatementProcessor : StatementProcessor
    {
        public override string Discriminator => ".PRINT";
        protected override void DoProcess(Token[] tokens)
        {
            var analysisType = tokens[1].Value;
            if (!Context.KnownAnalysisTypes.Contains(analysisType))
            {
                Context.Errors.Add(new ErrorInfo { LineNumber = tokens[1].Line, LineColumn = tokens[1].Char, Messsage = $"Unrecognized analysis type: '{analysisType}'." });
                return;
            }

            for (int i = 2; i < tokens.Length; i++)
            {
                var t = tokens[i];
                var s = t.Value;

                // expected token in format V(element), I(element), V(node)
                if (s.Length > 3 && (s[0] == 'I' || s[0] == 'V') && s[1] == '(' && s[s.Length - 1] == ')')
                {
                    Context.PrintStatements.Add(new PrintStatement { AnalysisType = analysisType, Header = s });
                }
            }
        }
    }
}