using System.Collections.Generic;

namespace NextGenSpice
{
    public class ParserResult
    {
        public ParserResult(IEnumerable<ElementStatement> elementStatements, IEnumerable<PrintStatement> printStatements, IEnumerable<TranSimulationStatement> simulationStatements, IEnumerable<ErrorInfo> errors)
        {
            ElementStatements = elementStatements;
            PrintStatements = printStatements;
            SimulationStatements = simulationStatements;
            Errors = errors;
        }

        public IEnumerable<ElementStatement> ElementStatements { get; }
        public IEnumerable<PrintStatement> PrintStatements { get; }
        public IEnumerable<TranSimulationStatement> SimulationStatements { get; }
        public IEnumerable<ErrorInfo> Errors { get; set; }
    }
}