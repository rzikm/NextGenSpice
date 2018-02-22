namespace NextGenSpice.Parser.Statements
{
    /// <summary>
    ///     Class for handling statements that specify initial node voltages.
    /// </summary>
    public class InitialConditionStatement : StatementProcessor
    {
        /// <summary>
        ///     Statement discriminator, that this class can handle.
        /// </summary>
        public override string Discriminator => ".IC";

        /// <summary>
        ///     Processes given statement.
        /// </summary>
        /// <param name="tokens">All tokens of the statement.</param>
        protected override void DoProcess(Token[] tokens)
        {
            throw new System.NotImplementedException();
        }
    }
}