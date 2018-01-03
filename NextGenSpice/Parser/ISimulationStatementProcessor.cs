namespace NextGenSpice
{
    public interface ISimulationStatementProcessor : IStatementProcessor 
    {
        IPrintStatementHandler GetPrintStatementHandler();
    }
}