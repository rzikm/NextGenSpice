using System.Collections.Generic;
using System.IO;
using System.Linq;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;
using NextGenSpice.Parser;
using NextGenSpice.Parser.Utils;

namespace NextGenSpice.Printing
{
    /// <summary>Class representing a .PRINT statement that prints a certain characteristic of a device.</summary>
    public class DevicePrintStatement : PrintStatement<LargeSignalCircuitModel>
    {
        private readonly string name;
        private readonly string stat;
        private readonly Token t;
        private IDeviceStatsProvider provider;

        public DevicePrintStatement(string stat, string name, Token t)
        {
            this.stat = stat;
            this.name = name;
            this.t = t;
        }

        /// <summary>Information about what kind of data are handled by this print statement.</summary>
        public override string Header => $"{stat}({name})";

        /// <summary>Prints value of handled by this print statement into given TextWriter.</summary>
        /// <param name="output">Output TextWriter where to write.</param>
        public override void PrintValue(TextWriter output)
        {
            output.Write(provider.GetValue());
        }

        /// <summary>Initializes print statement for given circuit model and returns set of errors that occured (if any).</summary>
        /// <param name="circuitModel">Current model of the circuit.</param>
        /// <returns>Set of errors that errored (if any).</returns>
        public override IEnumerable<SpiceParserError> Initialize(LargeSignalCircuitModel circuitModel)
        {
            var model = circuitModel.FindDevice(name);
            provider = model.GetDeviceStatsProviders().SingleOrDefault(pr => pr.StatName == stat);
            var errorInfos = provider == null
                ? new[]
                {
                    SpiceParserError.Create(Parser.SpiceParserErrorCode.NoPrintProvider, 0, 0, stat, name)
                }
                : Enumerable.Empty<SpiceParserError>();
            return errorInfos;
        }
    }
}