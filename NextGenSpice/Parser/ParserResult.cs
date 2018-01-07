using System.Collections.Generic;
using NextGenSpice.Core.Representation;
using NextGenSpice.Parser.Statements.Printing;
using NextGenSpice.Parser.Statements.Simulation;
using NextGenSpice.Utils;

namespace NextGenSpice.Parser
{
    /// <summary>
    /// Class holding result of input file parsing
    /// </summary>
    public class ParserResult
    {
        public ParserResult(ICircuitDefinition circuit, IReadOnlyList<PrintStatement> printStatements, IReadOnlyList<ISimulationStatement> simulationStatements, IReadOnlyList<ErrorInfo> errors)
        {
            CircuitDefinition = circuit;
            PrintStatements = printStatements;
            SimulationStatements = simulationStatements;
            Errors = errors;
        }

        /// <summary>
        /// Circuit defined in the input file. Is null if there was an error in input file.
        /// </summary>
        public ICircuitDefinition CircuitDefinition { get; }

        /// <summary>
        /// All print statements from the input file.
        /// </summary>
        public IReadOnlyList<PrintStatement> PrintStatements { get; }

        /// <summary>
        /// All siulations requested in the input file.
        /// </summary>
        public IReadOnlyList<ISimulationStatement> SimulationStatements { get; }

        /// <summary>
        /// Set of errors encountered in input file.
        /// </summary>
        public IReadOnlyList<ErrorInfo> Errors { get; }

        /// <summary>
        /// Indicates that parsing was not successful and there were some errors in the input file.
        /// </summary>
        public bool HasError => Errors.Count > 0;
    }
}