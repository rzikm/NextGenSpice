using System.Collections.Generic;
using System.Linq;

namespace NextGenSpice
{
    public class SpiceCodeParser
    {
        private readonly IDictionary<char, ElementStatementProcessor> elementProcessors;
        private readonly IDictionary<string, StatementProcessor> statementProcessors;

        public SpiceCodeParser()
        {
            elementProcessors = new Dictionary<char, ElementStatementProcessor>();
        }

        public void Register(ElementStatementProcessor processor)
        {
            elementProcessors.Add(processor.Discriminator, processor);
        }
        public void Register(StatementProcessor processor)
        {
            statementProcessors.Add(processor.Discriminator, processor);
        }

        public ParserResult ParseInputFile(TokenStream stream)
        {
            var tab = new SymbolTable();
            Token[] tokens;

            var ctx = new ParsingContext();

            while ((tokens = stream.ReadLogicalLine().ToArray()).Length > 0) // while not EOF
            {
                var firstToken = tokens[0];
                var discriminator = firstToken.Value[0];

                if (char.IsLetter(discriminator)) // element statement
                {
                    ProcessElement(discriminator, tokens, ctx);
                }
                else if (discriminator != '.')
                {
                    ctx.Errors.Add(firstToken.ToErrorInfo($"Unexpected character: '{discriminator}'."));
                }
                else
                {
                    ProcessStatement(tokens, ctx);
                }
            }
            return new ParserResult(ctx.ElementStatements, ctx.PrintStatements, ctx.SimulationStatements, ctx.Errors);
        }

        private void ProcessStatement(Token[] tokens, ParsingContext ctx)
        {
            var discriminator = tokens[0].Value;
            if (statementProcessors.TryGetValue(discriminator, out var proc))
            {
                proc.Process(tokens, ctx);
            }
            else
            {
                ctx.Errors.Add(tokens[0].ToErrorInfo($"Statement invalid or not implemented: {discriminator}."));
            }
        }

        private void ProcessPrint(Token[] tokens, ParsingContext ctx)
        {
            if (tokens.Length < 3)
            {
                ctx.Errors.Add(new ErrorInfo { LineNumber = tokens[0].Line, LineColumn = tokens[0].Char, Messsage = $"Too few arguments for {tokens[0].Value} statement." });
                return;
            }

            var analysisType = tokens[1].Value;
            if (analysisType != "TRAN")
            {
                ctx.Errors.Add(new ErrorInfo { LineNumber = tokens[1].Line, LineColumn = tokens[1].Char, Messsage = $"Unrecognized analysis type: '{analysisType}'." });
            }

            for (int i = 2; i < tokens.Length; i++)
            {
                var t = tokens[i];
                var s = t.Value;

                // expected token in format V(element), I(element), V(node)
                if (s.Length > 3 && (s[0] == 'I' || s[0] == 'V') && s[1] == '(' && s[s.Length - 1] == ')')
                {
                    ctx.PrintStatements.Add(new PrintStatement { AnalysisType = analysisType, Header = s });
                }
                else
                {
                    ctx.Errors.Add(new ErrorInfo { LineNumber = t.Line, LineColumn = t.Char, Messsage = $"Unrecognized variable: '{s}'." });
                }
            }
        }

        private void ProcessElement(char discriminator, Token[] tokens, ParsingContext ctx)
        {
            if (elementProcessors.TryGetValue(discriminator, out var proc))
            {
                proc.Process(tokens, ctx);
            }
            else
            {
                ctx.Errors.Add(tokens[0].ToErrorInfo($"Element type invalid or not implemented: {discriminator}."));
            }
            
        }
    }
}