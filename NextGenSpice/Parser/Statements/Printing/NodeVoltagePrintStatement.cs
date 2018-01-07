using System.IO;
using NextGenSpice.LargeSignal;

namespace NextGenSpice.Parser.Statements.Printing
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

    /// <summary>
    /// Print statement corresponding to printing voltage of a certain node.
    /// </summary>
    class NodeVoltageDifferencePrintStatement : PrintStatement<LargeSignalCircuitModel>
    {
        private readonly string nodeNames;
        private readonly int i1;
        private readonly int i2;
        private LargeSignalCircuitModel model;

        public NodeVoltageDifferencePrintStatement(string nodeNames, int i1, int i2)
        {
            this.nodeNames = nodeNames;
            this.i1 = i1;
            this.i2 = i2;
        }

        /// <summary>
        /// Information about what kind of data are handled by this print statement.
        /// </summary>
        public override string Header => $"V({nodeNames})";

        /// <summary>
        /// Prints value of handled by this print statement into given TextWriter.
        /// </summary>
        /// <param name="output"></param>
        public override void PrintValue(TextWriter output)
        {
            output.Write(model.NodeVoltages[i1] - model.NodeVoltages[i2]);
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