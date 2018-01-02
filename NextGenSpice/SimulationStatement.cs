using System.Collections.Generic;
using System.IO;
using NextGenSpice.Core.Representation;

namespace NextGenSpice
{
    public abstract class SimulationStatement
    {
        public abstract void Simulate(ICircuitDefinition circuit, IEnumerable<PrintStatement> printStatements, TextWriter output);
    }
}