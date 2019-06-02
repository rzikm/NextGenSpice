using NextGenSpice.Parser.Statements;
using NextGenSpice.Printing;
using IPrintStatementHandler = NextGenSpice.Parser.Statements.Printing.IPrintStatementHandler;

namespace NextGenSpice.Simulation
{
    /// <summary>Class for handling .TRAN statements.</summary>
    public class TranStatementProcessor : SimpleDotStatementProcessor<TranStatementParam>//, ISimulationStatementProcessor
    {
        public TranStatementProcessor()
        {
            MinArgs = 2;
            MaxArgs = 4;

            Mapper.Map(c => c.TimeStep, 0);
            Mapper.Map(c => c.StopTime, 1);
            Mapper.Map(c => c.StartTime, 2);

            // not supported yet, the timestep is constant and equal to TimeStep parameter
//            Mapper.Map(c => c.MaximmumTimestep, 3);
        }

        /// <summary>Statement discriminator, that this class can handle.</summary>
        public override string Discriminator => ".TRAN";

        /// <summary>Gets handler that can handle .PRINT statements that belong to analysis of this processor</summary>
        /// <returns></returns>
        public IPrintStatementHandler GetPrintStatementHandler()
        {
            return LsPrintStatementHandler.CreateTran();
        }

        /// <summary>Initializes mapper target (instance hodling the param values), including default parameters.</summary>
        protected override void InitMapper()
        {
            Mapper.Target = new TranStatementParam();
        }

        /// <summary>Final action for processing the statement</summary>
        protected override void UseParam()
        {
            Context.OtherStatements.Add(new TranSimulationStatement(Mapper.Target, Context.SymbolTable.GetNodeIdMappings()));
        }
    }
}