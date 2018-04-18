using System.Collections.Generic;
using System.IO;
using NextGenSpice.Core.Representation;
using NextGenSpice.Printing;

namespace NextGenSpice.Simulation
{
    /// <summary>Defines methods for performing an analysis of a circuit and printing the results.</summary>
    public interface ISimulationStatement
    {
        /// <summary>Performs the simulation and prints results to specified TextWriter.</summary>
        /// <param name="circuit">Circuit on which analysis should be performed.</param>
        /// <param name="printStatements">Set of all requested print statements that were requested in SPICE input file.</param>
        /// <param name="output">TextWriter instance to which the results should be written.</param>
        void Simulate(ICircuitDefinition circuit, IEnumerable<PrintStatement> printStatements, TextWriter output);
    }
}