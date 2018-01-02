using System.Collections.Generic;
using System.IO;
using System.Linq;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal;

namespace NextGenSpice
{
    public class TranSimulationStatement : SimulationStatement
    {
        private readonly TranSimulationParams param;

        public TranSimulationStatement(TranSimulationParams param)
        {
            this.param = param;
        }

        public override void Simulate(ICircuitDefinition circuit, IEnumerable<PrintStatement> printStatements, TextWriter output)
        {
            var printers = printStatements.OfType<LsPrintStatement>().Where(st => st.AnalysisType == "TRAN").ToArray();

            var model = circuit.GetLargeSignalModel();

            model.MaxTimeStep = param.TimeStep;
            model.AdvanceInTime(param.StartTime);

            var time = param.StartTime;
            PrintHeader(model, printers, output);
            while (time < param.StopTime)
            {
                model.AdvanceInTime(param.TimeStep);
                time += param.TimeStep;
                PrintValues(model, printers, output);
            }
        }

        private void PrintHeader(LargeSignalCircuitModel model, LsPrintStatement[] printers, TextWriter output)
        {
            output.Write("Time");
            foreach (var printer in printers)
            {
                output.Write(" ");
                printer.Initialize(model);
                printer.PrintHeader(output);
            }
            output.WriteLine();
        }

        private void PrintValues(LargeSignalCircuitModel model, LsPrintStatement[] printers, TextWriter output)
        {
            output.Write(model.CurrentTimePoint);
            foreach (var printer in printers)
            {
                output.Write(" ");
                printer.PrintValue(output);
            }
            output.WriteLine();
        }
    }
}