using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NextGenSpice.Parser
{
    /// <summary>Class for reading tokens with their location from given TextReader</summary>
    public class TokenStream : ITokenStream
    {
        private readonly TextReader inputReader;

        private StringReader currentLine;
        private int line;
        private int offset;

        public TokenStream(TextReader input, int line)
        {
            inputReader = input;
            this.line = line;
        }

        /// <summary>Reads a token from the stream, returns null on EOF.</summary>
        /// <returns>First not already read token or null</returns>
        public Token Read()
        {
            if (!TryEnsureHasLine()) return null;

            return MakeToken();
        }

        /// <summary>
        ///     Skips empty lines and then reads all tokens until line break. Tokens on subsequent lines beginning with '+'
        ///     are also returned. returns empty collection on EOF.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Token> ReadLogicalLine()
        {
            var token = Read();
            if (token == null) yield break;
            yield return token;
            while (true)
            {
                SkipWhiteSpaceAndComments();
                if (currentLine == null && !ShouldContinue()) break;
                yield return MakeToken();
            }
        }

        /// <summary>Ensures that there is some token to be read in currentLine. Return true if succeeded.</summary>
        /// <returns></returns>
        private bool TryEnsureHasLine()
        {
            if (currentLine != null) // there is yet unfinished line
                SkipWhiteSpaceAndComments();

            while (currentLine == null)
            {
                var readLine = inputReader.ReadLine();
                if (readLine == null) return false; // end of underlying stream
                currentLine = new StringReader(readLine);

                line++;
                offset = 1;

                SkipWhiteSpaceAndComments(); // skip leading whitespaces or skip line as whole if begins with comment
            }

            return true;
        }

        /// <summary>Reads available token from currentLine, return null if currentLine contains only whitespace</summary>
        /// <returns></returns>
        private Token MakeToken()
        {
            // mark the position of the token
            var line = this.line;
            var offset = this.offset;

            var s = ReadStringToken();

            return string.IsNullOrWhiteSpace(s)
                ? null // no more tokens in the stream
                : new Token
                {
                    Value = s.ToUpperInvariant(),
                    LineColumn = offset,
                    LineNumber = line
                };
        }

        private bool ShouldContinue()
        {
            while (currentLine == null && TryEnsureHasLine())
                if (currentLine.Peek() == '+')
                {
                    currentLine.Read();
                    offset++;
                    SkipWhiteSpaceAndComments();
                    if (currentLine != null) return true;
                }

            return false;
        }

        /// <summary>Reads token from</summary>
        /// <returns></returns>
        private string ReadStringToken()
        {
            int c;
            var sb = new StringBuilder();
            while ((c = currentLine.Peek()) != -1)
            {
                if (char.IsWhiteSpace((char) c) || c == '*') break; // end of the token
                sb.Append((char) currentLine.Read());
                ++offset;
            }

            return sb.ToString();
        }

        /// <summary>Skips all leading whitespaces and comments, sets currentLine to null if EOL reached.</summary>
        private void SkipWhiteSpaceAndComments()
        {
            int c;
            while ((c = currentLine.Peek()) != -1 && char.IsWhiteSpace((char) c))
            {
                currentLine.Read();
                ++offset;
            }

            if (c == -1 || c == '*') currentLine = null;
        }
    }
}