using System.IO;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpice
{
    class ElementCurrentPrintStatement : LsPrintStatement
    {
        private readonly string elemName;
        private ITwoTerminalLargeSignalDeviceModel model;

        public ElementCurrentPrintStatement(string elemName)
        {
            this.elemName = elemName;
        }

        public override void PrintHeader(TextWriter output)
        {
            output.Write($"V({elemName})");
        }

        public override void PrintValue(TextWriter output)
        {
            output.Write(model.Current);
        }

        public override void Initialize(LargeSignalCircuitModel model)
        {
            this.model = (ITwoTerminalLargeSignalDeviceModel)model.GetElement(elemName);
        }
    }
}