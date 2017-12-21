using System;

namespace NextGenSpice
{
    public class TranStatementProcessor : SimpleStatementProcessor<TranSimulationStatement>
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
            Mapper.Target = new TranSimulationStatement();
        }

        protected override void UseParam()
        {
            Mapper.Target.AnalysisType = "TRAN";
            throw new NotImplementedException();
        }
    }
}