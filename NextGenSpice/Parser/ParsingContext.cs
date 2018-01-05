using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Circuit;

namespace NextGenSpice
{
    /// <summary>
    /// Class that hold intermediate data during paring of an input file.
    /// </summary>
    public class ParsingContext
    {
        public ParsingContext()
        {
            SymbolTable = new SymbolTable();
            Errors = new List<ErrorInfo>();
            DeferredStatements = new List<DeferredStatement>();
            SimulationStatements = new List<SimulationStatement>();
            PrintStatements = new List<PrintStatement>();
            CircuitBuilder = new CircuitBuilder();
        }
        
        /// <summary>
        /// Table containing known symbols from input file.
        /// </summary>
        public SymbolTable SymbolTable { get;  }

        /// <summary>
        /// Set of errors from the input file.
        /// </summary>
        public List<ErrorInfo> Errors { get;  }

        /// <summary>
        /// Set of all syntactically correct staements encountered to be evaluated.
        /// </summary>
        public List<DeferredStatement> DeferredStatements { get; }

        /// <summary>
        /// Set of all simulation statements encountered.
        /// </summary>
        public List<SimulationStatement> SimulationStatements { get;  }

        /// <summary>
        /// Set of all .PRINT statements encountered.
        /// </summary>
        public List<PrintStatement> PrintStatements { get; }

        /// <summary>
        /// Builder responsible for creating the circuit definition from the statements.
        /// </summary>
        public CircuitBuilder CircuitBuilder { get; }
    }
}