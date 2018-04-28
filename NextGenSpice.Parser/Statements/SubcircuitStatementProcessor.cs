using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Exceptions;
using NextGenSpice.Parser.Statements.Deferring;
using NextGenSpice.Parser.Utils;

namespace NextGenSpice.Parser.Statements
{
    /// <summary>Class for handling SPICE .SUBCKT statements.</summary>
    public class SubcircuitStatementProcessor : DotStatementProcessor
    {
        public SubcircuitStatementProcessor()
        {
            MinArgs = 2;
        }

        /// <summary>Statement discriminator, that this class can handle.</summary>
        public override string Discriminator => ".SUBCKT";

        /// <summary>Processes given statement.</summary>
        /// <param name="tokens">All tokens of the statement.</param>
        protected override void DoProcess(Token[] tokens)
        {
            var stmt = new DeferredSubcktStatement(Context.CurrentScope, tokens);
            Context.DeferredStatements.Add(stmt);

            Context.EnterSubcircuit(tokens);
            stmt.subScope = Context.CurrentScope; // add subscope information
            //            root.Statements.Push(tokens); // to be processed in .ENDS statement handler
        }

        public class DeferredSubcktStatement : DeferredStatement
        {
            private readonly List<SpiceParserError> errors;
            private readonly Token[] tokens;

            private ISubcircuitDefinition def;
            private string subname;
            public ParsingScope subScope;

            public DeferredSubcktStatement(ParsingScope context, Token[] tokens) : base(context)
            {
                this.tokens = tokens;
                errors = new List<SpiceParserError>();
            }

            /// <summary>Returns true if all prerequisites for the statements have been fulfilled and statement is ready to be applied.</summary>
            /// <param name="context"></param>
            /// <returns></returns>
            public override bool CanApply()
            {
                // wait until the subcircuit statements are processed
                return subScope.Statements.Count == 0;
            }

            /// <summary>Returns set of errors due to which this stetement cannot be processed.</summary>
            /// <returns></returns>
            public override IEnumerable<SpiceParserError> GetErrors()
            {
                throw new InvalidOperationException();
            }

            /// <summary>Applies the statement in the given context.</summary>
            /// <param name="context"></param>
            public override void Apply()
            {
                base.Apply();

                var name = tokens[1];
                subname = name.Value;

                var terminals = GetNodeIndices(tokens.Skip(2), subScope);

                if (context.SymbolTable.TryGetSubcircuit(name.Value, out _))
                    errors.Add(tokens[1].ToError(SpiceParserErrorCode.SubcircuitAlreadyExists));

                // validate terminal specs - no duplicates
                if (terminals.Distinct().Count() != terminals.Length)
                    errors.Add(name.ToError(SpiceParserErrorCode.TerminalNamesNotUnique));

                // ground node not allowed as terminal
                if (terminals.Contains(0))
                    errors.Add(name.ToError(SpiceParserErrorCode.TerminalToGround));

                try
                {
                    if (subScope.Errors.Count + errors.Count == 0)
                    {
                        subScope.CircuitBuilder.SetNodeVoltage(terminals.Max(), null);
                        def = subScope.CircuitBuilder.BuildSubcircuit(terminals, name.Value);
                    }
                }
                catch (NotConnectedSubcircuitException e)
                {
                    // translate node indexes to node names used in the input file 
                    var names = e.Components.Select(c => subScope.SymbolTable.GetNodeNames(c).ToArray()).Cast<object>()
                        .ToArray();
                    errors.Add(new SpiceParserError(SpiceParserErrorCode.SubcircuitNotConnected, name.LineNumber,
                        name.LineColumn, names));
                }

                if (def == null) def = new NullSubcircuitDefinition(terminals, subScope.CircuitBuilder.NodeCount);

                context.Errors.AddRange(errors);
                context.SymbolTable.AddSubcircuit(subname, def);
            }

            /// <summary>Gets indices of the nodes represented by given set of tokens. Adds relevant errors into the errors collection.</summary>
            /// <param name="tokens"></param>
            /// <returns></returns>
            private int[] GetNodeIndices(IEnumerable<Token> tokens, ParsingScope context)
            {
                return tokens.Select(token =>
                {
                    if (!context.SymbolTable.TryGetOrCreateNode(token.Value, out var node))
                    {
                        node = -1;
                        context.Errors.Add(token.ToError(SpiceParserErrorCode.NotANode));
                    }

                    return node;
                }).ToArray();
            }
        }

        private class NullSubcircuitDefinition : ISubcircuitDefinition
        {
            public NullSubcircuitDefinition(int[] terminals, int innerNodes)
            {
                TerminalNodes = terminals;
                InnerNodeCount = innerNodes;
                Devices = Enumerable.Empty<ICircuitDefinitionDevice>();
                Tag = null;
            }

            public object Tag { get; }

            public int[] TerminalNodes { get; }

            public int InnerNodeCount { get; }

            public IEnumerable<ICircuitDefinitionDevice> Devices { get; }
        }
    }

    /// <summary>Class for handling SPICE .ENDS statements.</summary>
    public class SubcircuitEndStatementProcessor : DotStatementProcessor
    {
        public SubcircuitEndStatementProcessor()
        {
            MinArgs = 0;
            MaxArgs = 1;
        }


        /// <summary>Statement discriminator, that this class can handle.</summary>
        public override string Discriminator => ".ENDS";

        /// <summary>Processes given statement.</summary>
        /// <param name="tokens">All tokens of the statement.</param>
        protected override void DoProcess(Token[] tokens)
        {
            if (tokens.Length == 2 && tokens[1].Value != Context.CurrentScope.SubcktStatement[1].Value)
                Context.Errors.Add(tokens[0].ToError(SpiceParserErrorCode.UnexpectedEnds, tokens[1].Value));

            Context.ExitSubcircuit();
        }
    }
}