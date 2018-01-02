using System.Collections.Generic;
using System.IO;
using System.Linq;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpice
{
    public class OpSimulationStatement : SimulationStatement
    {
        private readonly OpSimulationParams param;
        private readonly Dictionary<int, string> nodeNames;

        public OpSimulationStatement(OpSimulationParams param, Dictionary<int, string> nodeNames)
        {
            this.param = param;
            this.nodeNames = nodeNames;
        }

        public override void Simulate(ICircuitDefinition circuit, IEnumerable<PrintStatement> printStatements, TextWriter output)
        {
            var model = circuit.GetLargeSignalModel();

            model.EstablishDcBias();

            for (int i = 0; i < model.NodeCount; i++)
            {
                output.WriteLine($"V({nodeNames[i]}) = {model.NodeVoltages[i]}");
            }

            foreach (var element in model.Elements.OfType<ITwoTerminalLargeSignalDeviceModel>().Where(e => !string.IsNullOrEmpty(e.Name)))
            {
                output.WriteLine($"V({element.Name}) = {element.Voltage}");
                output.WriteLine($"I({element.Name}) = {element.Current}");
            }
        }
    }
}