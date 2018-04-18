using System.Collections.Generic;
using System.IO;
using NextGenSpice.Core.Parser.Statements;
using NextGenSpice.Core.Parser.Utils;

namespace NextGenSpice.Printing
{
    /// <summary>Class that represent a parsed .PRINT statement for certain analysis type.</summary>
    public abstract class PrintStatement : SpiceStatement
    {
        /// <summary>Information about what kind of data are handled by this print statement.</summary>
        public abstract string Header { get; }

        /// <summary>Analysis type during which this statement should take effect.</summary>
        public string AnalysisType { get; set; }

        /// <summary>Initializes print statement for given circuit model and returns set of errors that occured (if any).</summary>
        /// <param name="circuitModel">Current model of the circuit.</param>
        /// <returns>Set of errors that errored (if any).</returns>
        public abstract IEnumerable<ErrorInfo> Initialize(object circuitModel);

        /// <summary>Prints value of handled by this print statement into given TextWriter.</summary>
        /// <param name="output">Output TextWriter where to write.</param>
        public abstract void PrintValue(TextWriter output);
    }


    /// <summary>
    ///     Class that represent a parsed .PRINT statement for certain analysis type. Generic type variant of
    ///     <see cref="PrintStatement" />.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public abstract class PrintStatement<TModel> : PrintStatement
    {
        /// <summary>Initializes print statement for given circuit model and returns set of errors that occured (if any).</summary>
        /// <param name="circuitModel">Current model of the circuit.</param>
        /// <returns>Set of errors that errored (if any).</returns>
        public override IEnumerable<ErrorInfo> Initialize(object circuitModel)
        {
            return Initialize((TModel) circuitModel);
        }

        /// <summary>Initializes print statement for given circuit model and returns set of errors that occured (if any).</summary>
        /// <param name="circuitModel">Current model of the circuit.</param>
        /// <returns>Set of errors that errored (if any).</returns>
        public abstract IEnumerable<ErrorInfo> Initialize(TModel circuitModel);
    }
}