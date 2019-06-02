namespace NextGenSpice.Parser
{
	/// <summary>Class representing occurence of a token in a source file</summary>
	public class Token
	{
		/// <summary>String value of the token.</summary>
		public string Value { get; set; }

		/// <summary>Number of line, on which the token occured in the source file.</summary>
		public int LineNumber { get; set; }

		/// <summary>One based index from beginning of the line, where the token occured in the source file.</summary>
		public int LineColumn { get; set; }

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return $"({LineNumber},{LineColumn}) '{Value}'";
		}
	}
}