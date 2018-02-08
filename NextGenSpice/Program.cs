﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Exceptions;
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
            parser.RegisterElement(new ResistorStatementProcessor());
            parser.RegisterElement(new CurrentSourceStatementProcessor());
            parser.RegisterElement(new VoltageSourceStatementProcessor());

            parser.RegisterElement(new CapacitorStatementProcessor());
            parser.RegisterElement(new InductorStatementProcessor());

            parser.RegisterElement(new DiodeStatementProcessor());
            parser.RegisterElement(new BjtStatementProcessor());


            parser.RegisterSimulation(new TranStatementProcessor());
            parser.RegisterSimulation(new OpStatementProcessor());
        }

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

            bool first = true;
            foreach (var statement in result.SimulationStatements)
            {
                if (!first) Console.WriteLine();
                first = false;
                RunStatement(statement, result);
            }
        }

        private static void RunStatement(ISimulationStatement statement, ParserResult result)
        {
            try
            {
                statement.Simulate(result.CircuitDefinition, result.PrintStatements, Console.Out);
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