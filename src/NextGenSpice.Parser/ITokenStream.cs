using System.Collections.Generic;

namespace NextGenSpice.Parser
{
    /// <summary>Defines interface for reading tokens.</summary>
    public interface ITokenStream
    {
        /// <summary>
        ///     Reads collection of tokens that form one statement, i.e. starting from next token until the end of line not
        ///     beginning with '+'. Returns empty collection on EOF.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Token> ReadStatement();
    }
}