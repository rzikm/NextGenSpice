using System.IO;
using NextGenSpice.LargeSignal;

namespace NextGenSpice
{
    /// <summary>
    /// Print statement corresponding to printing voltage of a certain node.
    /// </summary>
    class NodeVoltagePrintStatement : PrintStatement<LargeSignalCircuitModel>
    {
        private readonly string nodeName;
        private readonly int index;
        private LargeSignalCircuitModel model;

        public NodeVoltagePrintStatement(string nodeName, int index)
        {
            this.nodeName = nodeName;
            this.index = index;
        }

        /// <summary>
        /// Information about what kind of data are handled by this print statement.
        /// </summary>
        public override string Header => $"V({nodeName})";

        /// <summary>
        /// Prints value of handled by this print statement into given TextWriter.
        /// </summary>
        /// <param name="output"></param>
        public override void PrintValue(TextWriter output)
        {
            output.Write(model.NodeVoltages[index]);
        }

        /// <summary>
        /// Sets analysis type circuit model from which data for printing are to be extracted.
        /// </summary>
        /// <param name="model"></param>
        public override void Initialize(LargeSignalCircuitModel model)
        {
            this.model = model;
        }
    }
}