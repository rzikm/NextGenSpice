using System.Linq;

namespace NextGenSpice
{
    /// <summary>
    /// Class for processing .OP simulation statements.
    /// </summary>
    public class OpStatementProcessor : SimpleStatementProcessor<OpSimulationParams>, ISimulationStatementProcessor
    {
        public OpStatementProcessor()
        {
            MinArgs = 0;
            MaxArgs = 0;
            
        }

        /// <summary>
        /// Statement discriminator, that this class can handle.
        /// </summary>
        public override string Discriminator => ".OP";

        /// <summary>
        /// Gets handler that can handle .PRINT statements that belong to analysis of this processor
        /// </summary>
        /// <returns></returns>
        public IPrintStatementHandler GetPrintStatementHandler()
        {
            return LsPrintStatementHandler.CreateOp();
        }

        /// <summary>
        /// Initializes mapper target (instance hodling the param values), including default parameters.
        /// </summary>
        protected override void InitMapper()
        {
            Mapper.Target = new OpSimulationParams();
        }

        /// <summary>
        /// Final action for processing the statement
        /// </summary>
        protected override void UseParam()
        {
            Context.SimulationStatements.Add(new OpSimulationStatement(Mapper.Target, Context.SymbolTable.NodeIndices.ToDictionary(kvp=> kvp.Value, kvp=> kvp.Key)));
            Mapper.Target = null;
        }
    }
}