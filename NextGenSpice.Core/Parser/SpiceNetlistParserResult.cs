using System.Collections.Generic;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Parser.Statements;
using NextGenSpice.Core.Parser.Utils;
using NextGenSpice.Core.Representation;

namespace NextGenSpice.Core.Parser
{
    /// <summary>Class holding result of input file parsing</summary>
    public class SpiceNetlistParserResult
    {
        public SpiceNetlistParserResult(string title, ICircuitDefinition circuit,
            IReadOnlyList<SpiceStatement> otherStatements,
            IReadOnlyList<ErrorInfo> errors,
            IEnumerable<ISubcircuitDefinition> subcircuits, IReadOnlyList<string> nodeNames)
        {
            CircuitDefinition = circuit;
            Errors = errors;
            Subcircuits = subcircuits;
            NodeNames = nodeNames;
            Title = title;
            OtherStatements = otherStatements;
        }

        /// <summary>Circuit defined in the input file. Is null if there was an error in input file.</summary>
        public ICircuitDefinition CircuitDefinition { get; }

        /// <summary>The set of subcircuits that were defined</summary>
        public IEnumerable<ISubcircuitDefinition> Subcircuits { get; }

        /// <summary>Names used in the netlist to refer to circuit nodes, indexed by node id.</summary>
        public IReadOnlyList<string> NodeNames { get; }

        /// <summary>All statements that do not directly influence the circuit description.</summary>
        public IReadOnlyList<SpiceStatement> OtherStatements { get; }

        /// <summary>Set of errors encountered in input file.</summary>
        public IReadOnlyList<ErrorInfo> Errors { get; }

        /// <summary>Title of the netlist file</summary>
        public string Title { get; }

        /// <summary>Indicates that parsing was not successful and there were some errors in the input file.</summary>
        public bool HasError => Errors.Count > 0;
    }
}