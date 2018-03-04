﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Exceptions;
using NextGenSpice.Core.Representation;
using NextGenSpice.Parser.Statements;
using NextGenSpice.Parser.Statements.Deferring;
using NextGenSpice.Parser.Statements.Devices;
using NextGenSpice.Parser.Statements.Simulation;
using NextGenSpice.Utils;

namespace NextGenSpice.Parser
{
    /// <summary>
    ///     Main class for parsing SPICE code input files
    /// </summary>
    public class SpiceCodeParser
    {
        private readonly IDictionary<char, IElementStatementProcessor> elementProcessors;

        private readonly ModelStatementProcessor modelProcessor;
        private readonly PrintStatementProcessor printProcessor;
        private readonly IDictionary<string, IStatementProcessor> statementProcessors;

        public SpiceCodeParser()
        {
            modelProcessor = new ModelStatementProcessor();
            printProcessor = new PrintStatementProcessor();

            elementProcessors = new Dictionary<char, IElementStatementProcessor>();
            statementProcessors = new Dictionary<string, IStatementProcessor>();
            insubcircuitStatementProcessors = new Dictionary<string, IStatementProcessor>();

            RegisterStatement(modelProcessor, true, true);
            RegisterStatement(printProcessor, true, false);

            RegisterDefaults();
        }

        private readonly IDictionary<string, IStatementProcessor> insubcircuitStatementProcessors;

        private void RegisterDefaults()
        {
            RegisterElement(new ResistorStatementProcessor());
            RegisterElement(new CurrentSourceStatementProcessor());
            RegisterElement(new VoltageSourceStatementProcessor());

            RegisterElement(new CapacitorStatementProcessor());
            RegisterElement(new InductorStatementProcessor());

            RegisterElement(new DiodeStatementProcessor());
            RegisterElement(new BjtStatementProcessor());

            RegisterElement(new SubcircuitElementStatementProcessor());

            RegisterElement(new VoltageControlledVoltageSourceStatementProcessor());

            var root = new SubcircuitStatementRoot();
            RegisterStatement(new SubcircuitStatementProcessor(root), true, true);
            RegisterStatement(new SubcircuitEndStatementProcessor(root), false, true);
            
            RegisterSimulation(new TranStatementProcessor());
            RegisterSimulation(new OpStatementProcessor());
        }

        /// <summary>
        ///     Adds handler class for element statement processing, including their .MODEL statements
        /// </summary>
        /// <param name="processor"></param>
        public void RegisterElement(IElementStatementProcessor processor)
        {
            elementProcessors.Add(processor.Discriminator, processor);
            foreach (var handler in processor.GetModelStatementHandlers())
                modelProcessor.AddHandler(handler);
        }

        /// <summary>
        ///     Adds handler for .[simulation type] statement processing, including accompanying .PRINT statements.
        /// </summary>
        /// <param name="processor"></param>
        public void RegisterSimulation(ISimulationStatementProcessor processor)
        {
            statementProcessors.Add(processor.Discriminator, processor);
            printProcessor.AddHandler(processor.GetPrintStatementHandler());
        }

        private void RegisterStatement(IStatementProcessor processor, bool global, bool subcircuit)
        {
            if (global)
                statementProcessors.Add(processor.Discriminator, processor);
            if (subcircuit)
                insubcircuitStatementProcessors.Add(processor.Discriminator, processor);
        }



        /// <summary>
        ///     Parses SPICE code in the input stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public ParserResult Parse(FileStream filestream)
        {
            return Parse(new TokenStream(new StreamReader(filestream)));
        }

        /// <summary>
        ///     Parses SPICE code in the input stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public ParserResult Parse(ITokenStream stream)
        {
            Token[] tokens;
            var ctx = new ParsingContext();
            modelProcessor.RegisterDefaultModels(ctx);
            ctx.SymbolTable.FreezeDefaults();

            // parse input file by logical lines, each line is an independent statement
            while ((tokens = stream.ReadLogicalLine().ToArray()).Length > 0) // while not EOF
            {
                var firstToken = tokens[0]; // statement discriminator
                var c = firstToken.Value[0];

                if (char.IsLetter(c)) // possible element statement
                    ProcessElement(tokens, ctx, elementProcessors);
                else if (c != '.') // syntactic error
                    ctx.Errors.Add(firstToken.ToErrorInfo($"Unexpected character: '{c}'."));
                else if (tokens[0].Value == ".END" && tokens.Length == 1)
                    break; // end parsing now
                else // other .[keyword] statement
                     // if currently inside a subcircuit definition, use different statement processors
                    ProcessStatement(tokens, ctx);
            }

            return ApplyStatements(ctx);
        }

        private ParserResult ApplyStatements(ParsingContext ctx)
        {
            ctx.FlushStatements();

            // create circuit only if there were no errors
            var circuitDefinition = ctx.Errors.Count == 0 ? TryCreateCircuitDefinition(ctx) : null;

            return new ParserResult(circuitDefinition, ctx.PrintStatements, ctx.SimulationStatements,
                ctx.Errors.OrderBy(e => e.LineNumber).ThenBy(e => e.LineColumn).ToList());
        }
        
        private static ElectricCircuitDefinition TryCreateCircuitDefinition(ParsingContext ctx)
        {
            ElectricCircuitDefinition circuitDefinition = null;
            try
            {
                circuitDefinition = ctx.CircuitBuilder.BuildCircuit();
            }
            catch (Exception e)
            {
                string message;

                // translate node indexes to node names used in the input file
                switch (e)
                {
                    case NoDcPathToGroundException ex:
                        message =
                            $"Some nodes are not connected to the ground node ({string.Join(", ", ctx.SymbolTable.GetNodeNames(ex.Nodes))}).";
                        break;

                    case NotConnectedSubcircuitException ex:
                        message =
                            $"No path connecting node sets {string.Join(", ", ex.Components.Select(c => $"({string.Join(", ", ctx.SymbolTable.GetNodeNames(c))})"))}.";
                        break;

                    case VoltageBranchCycleException ex:
                        message =
                            $"Circuit contains a cycle of voltage defined elements ({string.Join(", ", ex.Elements.Select(el => el.Name))}).";
                        break;

                    case CurrentBranchCutsetException ex:
                        message =
                            $"Circuit contains a cutset of current defined elements ({string.Join(", ", ex.Elements.Select(el => el.Name))}).";
                        break;

                    default:
                        throw;
                }

                ctx.Errors.Add(new ErrorInfo
                {
                    LineColumn = 0, // no coordinates shall be displayed when printing this error
                    LineNumber = 0,
                    Messsage = message
                });
            }
            return circuitDefinition;
        }

        private void ProcessStatement(Token[] tokens, ParsingContext ctx)
        {
            // find processor that can handle this statement
            var discriminator = tokens[0].Value;
            var processors = ctx.SubcircuitDepth == 0 ? statementProcessors : insubcircuitStatementProcessors;
            if (processors.TryGetValue(discriminator, out var proc))
                proc.Process(tokens, ctx);
            else // unknown statement
                ctx.Errors.Add(tokens[0].ToErrorInfo($"Statement invalid: '{string.Join(" ", tokens.Select(t => t.Value))}'."));
        }

        private void ProcessElement(Token[] tokens, ParsingContext ctx, IDictionary<char, IElementStatementProcessor> elementStatementProcessors)
        {
            var discriminator = tokens[0].Value[0];
            // find processor that can handle this element
            if (elementStatementProcessors.TryGetValue(discriminator, out var proc))
                proc.Process(tokens, ctx);
            else // unknown element
                ctx.Errors.Add(tokens[0].ToErrorInfo($"Element type invalid: '{string.Join(" ", tokens.Select(t => t.Value))}'."));
        }
    }
}