namespace NextGenSpice
{
    public interface IModelStatementHandler
    {
        string Discriminator { get; }
        void Process(Token[] tokens, ParsingContext context);
    }
}