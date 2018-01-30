using System.Collections.Generic;
using System.IO;
using System.Linq;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;
using NextGenSpice.Parser.Statements.Printing;

namespace NextGenSpice.Parser.Statements.Simulation
{
    /// <summary>
    /// Class representing a call to .OP statement.
    /// </summary>
    public class OpSimulationStatement : ISimulationStatement
    {
        private readonly OpSimulationParams param;
        private readonly Dictionary<int, string> nodeNames;

        public OpSimulationStatement(OpSimulationParams param, Dictionary<int, string> nodeNames)
        {
            this.param = param;
            this.nodeNames = nodeNames;
        }


        /// <summary>
        /// Performs the simulation and prints results to specified TextWriter.
        /// </summary>
        /// <param name="circuit">Circuit on which analysis should be performed.</param>
        /// <param name="printStatements">Set of all requested print statements that were requested in SPICE input file.</param>
        /// <param name="output">TextWriter instance to which the results should be written.</param>
        public void Simulate(ICircuitDefinition circuit, IEnumerable<PrintStatement> printStatements, TextWriter output)
        {
            var model = circuit.GetLargeSignalModel();
            var prints = printStatements.OfType<PrintStatement<LargeSignalCircuitModel>>()
                .Where(s => s.AnalysisType == "OP").ToList();
            model.EstablishDcBias();
            
            if (prints.Count == 0)
            {
                // print all values from the circuit that are available. 
                for (int i = 1; i < model.NodeCount; i++) // no need to print ground voltage
                {
                    output.WriteLine($"V({nodeNames[i]}) = {model.NodeVoltages[i]}");
                }

                foreach (var element in model.Elements.OfType<ITwoTerminalLargeSignalDeviceModel>().Where(e => !string.IsNullOrEmpty(e.Name)))
                {
                    output.WriteLine();
                    output.WriteLine($"V({element.Name}) = {element.Voltage}");
                    output.WriteLine($"I({element.Name}) = {element.Current}");
                }
            }
            else // only requested values
            {
                foreach (var statement in prints)
                {
                    statement.Initialize(model);
                    output.Write($"{statement.Header} = ");
                    statement.PrintValue(output);
                    output.WriteLine();
                }
            }
            
        }
    }
}