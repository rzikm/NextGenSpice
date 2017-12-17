using System.Collections.Generic;
using System.Linq;

namespace NextGenSpice
{
    public class SpiceCodeParser
    {
        private readonly IDictionary<char, ElementStatementProcessor> elementProcessors;

        public SpiceCodeParser()
        {
            elementProcessors = new Dictionary<char, ElementStatementProcessor>();
        }

        public void Register(ElementStatementProcessor processor)
        {
            elementProcessors.Add(processor.Discriminator, processor);
        }

        public ParserResult ParseInputFile(TokenStream stream)
        {
            var tab = new SymbolTable();
            Token[] tokens;
            var elemStatements = new List<ElementStatement>();

            while ((tokens = stream.ReadLogicalLine().ToArray()).Length > 0) // while not EOF
            {
                var discriminator = tokens[0].Value[0];
                if (char.IsLetter(discriminator)) // element statement
                    elemStatements.Add(ProcessElement(discriminator, tokens, tab));
            }
            return new ParserResult(elemStatements);
        }

        private ElementStatement ProcessElement(char discriminator, Token[] tokens, SymbolTable tab)
        {
            return !elementProcessors.TryGetValue(discriminator, out var proc)
                ? new ErrorElementStatement(new List<ErrorInfo>
                {
                    new ErrorInfo
                    {
                        LineNumber = tokens[0].Line,
                        LineColumn = tokens[0].Char,
                        Messsage = $"Element type invalid or not implemented: {discriminator}."
                    }
                })
                : proc.Process(tokens, tab);
        }
    }

    public class ParserResult
    {
        public ParserResult(IEnumerable<ElementStatement> elementStatements)
        {
            ElementStatements = elementStatements;
        }

        public IEnumerable<ElementStatement> ElementStatements { get; }
    }
}