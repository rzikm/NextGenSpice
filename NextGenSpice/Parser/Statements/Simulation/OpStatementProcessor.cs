using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Parser.Statements.Printing;

namespace NextGenSpice.Parser.Statements.Simulation
{
    /// <summary>
    ///     Class for processing .OP simulation statements.
    /// </summary>
    public class OpStatementProcessor : SimpleStatementProcessor<OpSimulationParams>, ISimulationStatementProcessor
    {
        public OpStatementProcessor()
        {
            MinArgs = 0;
            MaxArgs = 0;
        }

        /// <summary>
        ///     Statement discriminator, that this class can handle.
        /// </summary>
        public override string Discriminator => ".OP";

        /// <summary>
        ///     Gets handler that can handle .PRINT statements that belong to analysis of this processor
        /// </summary>
        /// <returns></returns>
        public IPrintStatementHandler GetPrintStatementHandler()
        {
            return LsPrintStatementHandler.CreateOp();
        }

        /// <summary>
        ///     Initializes mapper target (instance hodling the param values), including default parameters.
        /// </summary>
        protected override void InitMapper()
        {
            Mapper.Target = new OpSimulationParams();
        }

        /// <summary>
        ///     Final action for processing the statement
        /// </summary>
        protected override void UseParam()
        {
            Dictionary<int, string> names = new Dictionary<int, string>();

            for (int i = 0; i < Context.CircuitBuilder.NodeCount; i++)
            {
                names[i] = Context.SymbolTable.GetNodeNames(new[] {i}).Single();
            }

            Context.SimulationStatements.Add(new OpSimulationStatement(Mapper.Target, names));
        }
    }
}