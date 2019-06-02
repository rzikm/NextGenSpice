using NextGenSpice.Parser.Statements;
using NextGenSpice.Parser.Statements.Printing;
using NextGenSpice.Printing;

namespace NextGenSpice.Simulation
{
	/// <summary>Class for processing .OP simulation statements.</summary>
	public class OpStatementProcessor : SimpleDotStatementProcessor<OpStatementParam>
	{
		public OpStatementProcessor()
		{
			MinArgs = 0;
			MaxArgs = 0;
		}

		/// <summary>Statement discriminator, that this class can handle.</summary>
		public override string Discriminator => ".OP";

		/// <summary>Gets handler that can handle .PRINT statements that belong to analysis of this processor</summary>
		/// <returns></returns>
		public IPrintStatementHandler GetPrintStatementHandler()
		{
			return LsPrintStatementHandler.CreateOp();
		}

		/// <summary>Initializes mapper target (instance hodling the param values), including default parameters.</summary>
		protected override void InitMapper()
		{
			Mapper.Target = new OpStatementParam();
		}

		/// <summary>Final action for processing the statement</summary>
		protected override void UseParam()
		{
			Context.OtherStatements.Add(new OpSimulationStatement(Mapper.Target, Context.SymbolTable.GetNodeIdMappings()));
		}
	}
}