using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Exceptions;
using NextGenSpice.Core.Representation;
using NextGenSpice.Parser.Statements;
using NextGenSpice.Parser.Statements.Devices;
using NextGenSpice.Parser.Statements.Printing;
using NextGenSpice.Parser.Utils;

namespace NextGenSpice.Parser
{
    /// <summary>Main class for parsing SPICE netlist files</summary>
    public class SpiceNetlistParser
    {
        private readonly IDictionary<char, IDeviceStatementProcessor> deviceProcessors;

        private readonly IDictionary<string, IDotStatementProcessor> insubcircuitStatementProcessors;

        private readonly ModelStatementProcessor modelProcessor;
        private readonly PrintStatementProcessor printProcessor;
        private readonly IDictionary<string, IDotStatementProcessor> statementProcessors;

        public SpiceNetlistParser()
        {
            modelProcessor = new ModelStatementProcessor();
            printProcessor = new PrintStatementProcessor();

            deviceProcessors = new Dictionary<char, IDeviceStatementProcessor>();
            statementProcessors = new Dictionary<string, IDotStatementProcessor>();
            insubcircuitStatementProcessors = new Dictionary<string, IDotStatementProcessor>();

            RegisterStatement(modelProcessor, true, true);
            RegisterStatement(printProcessor, true, false);
        }

        public static SpiceNetlistParser WithDefaults()
        {
            var p = new SpiceNetlistParser();
            p.RegisterDevice(new ResistorStatementProcessor());
            p.RegisterDevice(new CurrentSourceStatementProcessor());
            p.RegisterDevice(new VoltageSourceStatementProcessor());

            p.RegisterDevice(new CapacitorStatementProcessor());
            p.RegisterDevice(new InductorStatementProcessor());

            p.RegisterDevice(new DiodeStatementProcessor());
            p.RegisterDevice(new BjtStatementProcessor());

            p.RegisterDevice(new SubcircuitDeviceStatementProcessor());

            p.RegisterDevice(new VoltageControlledVoltageSourceStatementProcessor());
            p.RegisterDevice(new VoltageControlledCurrentSourceStatementProcessor());

            var root = new SubcircuitStatementRoot();
            p.RegisterStatement(new SubcircuitStatementProcessor(root), true, true);
            p.RegisterStatement(new SubcircuitEndStatementProcessor(root), false, true);
            p.RegisterStatement(new InitialConditionStatement(), true, false);

            return p;
        }


        /// <summary>Adds handler class for device statement processing, including their .MODEL statements</summary>
        /// <param name="processor"></param>
        public void RegisterDevice(IDeviceStatementProcessor processor)
        {
            deviceProcessors[processor.Discriminator] = processor;
            foreach (var handler in processor.GetModelStatementHandlers())
                modelProcessor.AddHandler(handler);
        }

        public void RegisterStatement(IDotStatementProcessor processor, bool global, bool subcircuit)
        {
            if (global)
                statementProcessors[processor.Discriminator] = processor;
            if (subcircuit)
                insubcircuitStatementProcessors[processor.Discriminator] = processor;
        }

        public void RegisterPrint(IPrintStatementHandler handler)
        {
            printProcessor.AddHandler(handler);
        }

        /// <summary>Parses SPICE code in the input stream.</summary>
        /// <param name="filestream">Netlist input filestream</param>
        /// <returns></returns>
        public SpiceNetlistParserResult Parse(FileStream filestream)
        {
            return Parse(new StreamReader(filestream));
        }

        /// <summary>Parses SPICE code in the input stream.</summary>
        /// <param name="input">Netlist input source code.</param>
        /// <returns></returns>
        public SpiceNetlistParserResult Parse(TextReader input)
        {
            var ctx = new ParsingContext()
            {
                Title = input.ReadLine() // First line of the file always contains title
            };

            ITokenStream stream = new TokenStream(input, 1);
            Token[] tokens;
            modelProcessor.RegisterDefaultModels(ctx);
            ctx.SymbolTable.FreezeDefaults();

            // parse input file by logical lines, each line is an independent statement
            while ((tokens = stream.ReadStatement().ToArray()).Length > 0) // while not EOF
            {
                var firstToken = tokens[0]; // statement discriminator
                var c = firstToken.Value[0];

                if (char.IsLetter(c)) // possible device statement
                    ProcessDevice(tokens, ctx, deviceProcessors);
                else if (c != '.') // syntactic error
                    ctx.Errors.Add(firstToken.ToError(SpiceParserErrorCode.UnexpectedCharacter, c));
                else if (tokens[0].Value == ".END" && tokens.Length == 1)
                    break; // end parsing now
                else // other .[keyword] statement
                    // if currently inside a subcircuit definition, use different statement processors
                    ProcessStatement(tokens, ctx);
            }

            return ApplyStatements(ctx);
        }

        private SpiceNetlistParserResult ApplyStatements(ParsingContext ctx)
        {
            ctx.FlushStatements();

            // create circuit only if there were no errors
            var circuitDefinition = ctx.Errors.Count == 0 ? TryCreateCircuitDefinition(ctx) : null;

            return new SpiceNetlistParserResult(
                ctx.Title,
                circuitDefinition,
                ctx.OtherStatements,
                ctx.Errors.OrderBy(e => e.LineNumber).ThenBy(e => e.LineColumn).ToList(), // order errors
                ctx.SymbolTable.GetSubcircuits().OfType<SubcircuitDefinition>().ToList(), // only valid subcircuit definitions, no NullCircuitDefinitions
                ctx.SymbolTable.GetNodeNames(Enumerable.Range(0, ctx.CircuitBuilder.NodeCount)).ToList(),
                ctx.SymbolTable.GetAllModels());
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
                Utils.SpiceParserError error;
                switch (e)
                {
                    case NoDcPathToGroundException ex:
                        error = new Utils.SpiceParserError(SpiceParserErrorCode.NoDcPathToGround, 0, 0,
                            ctx.SymbolTable.GetNodeNames(ex.Nodes).Cast<object>().ToArray());
                        break;

                    case NotConnectedSubcircuitException ex:
                        var names = ex.Components.Select(c => ctx.SymbolTable.GetNodeNames(c).ToArray()).Cast<object>()
                            .ToArray();
                        error = new Utils.SpiceParserError(SpiceParserErrorCode.SubcircuitNotConnected, 0, 0, names);
                        break;

                    case VoltageBranchCycleException ex:
                        error = new Utils.SpiceParserError(SpiceParserErrorCode.VoltageBranchCycle, 0, 0,
                            ex.Devices.Select(el => el.Tag).ToArray());
                        break;

                    case CurrentBranchCutsetException ex:
                        error = new Utils.SpiceParserError(SpiceParserErrorCode.CurrentBranchCutset, 0, 0,
                            ex.Devices.Select(el => el.Tag).ToArray());
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
                ctx.Errors.Add(tokens[0].ToError(SpiceParserErrorCode.UnknownStatement));
        }

        private void ProcessDevice(Token[] tokens, ParsingContext ctx,
            IDictionary<char, IDeviceStatementProcessor> deviceStatementProcessors)
        {
            var discriminator = tokens[0].Value[0];
            // find processor that can handle this device
            if (deviceStatementProcessors.TryGetValue(discriminator, out var proc))
                proc.Process(tokens, ctx);
            else // unknown device
                ctx.Errors.Add(tokens[0].ToError(SpiceParserErrorCode.UnknownDevice));
        }
    }
}