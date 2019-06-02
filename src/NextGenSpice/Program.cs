using System;
using System.IO;
using System.Linq;
using NextGenSpice.Core.Exceptions;
using NextGenSpice.Parser;
using NextGenSpice.Parser.Statements.Printing;
using NextGenSpice.Printing;
using NextGenSpice.Simulation;

namespace NextGenSpice
{
	internal class Program
	{
		private static int Main(string[] args)
		{
			if (args.Length != 1)
			{
				Console.Error.WriteLine("Usage: dotnet NextGenSpice.dll <input file>");
				return 1;
			}

			StreamReader input;
			try
			{
				input = new StreamReader(args[0]);
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(e.Message);
				return 1;
			}

			var parser = CreateParser();

			var result = parser.Parse(input);

			if (result.HasError)
			{
				// display errors and exit
				foreach (var error in result.Errors)
					Console.WriteLine(error);
				Console.WriteLine($"There were {result.Errors.Count} errors.");
				return 1;
			}

			var first = true;
			var simStatements = result.OtherStatements.OfType<ISimulationStatement>();
			foreach (var statement in simStatements)
			{
				if (!first) Console.WriteLine();
				first = false;
				try
				{
					statement.Simulate(result.CircuitDefinition, result.OtherStatements.OfType<PrintStatement>(), Console.Out);
				}
				catch (PrinterInitializationException e)
				{
					foreach (var error in e.Errors)
						Console.WriteLine(error);
				}
				catch (SimulationException e)
				{
					Console.WriteLine($"ERROR: {e.Message}");
					return 1;
				}
			}

			return 0;
		}

		private static SpiceNetlistParser CreateParser()
		{
			var parser = SpiceNetlistParser.WithDefaults();

			// add additional processors
			var tran = new TranStatementProcessor();
			var op = new OpStatementProcessor();
			var print = new PrintStatementProcessor();
			print.AddHandler(tran.GetPrintStatementHandler());
			print.AddHandler(op.GetPrintStatementHandler());

			parser.RegisterStatement(tran, true, false);
			parser.RegisterStatement(print, true, false);
			parser.RegisterStatement(op, true, false);
			return parser;
		}
	}
}