namespace NextGenSpice
{
    /// <summary>
    /// Class representing occurence of a token in a source file
    /// </summary>
    public class Token
    {
        /// <summary>
        /// String value of the token.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Number of line, on which the token occured in the source file.
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Offset from beginning of the line, where the token occured in the source file.
        /// </summary>
        public int Char { get; set; }
    }
}