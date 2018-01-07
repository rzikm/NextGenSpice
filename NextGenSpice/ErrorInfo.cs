namespace NextGenSpice
{
    /// <summary>
    /// Provides basic information about an error that occured during parsing of an input file.
    /// </summary>
    public class ErrorInfo
    {
        /// <summary>
        /// Index of a line from input file on which error occured.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// One-based index of character where the error occured.
        /// </summary>
        public int LineColumn { get; set; }

        /// <summary>
        /// Messsage summarizing the error.
        /// </summary>
        public string Messsage { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return LineColumn * LineNumber == 0 ? Messsage : $"({LineNumber}, {LineColumn}): {Messsage}";
        }
    }
}