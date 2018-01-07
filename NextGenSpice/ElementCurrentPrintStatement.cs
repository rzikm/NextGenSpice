using System.IO;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpice
{
    /// <summary>
    /// Print statement corresponding to printing current flowing through a two terminal device.
    /// </summary>
    class ElementCurrentPrintStatement : PrintStatement<LargeSignalCircuitModel>
    {
        private readonly string elemName;
        private ITwoTerminalLargeSignalDeviceModel model;

        public ElementCurrentPrintStatement(string elemName)
        {
            this.elemName = elemName;
        }

        /// <summary>
        /// Information about what kind of data are handled by this print statement.
        /// </summary>
        public override string Header => $"I({elemName})";

        /// <summary>
        /// Prints value of handled by this print statement into given TextWriter.
        /// </summary>
        /// <param name="output"></param>
        public override void PrintValue(TextWriter output)
        {
            output.Write(model.Current);
        }

        /// <summary>
        /// Sets analysis type circuit model from which data for printing are to be extracted.
        /// </summary>
        /// <param name="model"></param>
        public override void Initialize(LargeSignalCircuitModel model)
        {
            this.model = (ITwoTerminalLargeSignalDeviceModel)model.GetElement(elemName);
        }
    }
}