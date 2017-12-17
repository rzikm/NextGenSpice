using System.Collections.Generic;

namespace NextGenSpice
{
    /// <summary>
    /// Class representing processor for a given element type.
    /// </summary>
    public abstract class ElementStatementProcessor
    {
        protected SymbolTable SymbolTable { get; private set; }

        /// <summary>
        /// Discriminator of the element type this processor can parse.
        /// </summary>
        public abstract char Discriminator { get; }

        /// <summary>
        /// Parses given line of tokens and returns statement to be processed later.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="tab"></param>
        /// <returns></returns>
        public ElementStatement Process(Token[] tokens, SymbolTable tab)
        {
            SymbolTable = tab;

            var errors = new List<ErrorInfo>();
            var ret = DoProcess(tokens, errors);

            SymbolTable = null;

            return errors.Count > 0 ? new ErrorElementStatement(errors) : ret;
        }

        protected abstract ElementStatement DoProcess(Token[] tokens, List<ErrorInfo> errors);

        /// <summary>
        /// Returns generic error message
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected ErrorInfo Error(Token source, string message)
        {
            return new ErrorInfo
            {
                Messsage = message,
                LineNumber = source.Line,
                LineColumn = source.Char
            };
        }

        /// <summary>
        /// Returns message, that some element with given name has been already defined.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected ErrorInfo ElementAlreadyDefined(Token token)
        {
            return Error(token, $"Element with name {token.Value} is already defined.");
        }

        /// <summary>
        /// Return message, that given token cannot be converted to a numeric representation.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected ErrorInfo NotANumber(Token token)
        {
            return Error(token, $"Cannot convert {token.Value} to numeric representation.");
        }

        /// <summary>
        /// Returns message indicating that given token does not represent a node name.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected ErrorInfo NotANode(Token token)
        {
            return Error(token, $"Symbol {token.Value} is not a node");
        }

        /// <summary>
        /// Return message indicatiing that there was wrong number of arguments for given element type.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected ErrorInfo InvalidNumberOfArguments(Token token)
        {
            return Error(token, "Invalid number of arguments");
        }

        protected string DeclareElement(Token token, List<ErrorInfo> errors)
        {
            string name = token.Value;
            if (!SymbolTable.DefineElement(name)) errors.Add(ElementAlreadyDefined(token));
            return name;
        }

        protected int[] GetNodeIndices(Token[] tokens, int startIndex, int count, List<ErrorInfo> errors)
        {
            var ret = new int[count];
            for (int i = 0; i < count; i++)
            {
                var token = tokens[startIndex + i];
                if (!SymbolTable.TryGetNodeIndex(token.Value, out var node))
                {
                    node = -1;
                    errors.Add(NotANode(token));
                }
                ret[i] = node;
            }

            return ret;
        }

        protected double GetValue(Token token, List<ErrorInfo> errors)
        {
            var value = ConvertorHelpers.ConvertValue(token.Value);
            if (double.IsNaN(value)) errors.Add(NotANumber(token));
            return value;
        }
    }
}