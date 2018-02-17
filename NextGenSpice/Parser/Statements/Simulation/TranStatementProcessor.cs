using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Parser.Statements.Printing;

namespace NextGenSpice.Parser.Statements.Simulation
{
    /// <summary>
    ///     Class for handling .TRAN statements.
    /// </summary>
    public class TranStatementProcessor : SimpleStatementProcessor<TranSimulationParams>, ISimulationStatementProcessor
    {
        public TranStatementProcessor()
        {
            MinArgs = 2;
            MaxArgs = 4;

            Mapper.Map(c => c.TimeStep, 1);
            Mapper.Map(c => c.StopTime, 2);
            Mapper.Map(c => c.StartTime, 3);

            // not supported yet, the timestep is constant and equal to TimeStep parameter
//            Mapper.Map(c => c.MaximmumTimestep, 4);
        }

        /// <summary>
        ///     Statement discriminator, that this class can handle.
        /// </summary>
        public override string Discriminator => ".TRAN";

        /// <summary>
        ///     Gets handler that can handle .PRINT statements that belong to analysis of this processor
        /// </summary>
        /// <returns></returns>
        public IPrintStatementHandler GetPrintStatementHandler()
        {
            return LsPrintStatementHandler.CreateTran();
        }

        /// <summary>
        ///     Initializes mapper target (instance hodling the param values), including default parameters.
        /// </summary>
        protected override void InitMapper()
        {
            Mapper.Target = new TranSimulationParams();
        }

        /// <summary>
        ///     Final action for processing the statement
        /// </summary>
        protected override void UseParam()
        {
            Dictionary<int, string> names = new Dictionary<int, string>();

            for (int i = 0; i < Context.CircuitBuilder.NodeCount; i++)
            {
                names[i] = Context.SymbolTable.GetNodeNames(new[] { i }).Single();
            }

            Context.SimulationStatements.Add(new TranSimulationStatement(Mapper.Target, names));
        }
    }
}