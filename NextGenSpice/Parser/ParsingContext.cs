using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Parser.Statements.Deferring;
using NextGenSpice.Parser.Statements.Printing;
using NextGenSpice.Parser.Statements.Simulation;
using NextGenSpice.Utils;

namespace NextGenSpice.Parser
{
    /// <summary>Class that hold intermediate data during paring of an input file.</summary>
    public class ParsingContext
    {
        private readonly Stack<(CircuitBuilder builder, List<DeferredStatement> stmts)> scopes;

        private readonly SymbolTable table;

        public ParsingContext()
        {
            table = new SymbolTable();
            Errors = new List<ErrorInfo>();
            DeferredStatements = new List<DeferredStatement>();
            SimulationStatements = new List<ISimulationStatement>();
            PrintStatements = new List<PrintStatement>();
            CircuitBuilder = new CircuitBuilder();

            scopes = new Stack<(CircuitBuilder builder, List<DeferredStatement> stmts)>();
        }

        /// <summary>Table containing known symbols from input file.</summary>
        public ISymbolTable SymbolTable => table;

        /// <summary>Set of errors from the input file.</summary>
        public List<ErrorInfo> Errors { get; }

        /// <summary>Set of all syntactically correct staements encountered to be evaluated.</summary>
        public List<DeferredStatement> DeferredStatements { get; private set; }

        /// <summary>Set of all simulation statements encountered.</summary>
        public List<ISimulationStatement> SimulationStatements { get; }

        /// <summary>Set of all .PRINT statements encountered.</summary>
        public List<PrintStatement> PrintStatements { get; }

        /// <summary>Builder responsible for creating the circuit definition from the statements.</summary>
        public CircuitBuilder CircuitBuilder { get; private set; }

        /// <summary>How many nested subcircuits are currently being parsed.</summary>
        public int SubcircuitDepth => table.SubcircuitDepth;

        /// <summary>Temporarily suspends parsing of current circuit and creates new frame to parse a subcircuit.</summary>
        public void EnterSubcircuit()
        {
            scopes.Push((CircuitBuilder, DeferredStatements));
            CircuitBuilder = new CircuitBuilder();
            DeferredStatements = new List<DeferredStatement>();
            table.EnterSubcircuit();
        }

        /// <summary>Restores previous parsing frame.</summary>
        public void ExitSubcircuit()
        {
            (CircuitBuilder, DeferredStatements) = scopes.Pop();
            table.ExitSubcircuit();
        }

        /// <summary>Processes all deferred statements and if they cannot be processed, generate corresponding errors.</summary>
        public void FlushStatements()
        {
            // repeatedly try to process all statements until no more statements can be processed in the iteration
            // these repetitions are there to handle yet unknown statements with dependencies on later statements

            var deferred = DeferredStatements.OrderBy(GetDeferredStatementPriority).ToList();
            do
            {
                DeferredStatements.Clear();
                DeferredStatements.AddRange(deferred);
                deferred.Clear();

                foreach (var statement in DeferredStatements)
                    if (statement.CanApply(this))
                        statement.Apply(this);
                    else
                        deferred.Add(statement);
            } while (deferred.Count < DeferredStatements.Count);

            // get error messages from not processed statements
            foreach (var statement in DeferredStatements)
                Errors.AddRange(statement.GetErrors());

            DeferredStatements.Clear();
        }

        private static int GetDeferredStatementPriority(DeferredStatement statement)
        {
            switch (statement)
            {
                case SimpleElementDeferredStatement _: return 1;
                case DeferredPrintStatement _: return 50; // process these last
                default: return 20; // includes generic ModeledElementStatement<>
            }
        }
    }
}