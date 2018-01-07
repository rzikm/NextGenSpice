using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NextGenSpice.Core.Circuit;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;
using NextGenSpice.Parser;
using NextGenSpice.Parser.Statements.Devices;
using NextGenSpice.Parser.Statements.Simulation;

namespace NextGenSpice
{
    class Program
    {
        static void Main(string[] args)
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
            parser.RegisterElement(new CurrentSourceStatementProcessor());
            parser.RegisterElement(new VoltageSourceStatementProcessor());
            parser.RegisterElement(new ResistorStatementProcessor());
            parser.RegisterElement(new DiodeStatementProcessor());
            parser.RegisterElement(new CapacitorStatementProcessor());
            parser.RegisterElement(new InductorStatementProcessor());

            parser.RegisterSimulation(new TranStatementProcessor());
            parser.RegisterSimulation(new OpStatementProcessor());

            var result = parser.Parse(new TokenStream(input));

            if (result.HasError)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error);
                }
                Console.WriteLine($"There were {result.Errors.Count} errors.");
                return;
            }

//            TimeSpan total = TimeSpan.Zero;

//            for (int i = -1; i < 10; i++)
//            {
//                Stopwatch sw = Stopwatch.StartNew();

                foreach (var statement in result.SimulationStatements)
                {
                statement.Simulate(result.CircuitDefinition, result.PrintStatements, Console.Out);
//                    statement.Simulate(result.CircuitDefinition, result.PrintStatements, TextWriter.Null);
                }
//                sw.Stop();
//                if (i >= 0)
//                {
//                    Console.WriteLine(sw.Elapsed);
//                    total += sw.Elapsed;
//                }
//            }
//            Console.WriteLine($"Average: {total/10}");

        }
    }
}
