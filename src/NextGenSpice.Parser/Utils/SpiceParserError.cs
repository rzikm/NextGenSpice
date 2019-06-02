namespace NextGenSpice.Parser.Utils
{
	/// <summary>Provides basic information about an error that occured during parsing of an input file.</summary>
	public class SpiceParserError
	{
		/// <summary>Initializes a new instance of the <see cref="SpiceParserError"></see> class.</summary>
		public SpiceParserError(SpiceParserErrorCode errorCode, int lineNumber, int lineColumn, object[] args)
		{
			LineNumber = lineNumber;
			LineColumn = lineColumn;
			ErrorCode = errorCode;
			Args = args;
		}

		/// <summary>Error code characterizing this error.</summary>
		public SpiceParserErrorCode ErrorCode { get; }

		/// <summary>Index of a line from input file on which error occured.</summary>
		public int LineNumber { get; }

		/// <summary>One-based index of character where the error occured.</summary>
		public int LineColumn { get; }

		/// <summary>Additional arguments for the error message.</summary>
		public object[] Args { get; }

		/// <summary>Messsage summarizing the error.</summary>
		public string Messsage => ErrorCode.GetMessage(Args);

		/// <summary>Creates a new instance of the <see cref="SpiceParserError"></see> class.</summary>
		public static SpiceParserError Create(SpiceParserErrorCode errorCode, int lineNumber, int lineColumn,
			params object[] args)
		{
			return new SpiceParserError(errorCode, lineNumber, lineColumn, args);
		}

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return LineColumn * LineNumber == 0 ? Messsage : $"({LineNumber}, {LineColumn}): {Messsage}";
		}
	}
}