namespace NextGenSpice
{
    public interface IStatementProcessor
    {
        string Discriminator { get; }
        void Process(Token[] tokens, ParsingContext ctx);
    }
}