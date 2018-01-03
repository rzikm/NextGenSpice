using System;

namespace NextGenSpice
{
    public class TranStatementProcessor : SimpleStatementProcessor<TranSimulationParams>, ISimulationStatementProcessor
    {
        public TranStatementProcessor()
        {
            MinArgs = 2;
            MaxArgs = 4;

            Mapper.Map(c => c.TimeStep, 1);
            Mapper.Map(c => c.StopTime, 2);
            Mapper.Map(c => c.StartTime, 3);
            Mapper.Map(c => c.TMax, 4);
        }
        public override string Discriminator => ".TRAN";
        protected override void InitMapper()
        {
            Mapper.Target = new TranSimulationParams();
        }

        protected override void UseParam()
        {
            Context.SimulationStatements.Add(new TranSimulationStatement(Mapper.Target));
            Mapper.Target = null;
        }

        public IPrintStatementHandler GetPrintStatementHandler()
        {
            return new LsPrintStatementHandler("TRAN");
        }
    }
}