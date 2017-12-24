using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NextGenSpice.Core.Circuit;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;

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
            parser.Register(new CurrentSourceStatementProcessor());
            parser.Register(new VoltageSourceStatementProcessor());
            parser.Register(new ResistorStatementProcessor());

            var result = parser.ParseInputFile(new TokenStream(input));

            if (result.HasError)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error);
                }
                Console.WriteLine($"There were {result.Errors.Count} errors.");
                return;
            }


            return;
            var circuit = result.CircuitDefinition.GetLargeSignalModel();

            List<Func<double>> printers = new List<Func<double>>();

            foreach (var ps in result.PrintStatements)
            {
                var name = ps.Header.Substring(2, ps.Header.Length - 3);
                var element = (ITwoTerminalLargeSignalDeviceModel) circuit.GetModel(name);
                if (ps.Header[0] == 'I')
                    printers.Add(() => element.Current);
                else
                    printers.Add(() => element.Voltage);
            }
        }
    }
}
