using System.IO;
using NextGenSpice.LargeSignal;

namespace NextGenSpice
{
    class NodeVoltagePrintStatement : LsPrintStatement
    {
        private readonly string nodeName;
        private readonly int index;
        private LargeSignalCircuitModel model;

        public NodeVoltagePrintStatement(string nodeName, int index)
        {
            this.nodeName = nodeName;
            this.index = index;
        }

        public override void PrintHeader(TextWriter output)
        {
            output.Write($"V({nodeName})");
        }

        public override void PrintValue(TextWriter output)
        {
            output.Write(model.NodeVoltages[index]);
        }

        public override void Initialize(LargeSignalCircuitModel model)
        {
            this.model = model;
        }
    }
}