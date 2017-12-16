using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace NextGenSpice
{
    public class TokenStream : ITokenStream
    {
        private readonly TextReader inputReader;
        private int line;
        private int offset;

        private StringReader currentLine;

        public TokenStream(TextReader input)
        {
            inputReader = input;
        }

        public Token Read()
        {
            if (!EnsureHasLine()) return null;

            return MakeToken();
        }

        private bool EnsureHasLine()
        {
            if (currentLine != null)
                SkipWhiteSpaceAndComments();

            while (currentLine == null)
            {
                var readLine = inputReader.ReadLine();
                if (readLine == null) return false;
                currentLine = new StringReader(readLine);

                line++;
                offset = 1;

                SkipWhiteSpaceAndComments();
            }

            return true;
        }

        private Token MakeToken()
        {
            var line = this.line;
            var offset = this.offset;

            var s = ReadStringToken();

            return string.IsNullOrWhiteSpace(s)
                ? null
                : new Token
                {
                    Value = s,
                    Char = offset,
                    Line = line
                };
        }

        public IEnumerable<Token> ReadLogicalLine()
        {
            List<Token> tokens = new List<Token>();
            tokens.Add(Read());
            if (tokens[0] == null)
                return Enumerable.Empty<Token>();

            while (currentLine != null)
            {
                var t = MakeToken();
                if (t == null)
                    break;
                tokens.Add(t);
            }
            return tokens;
        }

        private string ReadStringToken()
        {
            int c;
            var sb = new StringBuilder();
            while ((c = currentLine.Read()) != -1)
            {
                ++offset;
                if (c == '*') // comment until the end of the line
                {
                    currentLine = null; 
                    break;
                }
                if (char.IsWhiteSpace((char)c)) break; // end of the token
                sb.Append((char)c);
            }
            return sb.ToString();
        }

        private void SkipWhiteSpaceAndComments()
        {
            int c;
            while ((c = currentLine.Peek()) != -1 && char.IsWhiteSpace((char)c))
            {
                currentLine.Read();
                ++offset;
            }
            if (c == -1 || c == '*') currentLine = null;
        }
    }
}