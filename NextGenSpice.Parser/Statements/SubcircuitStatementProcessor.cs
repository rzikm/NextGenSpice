using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Exceptions;
using NextGenSpice.Parser.Utils;

namespace NextGenSpice.Parser.Statements
{
    /// <summary>Class for handling SPICE .SUBCKT statements.</summary>
    public class SubcircuitStatementProcessor : DotStatementProcessor
    {
        private readonly SubcircuitStatementRoot root;

        public SubcircuitStatementProcessor(SubcircuitStatementRoot root)
        {
            this.root = root;
            MinArgs = 2;
        }

        /// <summary>Statement discriminator, that this class can handle.</summary>
        public override string Discriminator => ".SUBCKT";

        /// <summary>Processes given statement.</summary>
        /// <param name="tokens">All tokens of the statement.</param>
        protected override void DoProcess(Token[] tokens)
        {
            Context.EnterSubcircuit();
            var name = tokens[1].Value;
            if (Context.SymbolTable.TryGetSubcircuit(name, out _))
                Context.Errors.Add(tokens[1].ToError(SpiceParserErrorCode.SubcircuitAlreadyExists));
            root.Statements.Push(tokens); // to be processed in .ENDS statement handler
        }
    }

    /// <summary>Class for handling SPICE .ENDS statements.</summary>
    public class SubcircuitEndStatementProcessor : DotStatementProcessor
    {
        private readonly SubcircuitStatementRoot root;

        public SubcircuitEndStatementProcessor(SubcircuitStatementRoot root)
        {
            MinArgs = MaxArgs = 0;
            this.root = root;
        }


        /// <summary>Statement discriminator, that this class can handle.</summary>
        public override string Discriminator => ".ENDS";

        /// <summary>Processes given statement.</summary>
        /// <param name="tokens">All tokens of the statement.</param>
        protected override void DoProcess(Token[] tokens)
        {
            Debug.Assert(root.Statements.Count > 0);
            var stack = root.Statements.Pop();

            var name = stack[1].Value;
            var terminals = GetNodeIndices(stack.Skip(2));
            // enforce node existence, initial condition for nodes inside subcircuit is not supported and not used
            Context.CircuitBuilder.SetNodeVoltage(terminals.Max(), null);
            
            var subcircuit = CreateSubcircuit(stack[1], terminals);
            Context.ExitSubcircuit();

            Context.SymbolTable.AddSubcircuit(name, subcircuit);
        }

        /// <summary>Gets indices of the nodes represented by given set of tokens. Adds relevant errors into the errors collection.</summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        private int[] GetNodeIndices(IEnumerable<Token> tokens)
        {
            return tokens.Select(token =>
            {
                if (!Context.SymbolTable.TryGetOrCreateNode(token.Value, out var node))
                {
                    node = -1;
                    Context.Errors.Add(token.ToError(SpiceParserErrorCode.NotANode));
                }

                return node;
            }).ToArray();
        }

        private ISubcircuitDefinition CreateSubcircuit(Token name, int[] terminals)
        {
            ISubcircuitDefinition subcircuit = null;
            var errorCount = Context.Errors.Count;

            // validate terminal specs - no duplicates
            if (terminals.Distinct().Count() != terminals.Length)
                Context.Errors.Add(name.ToError(SpiceParserErrorCode.TerminalNamesNotUnique));

            // ground node not allowed as terminal
            if (terminals.Contains(0))
                Context.Errors.Add(name.ToError(SpiceParserErrorCode.TerminalToGround));

            Context.FlushStatements();
            if (errorCount == Context.Errors.Count) // no new errors, try to construct subcircuit
            {
                try
                {
                    subcircuit = Context.CircuitBuilder.BuildSubcircuit(terminals, name.Value);
                }
                catch (NotConnectedSubcircuitException e)
                {
                    // translate node indexes to node names used in the input file 
                    var names = e.Components.Select(c => Context.SymbolTable.GetNodeNames(c).ToArray()).Cast<object>()
                        .ToArray();
                    Context.Errors.Add(new Utils.SpiceParserError(SpiceParserErrorCode.SubcircuitNotConnected, name.LineNumber,
                        name.LineColumn, names));
                }
            }

            if (subcircuit == null) // create a "null object" to avoid explicit null checking
            {
                subcircuit = new NullSubcircuitDefinition(terminals);
            }

            return subcircuit;
        }

        class NullSubcircuitDefinition : ISubcircuitDefinition
        {
            public NullSubcircuitDefinition(int[] terminals)
            {
                TerminalNodes = terminals;
                InnerNodeCount = terminals.Length + 1;
                Devices = Enumerable.Empty<ICircuitDefinitionDevice>();
                Tag = null;
            }

            public object Tag { get; }

            public int[] TerminalNodes { get; }

            public int InnerNodeCount { get; }

            public IEnumerable<ICircuitDefinitionDevice> Devices { get; }
        }
    }

    public class SubcircuitStatementRoot
    {
        public SubcircuitStatementRoot()
        {
            Statements = new Stack<Token[]>();
        }

        public Stack<Token[]> Statements { get; }
    }
}