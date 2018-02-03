namespace NextGenSpice.Parser.Statements
{
    /// <summary>
    ///     Defines methods for processing of SPICE statements that begin with a .
    /// </summary>
    public interface IStatementProcessor
    {
        /// <summary>
        ///     Statement discriminator, that this class can handle.
        /// </summary>
        string Discriminator { get; }

        /// <summary>
        ///     Processes the statement.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="ctx"></param>
        void Process(Token[] tokens, ParsingContext ctx);
    }
}