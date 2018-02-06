using System.Collections.Generic;
using System.IO;
using System.Linq;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;
using NextGenSpice.Utils;

namespace NextGenSpice.Parser.Statements.Printing
{
    /// <summary>
    ///     Print statement corresponding to printing current flowing through a two terminal device.
    /// </summary>
    internal class ElementCurrentPrintStatement : PrintStatement<LargeSignalCircuitModel>
    {
        private readonly string elemName;
        private ITwoTerminalLargeSignalDeviceModel model;

        public ElementCurrentPrintStatement(string elemName)
        {
            this.elemName = elemName;
        }

        /// <summary>
        ///     Information about what kind of data are handled by this print statement.
        /// </summary>
        public override string Header => $"I({elemName})";

        /// <summary>
        ///     Prints value of handled by this print statement into given TextWriter.
        /// </summary>
        /// <param name="output">Output TextWriter where to write.</param>
        public override void PrintValue(TextWriter output)
        {
            output.Write(model.Current);
        }

        /// <summary>
        ///     Initializes print statement for given circuit model and returns set of errors that occured (if any).
        /// </summary>
        /// <param name="circuitModel">Current model of the circuit.</param>
        /// <returns>Set of errors that errored (if any).</returns>
        public override IEnumerable<ErrorInfo> Initialize(LargeSignalCircuitModel circuitModel)
        {
            this.model = (ITwoTerminalLargeSignalDeviceModel) circuitModel.GetElement(elemName);
            return Enumerable.Empty<ErrorInfo>();
        }
    }
}