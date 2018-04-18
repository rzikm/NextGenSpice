using System;
using System.IO;
using System.Linq;
using NextGenSpice.Core.Exceptions;
using NextGenSpice.Core.Parser;
using NextGenSpice.Core.Parser.Statements.Printing;
using NextGenSpice.Printing;
using NextGenSpice.Simulation;

namespace NextGenSpice
{
    internal class Program
    {
        private static void Main(string[] args)
        {
//            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: NextGenSpice <input file>");
                return;
            }

            StreamReader input;
            try
            {
                input = new StreamReader(args[0]);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return;
            }

            var parser = CreateParser();

            var result = parser.Parse(new TokenStream(input));

            if (result.HasError)
            {
                // display errors and exit
                foreach (var error in result.Errors)
                    Console.WriteLine(error);
                Console.WriteLine($"There were {result.Errors.Count} errors.");
                return;
            }

            bool first = true;
            var simStatements = result.OtherStatements.OfType<ISimulationStatement>();
            foreach (var statement in simStatements)
            {
                if (!first) Console.WriteLine();
                first = false;
                RunStatement(statement, result);
            }
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

        private static void RunStatement(ISimulationStatement statement, SpiceNetlistParserResult result)
        {
            try
            {
                statement.Simulate(result.CircuitDefinition, result.OtherStatements.OfType<PrintStatement>(), Console.Out);
            }
            catch (PrinterInitializationException e)
            {
                foreach (var error in e.Errors)
                    Console.WriteLine(error);
            }
            catch (NonConvergenceException e)
            {
                Console.WriteLine("ERROR: Simulation did not converge");
            }
        }
    }
}