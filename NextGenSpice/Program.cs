using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal;
using NextGenSpice.Parser;
using NextGenSpice.Parser.Statements.Devices;
using NextGenSpice.Parser.Statements.Simulation;

namespace NextGenSpice
{
    internal class Program
    {
        private static void RegisterStatementProcessors(SpiceCodeParser parser)
        {
            parser.RegisterElement(new CurrentSourceStatementProcessor());
            parser.RegisterElement(new VoltageSourceStatementProcessor());
            parser.RegisterElement(new ResistorStatementProcessor());
            parser.RegisterElement(new DiodeStatementProcessor());
            parser.RegisterElement(new CapacitorStatementProcessor());
            parser.RegisterElement(new InductorStatementProcessor());

            parser.RegisterSimulation(new TranStatementProcessor());
            parser.RegisterSimulation(new OpStatementProcessor());
        }

        private static void Main(string[] args)
        {
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

            var parser = new SpiceCodeParser();
            RegisterStatementProcessors(parser);
            var result = parser.Parse(new TokenStream(input));

            if (result.HasError)
            {
                // display errors and exit
                foreach (var error in result.Errors)
                    Console.WriteLine(error);
                Console.WriteLine($"There were {result.Errors.Count} errors.");
                return;
            }

            foreach (var statement in result.SimulationStatements)
                statement.Simulate(result.CircuitDefinition, result.PrintStatements, Console.Out);
        }
    }
}