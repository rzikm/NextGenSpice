namespace NextGenSpice
{
    public class ErrorInfo
    {
        public int LineNumber { get; set; }
        public int LineColumn { get; set; }
        public string Messsage { get; set; }

        public override string ToString()
        {
            return $"({LineNumber}, {LineColumn}): {Messsage}";
        }
    }
}