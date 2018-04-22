using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal;
using NextGenSpice.Parser;
using NextGenSpice.Parser.Utils;
using NextGenSpice.Printing;

namespace NextGenSpice.Simulation
{
    /// <summary>Class responsible for handling .TRAN simulation statements.</summary>
    public class TranSimulationStatement : SpiceSimulationStatement, ISimulationStatement
    {
        private readonly IDictionary<int, string> nodeNames;
        private readonly TranStatementParam param;

        public TranSimulationStatement(TranStatementParam param, IDictionary<int, string> nodeNames)
        {
            this.param = param;
            this.nodeNames = nodeNames;
        }

        /// <summary>Performs the simulation and prints results to specified TextWriter.</summary>
        /// <param name="circuit">Circuit on which analysis should be performed.</param>
        /// <param name="printStatements">Set of all requested print statements that were requested in SPICE input file.</param>
        /// <param name="output">TextWriter instance to which the results should be written.</param>
        public void Simulate(ICircuitDefinition circuit, IEnumerable<PrintStatement> printStatements, TextWriter output)
        {
            var printers = printStatements.OfType<PrintStatement<LargeSignalCircuitModel>>()
                .Where(st => st.AnalysisType == "TRAN").ToList();
            var model = circuit.GetLargeSignalModel();

            output.WriteLine($".TRAN {param.TimeStep} {param.StopTime} {param.StartTime}");

            if (printers.Count == 0) // if no printer here, print all data
                GetPrintersForAll(model, printers);

            var errors = printers.SelectMany(pr => pr.Initialize(model)).ToList();

            if (errors.Count > 0) throw new PrinterInitializationException(errors);

            model.MaxTimeStep = param.TimeStep;
            model.AdvanceInTime(param.StartTime);

            var time = param.StartTime;
            PrintHeader(model, printers, output);
            PrintValues(model, printers, output);
            while (time < param.StopTime)
            {
                model.AdvanceInTime(param.TimeStep);
                time += param.TimeStep;
                PrintValues(model, printers, output);
            }
        }

        private void GetPrintersForAll(LargeSignalCircuitModel model,
            List<PrintStatement<LargeSignalCircuitModel>> printers)
        {
            // get printers for all nodes and two terminal devices

            for (var i = 1; i < model.NodeCount; i++) // no need to print ground voltage
                printers.Add(new NodeVoltagePrintStatement(nodeNames[i], i));

            foreach (var device in model.Devices)
            {
                printers.AddRange(device.GetDeviceStatsProviders()
                    .Select(pr => new DevicePrintStatement(pr.StatName, device.DefinitionDevice.Tag as string, new Token())));
            }
        }

        private void PrintHeader(LargeSignalCircuitModel model, List<PrintStatement<LargeSignalCircuitModel>> printers,
            TextWriter output)
        {
            output.Write("Time");
            foreach (var printer in printers)
            {
                output.Write(" ");
                output.Write(printer.Header);
            }

            output.WriteLine();
        }

        private void PrintValues(LargeSignalCircuitModel model, List<PrintStatement<LargeSignalCircuitModel>> printers,
            TextWriter output)
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

    [Serializable]
    public class PrinterInitializationException : InvalidOperationException
    {

        public PrinterInitializationException(IEnumerable<SpiceParserError> errors) : base(
            "There were errors during printer initializations.")
        {
            Errors = errors;
        }

        protected PrinterInitializationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public IEnumerable<SpiceParserError> Errors { get; }
    }
}