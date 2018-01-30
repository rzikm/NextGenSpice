using NextGenSpice.Parser.Statements.Printing;

namespace NextGenSpice.Parser.Statements.Simulation
{
    /// <summary>
    /// Defines a method for getting print statement handler
    /// </summary>
    public interface ISimulationStatementProcessor : IStatementProcessor 
    {
        /// <summary>
        /// Gets handler that can handle .PRINT statements that belong to analysis of this processor
        /// </summary>
        /// <returns></returns>
        IPrintStatementHandler GetPrintStatementHandler();
    }
}