using System.Collections.Generic;
using System.IO;
using System.Linq;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;
using NextGenSpice.Parser.Utils;

namespace NextGenSpice.Printing
{
    /// <summary>Print statement corresponding to printing voltage across a two terminal device.</summary>
    internal class DeviceVoltagePrintStatement : PrintStatement<LargeSignalCircuitModel>
    {
        private readonly string elemName;
        private ITwoTerminalLargeSignalDevice model;

        public DeviceVoltagePrintStatement(string elemName)
        {
            this.elemName = elemName;
        }

        /// <summary>Information about what kind of data are handled by this print statement.</summary>
        public override string Header => $"V({elemName})";

        /// <summary>Prints value of handled by this print statement into given TextWriter.</summary>
        /// <param name="output">Output TextWriter where to write.</param>
        public override void PrintValue(TextWriter output)
        {
            output.Write(model.Voltage);
        }

        /// <summary>Initializes print statement for given circuit model and returns set of errors that occured (if any).</summary>
        /// <param name="circuitModel">Current model of the circuit.</param>
        /// <returns>Set of errors that errored (if any).</returns>
        public override IEnumerable<SpiceParserError> Initialize(LargeSignalCircuitModel circuitModel)
        {
            this.model = (ITwoTerminalLargeSignalDevice) circuitModel.GetDevice(elemName);
            return Enumerable.Empty<SpiceParserError>();
        }
    }
}