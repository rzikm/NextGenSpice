using System.Linq;

namespace NextGenSpice
{
    public class OpStatementProcessor : SimpleStatementProcessor<OpSimulationParams>, ISimulationStatementProcessor
    {
        public OpStatementProcessor()
        {
            MinArgs = 0;
            MaxArgs = 0;
            
        }
        public override string Discriminator => ".OP";

        public IPrintStatementHandler GetPrintStatementHandler()
        {
            return new LsPrintStatementHandler("OP");
        }

        protected override void InitMapper()
        {
            Mapper.Target = new OpSimulationParams();
        }

        protected override void UseParam()
        {
            Context.SimulationStatements.Add(new OpSimulationStatement(Mapper.Target, Context.SymbolTable.NodeIndices.ToDictionary(kvp=> kvp.Value, kvp=> kvp.Key)));
            Mapper.Target = null;
        }
    }
}