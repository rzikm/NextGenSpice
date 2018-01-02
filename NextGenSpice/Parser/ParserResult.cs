using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Representation;

namespace NextGenSpice
{
    public class ParserResult
    {
        public ParserResult(ICircuitDefinition circuit, IReadOnlyList<PrintStatement> printStatements, IReadOnlyList<SimulationStatement> simulationStatements, IReadOnlyList<ErrorInfo> errors)
        {
            CircuitDefinition = circuit;
            PrintStatements = printStatements;
            SimulationStatements = simulationStatements;
            Errors = errors;
        }

        public ICircuitDefinition CircuitDefinition { get; }
        public IReadOnlyList<PrintStatement> PrintStatements { get; }
        public IReadOnlyList<SimulationStatement> SimulationStatements { get; }
        public IReadOnlyList<ErrorInfo> Errors { get; }

        public bool HasError => Errors.Count > 0;
    }
}