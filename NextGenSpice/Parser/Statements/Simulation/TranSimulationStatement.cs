using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;
using NextGenSpice.Parser.Statements.Deferring;
using NextGenSpice.Parser.Statements.Printing;
using NextGenSpice.Utils;

namespace NextGenSpice.Parser.Statements.Simulation
{
    /// <summary>
    ///     Class responsible for handling .TRAN simulation statements.
    /// </summary>
    public class TranSimulationStatement : ISimulationStatement
    {
        private readonly Dictionary<int, string> nodeNames;
        private readonly TranSimulationParams param;

        public TranSimulationStatement(TranSimulationParams param, Dictionary<int, string> nodeNames)
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

            foreach (var element in model.Elements)
            {
                printers.AddRange(element.GetPrintValueProviders().Select(pr => new ElementPrintStatement(pr.StatName, element.Name, new Token())));
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
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public PrinterInitializationException(IEnumerable<ErrorInfo> errors) : base("There were errors during printer initializations.")
        {
            Errors = errors;
        }

        public IEnumerable<ErrorInfo> Errors { get; }

        protected PrinterInitializationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}