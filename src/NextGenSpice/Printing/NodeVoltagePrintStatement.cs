using System.Collections.Generic;
using System.IO;
using System.Linq;
using NextGenSpice.LargeSignal;
using NextGenSpice.Parser.Utils;

namespace NextGenSpice.Printing
{
	/// <summary>Print statement corresponding to printing voltage of a certain node.</summary>
	internal class NodeVoltagePrintStatement : PrintStatement<LargeSignalCircuitModel>
	{
		private readonly int index;
		private readonly string nodeName;
		private LargeSignalCircuitModel model;

		public NodeVoltagePrintStatement(string nodeName, int index)
		{
			this.nodeName = nodeName;
			this.index = index;
		}

		/// <summary>Information about what kind of data are handled by this print statement.</summary>
		public override string Header => $"V({nodeName})";

		/// <summary>Prints value of handled by this print statement into given TextWriter.</summary>
		/// <param name="output">Output TextWriter where to write.</param>
		public override void PrintValue(TextWriter output)
		{
			output.Write(model.NodeVoltages[index]);
		}

		/// <summary>Initializes print statement for given circuit model and returns set of errors that occured (if any).</summary>
		/// <param name="circuitModel">Current model of the circuit.</param>
		/// <returns>Set of errors that errored (if any).</returns>
		public override IEnumerable<SpiceParserError> Initialize(LargeSignalCircuitModel circuitModel)
		{
			model = circuitModel;
			return Enumerable.Empty<SpiceParserError>();
		}
	}

	/// <summary>Print statement corresponding to printing voltage of a certain node.</summary>
	internal class NodeVoltageDifferencePrintStatement : PrintStatement<LargeSignalCircuitModel>
	{
		private readonly int i1;
		private readonly int i2;
		private readonly string nodeNames;
		private LargeSignalCircuitModel model;

		public NodeVoltageDifferencePrintStatement(string nodeNames, int i1, int i2)
		{
			this.nodeNames = nodeNames;
			this.i1 = i1;
			this.i2 = i2;
		}

		/// <summary>Information about what kind of data are handled by this print statement.</summary>
		public override string Header => $"V({nodeNames})";

		/// <summary>Prints value of handled by this print statement into given TextWriter.</summary>
		/// <param name="output">Output TextWriter where to write.</param>
		public override void PrintValue(TextWriter output)
		{
			output.Write(model.NodeVoltages[i1] - model.NodeVoltages[i2]);
		}

		/// <summary>Initializes print statement for given circuit model and returns set of errors that occured (if any).</summary>
		/// <param name="circuitModel">Current model of the circuit.</param>
		/// <returns>Set of errors that errored (if any).</returns>
		public override IEnumerable<SpiceParserError> Initialize(LargeSignalCircuitModel circuitModel)
		{
			model = circuitModel;
			return Enumerable.Empty<SpiceParserError>();
		}
	}
}