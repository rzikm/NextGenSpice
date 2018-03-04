using NextGenSpice.Parser;

namespace NextGenSpice.Utils
{
    /// <summary>
    ///     Provides basic information about an error that occured during parsing of an input file.
    /// </summary>
    public class ErrorInfo
    {
        /// <summary>Creates a new instance of the <see cref="ErrorInfo"></see> class.</summary>
        public static ErrorInfo Create(SpiceParserError errorCode, int lineNumber, int lineColumn, params object[] args)
        {
            return new ErrorInfo(errorCode, lineNumber, lineColumn, args);
        }

        /// <summary>Initializes a new instance of the <see cref="ErrorInfo"></see> class.</summary>
        public ErrorInfo(SpiceParserError errorCode, int lineNumber, int lineColumn, object[] args)
        {
            LineNumber = lineNumber;
            LineColumn = lineColumn;
            ErrorCode = errorCode;
            Args = args;
        }

        /// <summary>
        ///     Error code characterizing this error.
        /// </summary>
        public SpiceParserError ErrorCode { get; }

        /// <summary>
        ///     Index of a line from input file on which error occured.
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        ///     One-based index of character where the error occured.
        /// </summary>
        public int LineColumn { get; }

        /// <summary>
        ///     Additional arguments for the error message.
        /// </summary>
        public object[] Args { get; }

        /// <summary>
        ///     Messsage summarizing the error.
        /// </summary>
        public string Messsage => ErrorCode.GetMessage(Args);

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return LineColumn * LineNumber == 0 ? Messsage : $"({LineNumber}, {LineColumn}): {Messsage}";
        }
    }
}