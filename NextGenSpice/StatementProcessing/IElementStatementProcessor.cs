using System.Collections.Generic;

namespace NextGenSpice
{
    public interface IElementStatementProcessor
    {
        /// <summary>
        /// Gets list of model statement handlers that are responsible to parsing respective models of this device.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IModelStatementHandler> GetModelStatementHandlers();

        /// <summary>
        /// Discriminator of the element type this processor can parse.
        /// </summary>
        char Discriminator { get; }

        /// <summary>
        /// Parses given line of tokens, adds statement to be processed later or adds errors to Errors collection.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        void Process(Token[] tokens, ParsingContext ctx);
    }
}