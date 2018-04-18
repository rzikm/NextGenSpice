using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NextGenSpice.Core.Exceptions;
using NextGenSpice.Core.Representation;
using NextGenSpice.Parser.Statements;
using NextGenSpice.Parser.Statements.Devices;
using NextGenSpice.Parser.Statements.Simulation;
using NextGenSpice.Utils;

namespace NextGenSpice.Parser
{
    /// <summary>Main class for parsing SPICE code input files</summary>
    public class SpiceCodeParser
    {
        private readonly IDictionary<char, IDeviceStatementProcessor> deviceProcessors;

        private readonly IDictionary<string, IDotStatementProcessor> insubcircuitStatementProcessors;

        private readonly ModelStatementProcessor modelProcessor;
        private readonly PrintStatementProcessor printProcessor;
        private readonly IDictionary<string, IDotStatementProcessor> statementProcessors;

        public SpiceCodeParser()
        {
            modelProcessor = new ModelStatementProcessor();
            printProcessor = new PrintStatementProcessor();

            deviceProcessors = new Dictionary<char, IDeviceStatementProcessor>();
            statementProcessors = new Dictionary<string, IDotStatementProcessor>();
            insubcircuitStatementProcessors = new Dictionary<string, IDotStatementProcessor>();

            RegisterStatement(modelProcessor, true, true);
            RegisterStatement(printProcessor, true, false);

            RegisterDefaults();
        }

        private void RegisterDefaults()
        {
            RegisterDevice(new ResistorStatementProcessor());
            RegisterDevice(new CurrentSourceStatementProcessor());
            RegisterDevice(new VoltageSourceStatementProcessor());

            RegisterDevice(new CapacitorStatementProcessor());
            RegisterDevice(new InductorStatementProcessor());

            RegisterDevice(new DiodeStatementProcessor());
            RegisterDevice(new BjtStatementProcessor());

            RegisterDevice(new SubcircuitDeviceStatementProcessor());

            RegisterDevice(new VoltageControlledVoltageSourceStatementProcessor());
            RegisterDevice(new VoltageControlledCurrentSourceStatementProcessor());

            var root = new SubcircuitStatementRoot();
            RegisterStatement(new SubcircuitStatementProcessor(root), true, true);
            RegisterStatement(new SubcircuitEndStatementProcessor(root), false, true);
            RegisterStatement(new InitialConditionStatement(), true, false);

            RegisterSimulation(new TranStatementProcessor());
            RegisterSimulation(new OpStatementProcessor());
        }

        /// <summary>Adds handler class for device statement processing, including their .MODEL statements</summary>
        /// <param name="processor"></param>
        public void RegisterDevice(IDeviceStatementProcessor processor)
        {
            deviceProcessors.Add(processor.Discriminator, processor);
            foreach (var handler in processor.GetModelStatementHandlers())
                modelProcessor.AddHandler(handler);
        }

        /// <summary>Adds handler for .[simulation type] statement processing, including accompanying .PRINT statements.</summary>
        /// <param name="processor"></param>
        public void RegisterSimulation(ISimulationStatementProcessor processor)
        {
            statementProcessors.Add(processor.Discriminator, processor);
            printProcessor.AddHandler(processor.GetPrintStatementHandler());
        }

        private void RegisterStatement(IDotStatementProcessor processor, bool global, bool subcircuit)
        {
            if (global)
                statementProcessors.Add(processor.Discriminator, processor);
            if (subcircuit)
                insubcircuitStatementProcessors.Add(processor.Discriminator, processor);
        }

        /// <summary>Parses SPICE code in the input stream.</summary>
        /// <param name="filestream">Netlist input filestream</param>
        /// <returns></returns>
        public ParserResult Parse(FileStream filestream)
        {
            return Parse(new TokenStream(new StreamReader(filestream)));
        }

        /// <summary>Parses SPICE code in the input stream.</summary>
        /// <param name="input">Netlist input source code.</param>
        /// <returns></returns>
        public ParserResult Parse(TextReader input)
        {
            return Parse(new TokenStream(input));
        }

        /// <summary>Parses SPICE code in the input stream.</summary>
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

                if (char.IsLetter(c)) // possible device statement
                    ProcessDevice(tokens, ctx, deviceProcessors);
                else if (c != '.') // syntactic error
                    ctx.Errors.Add(firstToken.ToErrorInfo(SpiceParserError.UnexpectedCharacter, c));
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

        private static CircuitDefinition TryCreateCircuitDefinition(ParsingContext ctx)
        {
            CircuitDefinition circuitDefinition = null;
            try
            {
                circuitDefinition = ctx.CircuitBuilder.BuildCircuit();
            }
            catch (Exception e)
            {
                string message;

                // translate node indexes to node names used in the input file
                ErrorInfo error;
                switch (e)
                {
                    case NoDcPathToGroundException ex:
                        error = new ErrorInfo(SpiceParserError.NoDcPathToGround, 0, 0,
                            ctx.SymbolTable.GetNodeNames(ex.Nodes).Cast<object>().ToArray());
                        break;

                    case NotConnectedSubcircuitException ex:
                        var names = ex.Components.Select(c => ctx.SymbolTable.GetNodeNames(c).ToArray()).Cast<object>()
                            .ToArray();
                        error = new ErrorInfo(SpiceParserError.SubcircuitNotConnected, 0, 0, names);
                        break;

                    case VoltageBranchCycleException ex:
                        error = new ErrorInfo(SpiceParserError.VoltageBranchCycle, 0, 0,
                            ex.Devices.Select(el => el.Name).Cast<object>().ToArray());
                        break;

                    case CurrentBranchCutsetException ex:
                        error = new ErrorInfo(SpiceParserError.CurrentBranchCutset, 0, 0,
                            ex.Devices.Select(el => el.Name).Cast<object>().ToArray());
                        break;

                    default:
                        throw;
                }

                ctx.Errors.Add(error);
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
                ctx.Errors.Add(tokens[0].ToErrorInfo(SpiceParserError.UnknownStatement));
        }

        private void ProcessDevice(Token[] tokens, ParsingContext ctx,
            IDictionary<char, IDeviceStatementProcessor> deviceStatementProcessors)
        {
            var discriminator = tokens[0].Value[0];
            // find processor that can handle this device
            if (deviceStatementProcessors.TryGetValue(discriminator, out var proc))
                proc.Process(tokens, ctx);
            else // unknown device
                ctx.Errors.Add(tokens[0].ToErrorInfo(SpiceParserError.UnknownDevice));
        }
    }
}