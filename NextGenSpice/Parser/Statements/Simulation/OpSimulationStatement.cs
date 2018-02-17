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
    ///     Class representing a call to .OP statement.
    /// </summary>
    public class OpSimulationStatement : ISimulationStatement
    {
        private readonly IDictionary<int, string> nodeNames;
        private readonly OpSimulationParams param;

        public OpSimulationStatement(OpSimulationParams param, IDictionary<int, string> nodeNames)
        {
            this.param = param;
            this.nodeNames = nodeNames;
        }


        /// <summary>
        ///     Performs the simulation and prints results to specified TextWriter.
        /// </summary>
        /// <param name="circuit">Circuit on which analysis should be performed.</param>
        /// <param name="printStatements">Set of all requested print statements that were requested in SPICE input file.</param>
        /// <param name="output">TextWriter instance to which the results should be written.</param>
        public void Simulate(ICircuitDefinition circuit, IEnumerable<PrintStatement> printStatements, TextWriter output)
        {
            output.WriteLine($".OP");
            var model = circuit.GetLargeSignalModel();
            var prints = printStatements.OfType<PrintStatement<LargeSignalCircuitModel>>()
                .Where(s => s.AnalysisType == "OP").ToList();
            model.EstablishDcBias();

            if (prints.Count == 0)
            {
                // print all values from the circuit that are available. 
                for (var i = 1; i < model.NodeCount; i++) // no need to print ground voltage
                    output.WriteLine($"V({nodeNames[i]}) = {model.NodeVoltages[i]}");

               
                foreach (var element in model.Elements)
                {
                    var providers = element.GetDeviceStatsProviders();
                    if (providers.Any()) output.WriteLine(); // separate from previous data
                    foreach (var provider in providers)
                    {
                        output.WriteLine($"{provider.StatName}({element.Name}) = {provider.GetValue()}"); 
                    }
                }
            }

            else // only requested values
            {
                var errors = prints.SelectMany(pr => pr.Initialize(model)).ToList();
                if (errors.Count > 0) throw new PrinterInitializationException(errors);
                foreach (var statement in prints)
                {
                    output.Write($"{statement.Header} = ");
                    statement.PrintValue(output);
                    output.WriteLine();
                }
            }
        }
    }
}