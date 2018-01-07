using System.Collections.Generic;

namespace NextGenSpice.Parser
{
    /// <summary>
    /// Defines interface for reading tokens.
    /// </summary>
    public interface ITokenStream
    {
        /// <summary>
        /// Reads one token from input, returns null on EOF.
        /// </summary>
        /// <returns></returns>
        Token Read();

        /// <summary>
        /// Reads set of tokens until the end of logical line, i.e. starting from next token until the end of line not beginning with '+'. Returns empty set on EOF.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Token> ReadLogicalLine();
    }
}