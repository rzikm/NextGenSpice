using System.Collections.Generic;
using System.Linq;

namespace NextGenSpice
{
    /// <summary>
    /// Class representing processor for a given element type.
    /// </summary>
    public abstract class ElementStatementProcessor : IElementStatementProcessor
    {
        protected SymbolTable SymbolTable => Context.SymbolTable;

        protected int Errors { get; private set; }

        protected ParsingContext Context { get; private set; }

        /// <summary>
        /// Discriminator of the element type this processor can parse.
        /// </summary>
        public abstract char Discriminator { get; }

        /// <summary>
        /// Parses given line of tokens, adds statement to be processed later or adds errors to Errors collection.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public void Process(Token[] tokens, ParsingContext ctx)
        {
            Context = ctx;

            Errors = 0;
            DoProcess(tokens);

            Context = null;
        }

        protected abstract void DoProcess(Token[] tokens);

        /// <summary>
        /// Returns generic error message
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected void Error(Token source, string message)
        {
            Errors++;
            Context.Errors.Add( new ErrorInfo
            {
                Messsage = message,
                LineNumber = source.Line,
                LineColumn = source.Char
            });
        }

        /// <summary>
        /// Returns message, that some element with given name has been already defined.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected void ElementAlreadyDefined(Token token)
        {
            Error(token, $"Element with name {token.Value} is already defined.");
        }

        /// <summary>
        /// Return message, that given token cannot be converted to a numeric representation.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected void NotANumber(Token token)
        {
            Error(token, $"Cannot convert {token.Value} to numeric representation.");
        }

        /// <summary>
        /// Returns message indicating that given token does not represent a node name.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected void NotANode(Token token)
        {
            Error(token, $"Symbol {token.Value} is not a node");
        }

        /// <summary>
        /// Return message indicatiing that there was wrong number of arguments for given element type.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected void InvalidNumberOfArguments(Token token)
        {
            Error(token, "Invalid number of arguments");
        }

        /// <summary>
        /// Gets element name and sets it in symbol table, adds relevant errors into the errors collection
        /// </summary>
        /// <param name="token"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        protected string DeclareElement(Token token)
        {
            var name = token.Value;
            if (!SymbolTable.DefineElement(name)) ElementAlreadyDefined(token);
            return name;
        }

        /// <summary>
        /// Gets indices of the nodes represented by tokens starting at startIndex. Adds relevant errors into the errors collection
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        protected int[] GetNodeIndices(Token[] tokens, int startIndex, int count)
        {
            var ret = new int[count];
            for (int i = 0; i < count; i++)
            {
                var token = tokens[startIndex + i];
                if (!SymbolTable.TryGetNodeIndex(token.Value, out var node))
                {
                    node = -1;
                    NotANode(token);
                }
                ret[i] = node;
            }

            return ret;
        }

        /// <summary>
        /// Parses numeric value from given token, adds relevant error into the errors collection and returns NaN if failed.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        protected double GetValue(Token token)
        {
            var value = Helper.ConvertValue(token.Value);
            if (double.IsNaN(value)) NotANumber(token);
            return value;
        }

        /// <summary>
        /// Gets list of model statement handlers that are responsible to parsing respective models of this device.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<IModelStatementHandler> GetModelStatementHandlers()
        {
            return Enumerable.Empty<IModelStatementHandler>();
        }
    }
}