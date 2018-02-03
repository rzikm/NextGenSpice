using System.IO;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpice.Parser.Statements.Printing
{
    /// <summary>
    ///     Print statement corresponding to printing voltage across a two terminal device.
    /// </summary>
    internal class ElementVoltagePrintStatement : PrintStatement<LargeSignalCircuitModel>
    {
        private readonly string elemName;
        private ITwoTerminalLargeSignalDeviceModel model;

        public ElementVoltagePrintStatement(string elemName)
        {
            this.elemName = elemName;
        }

        /// <summary>
        ///     Information about what kind of data are handled by this print statement.
        /// </summary>
        public override string Header => $"V({elemName})";

        /// <summary>
        ///     Prints value of handled by this print statement into given TextWriter.
        /// </summary>
        /// <param name="output"></param>
        public override void PrintValue(TextWriter output)
        {
            output.Write(model.Voltage);
        }

        /// <summary>
        ///     Sets analysis type circuit model from which data for printing are to be extracted.
        /// </summary>
        /// <param name="model"></param>
        public override void Initialize(LargeSignalCircuitModel model)
        {
            this.model = (ITwoTerminalLargeSignalDeviceModel) model.GetElement(elemName);
        }
    }
}