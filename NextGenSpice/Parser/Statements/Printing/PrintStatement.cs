using System.IO;

namespace NextGenSpice.Parser.Statements.Printing
{
    /// <summary>
    /// Class that represent a parsed .PRINT statement for certain analysis type.
    /// </summary>
    public abstract class PrintStatement
    {
        /// <summary>
        /// Sets analysis type circuit model from which data for printing are to be extracted.
        /// </summary>
        /// <param name="model"></param>
        public abstract void Initialize(object model);

        /// <summary>
        /// Information about what kind of data are handled by this print statement.
        /// </summary>
        public abstract string Header { get; }

        /// <summary>
        /// Prints value of handled by this print statement into given TextWriter.
        /// </summary>
        /// <param name="output"></param>
        public abstract void PrintValue(TextWriter output);

        /// <summary>
        /// Analysis type during which this statement should take effect.
        /// </summary>
        public string AnalysisType { get; set; }
    }

    /// <summary>
    /// Class that represent a parsed .PRINT statement for certain analysis type.
    /// Generic type variant of <see cref="PrintStatement"/>.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public abstract class PrintStatement<TModel> : PrintStatement
    {
        protected PrintStatement()
        {
        }

        /// <summary>
        /// Sets analysis type circuit model from which data for printing are to be extracted.
        /// </summary>
        /// <param name="model"></param>
        public override void Initialize(object model)
        {
            Initialize((TModel)model);
        }

        /// <summary>
        /// Sets analysis type circuit model from which data for printing are to be extracted.
        /// </summary>
        /// <param name="model"></param>
        public abstract void Initialize(TModel model);
    }
}