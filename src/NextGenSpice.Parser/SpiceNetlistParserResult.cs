using System;
using System.Collections.Generic;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Representation;
using NextGenSpice.Parser.Statements;
using NextGenSpice.Parser.Utils;

namespace NextGenSpice.Parser
{
	/// <summary>Class holding result of input file parsing</summary>
	public class SpiceNetlistParserResult
	{
		public SpiceNetlistParserResult(string title, ICircuitDefinition circuit,
			IReadOnlyList<SpiceStatement> otherStatements,
			IReadOnlyList<SpiceParserError> errors,
			IEnumerable<ISubcircuitDefinition> subcircuits, IReadOnlyList<string> nodeNames,
			IReadOnlyDictionary<Type, IReadOnlyDictionary<string, object>> models)
		{
			CircuitDefinition = circuit;
			Errors = errors;
			Subcircuits = subcircuits;
			NodeNames = nodeNames;
			Models = models;
			Title = title;
			OtherStatements = otherStatements;

			NodeIds = new Dictionary<string, int>(nodeNames.Count);
			for (var i = 0; i < nodeNames.Count; i++) NodeIds[nodeNames[i]] = i;
		}

		/// <summary>Circuit defined in the input file. Is null if there was an error in input file.</summary>
		public ICircuitDefinition CircuitDefinition { get; }

		/// <summary>The set of subcircuits that were defined</summary>
		public IEnumerable<ISubcircuitDefinition> Subcircuits { get; }

		/// <summary>Collection of all models from the netlist file.</summary>
		public IReadOnlyDictionary<Type, IReadOnlyDictionary<string, object>> Models { get; }

		/// <summary>Names used in the netlist to refer to circuit nodes, indexed by node id.</summary>
		public IReadOnlyList<string> NodeNames { get; }

		/// <summary>Mapping from node names from the netlist file to the node ids.</summary>
		public IDictionary<string, int> NodeIds { get; }

		/// <summary>All statements that do not directly influence the circuit description.</summary>
		public IReadOnlyList<SpiceStatement> OtherStatements { get; }

		/// <summary>Set of errors encountered in input file.</summary>
		public IReadOnlyList<SpiceParserError> Errors { get; }

		/// <summary>Title of the netlist file</summary>
		public string Title { get; }

		/// <summary>Indicates that parsing was not successful and there were some errors in the input file.</summary>
		public bool HasError => Errors.Count > 0;
	}
}