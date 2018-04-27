using System.Collections.Generic;
using NextGenSpice.Parser.Statements.Models;

namespace NextGenSpice.Parser.Statements.Devices
{
    /// <summary>Defines methods for parsing SPICE device statements.</summary>
    public interface IDeviceStatementProcessor
    {
        /// <summary>Discriminator of the device type this processor can parse.</summary>
        char Discriminator { get; }

        /// <summary>Gets list of model statement handlers that are responsible to parsing respective models of this device.</summary>
        /// <returns></returns>
        IEnumerable<IModelStatementHandler> GetModelStatementHandlers();

        /// <summary>Parses given line of tokens, adds statement to be processed later or adds errors to Errors collection.</summary>
        /// <param name="tokens"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        void Process(Token[] tokens, ParsingContext ctx);
    }
}