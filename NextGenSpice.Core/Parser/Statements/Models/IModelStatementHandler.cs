namespace NextGenSpice.Core.Parser.Statements.Models
{
    /// <summary>Defines methods for handling concrete .MODEL statements</summary>
    public interface IModelStatementHandler
    {
        /// <summary>Discriminator of model type.</summary>
        string Discriminator { get; }

        /// <summary>Processes the .MODEL statement in given context.</summary>
        /// <param name="tokens"></param>
        /// <param name="context"></param>
        void Process(Token[] tokens, ParsingContext context);

        /// <summary>Creates new instance of model parameter class with default values.</summary>
        /// <returns>Default device model.</returns>
        object CreateDefaultModel();
    }
}