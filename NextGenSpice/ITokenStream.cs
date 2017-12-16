using System.Collections.Generic;

namespace NextGenSpice
{
    public interface ITokenStream
    {
        Token Read();
        IEnumerable<Token> ReadLogicalLine();
    }
}