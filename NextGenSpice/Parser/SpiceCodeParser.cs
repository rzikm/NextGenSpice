using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Representation;
using NextGenSpice.Parser.Statements;
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
            statementProcessors = new Dictionary<string, IStatementProcessor>
            {
                [modelProcessor.Discriminator] = modelProcessor,
                [printProcessor.Discriminator] = printProcessor
            };
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
            // TODO: .END statement
            // TODO: Subcircuits

            Token[] tokens;
            var ctx = new ParsingContext();

            // parse input file by logical lines, each line is an independent statement
            while ((tokens = stream.ReadLogicalLine().ToArray()).Length > 0) // while not EOF
            {
                var firstToken = tokens[0]; // statement discriminator
                var c = firstToken.Value[0];

                if (char.IsLetter(c)) // possible element statement
                    ProcessElement(c, tokens, ctx);
                else if (c != '.') // syntactic error
                    ctx.Errors.Add(firstToken.ToErrorInfo($"Unexpected character: '{c}'."));
                else // .[keyword] statement
                    ProcessStatement(tokens, ctx);
            }

            return ApplyStatements(ctx);
        }

        private ParserResult ApplyStatements(ParsingContext ctx)
        {
            ProcessDeferredStatements(ctx);

            // get error messages from not processed statements
            foreach (var statement in ctx.DeferredStatements)
                ctx.Errors.AddRange(statement.GetErrors());

            // create circuit only if there were no errors
            var circuitDefinition = ctx.Errors.Count == 0 ? TryCreateCircuitDefinition(ctx) : null;

            return new ParserResult(circuitDefinition, ctx.PrintStatements, ctx.SimulationStatements,
                ctx.Errors.OrderBy(e => e.LineNumber).ThenBy(e => e.LineColumn).ToList());
        }

        private static void ProcessDeferredStatements(ParsingContext ctx)
        {
            //TODO: Use some ordering of statements and sort them beforehand (=> linear complexity)

            // repeatedly try to process all statements until no more statements can be processed in the iteration

            var deferred = ctx.DeferredStatements.ToList();
            do
            {
                ctx.DeferredStatements.Clear();
                ctx.DeferredStatements.AddRange(deferred);
                deferred.Clear();

                foreach (var statement in ctx.DeferredStatements)
                    if (statement.CanApply(ctx))
                        statement.Apply(ctx);
                    else
                        deferred.Add(statement);
            } while (deferred.Count < ctx.DeferredStatements.Count);
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
                            $"Some nodes are not connected to the ground node ({string.Join(", ", ctx.SymbolTable.GetNodeNames(ex.Nodes))})";
                        break;

                    case NotConnectedSubcircuit ex:
                        message =
                            $"No path connecting node sets {string.Join(", ", ex.Components.Select(c => $"({string.Join(", ", ctx.SymbolTable.GetNodeNames(c))})"))}.";
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
            if (statementProcessors.TryGetValue(discriminator, out var proc))
                proc.Process(tokens, ctx);
            else // unknown statement
                ctx.Errors.Add(tokens[0].ToErrorInfo($"Statement invalid or not implemented: {discriminator}."));
        }

        private void ProcessElement(char discriminator, Token[] tokens, ParsingContext ctx)
        {
            // find processor that can handle this element
            if (elementProcessors.TryGetValue(discriminator, out var proc))
                proc.Process(tokens, ctx);
            else // unknown element
                ctx.Errors.Add(tokens[0].ToErrorInfo($"Element type invalid or not implemented: {discriminator}."));
        }
    }
}