using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Representation;

namespace NextGenSpice
{
    public class SpiceCodeParser
    {
        private readonly IDictionary<char, ElementStatementProcessor> elementProcessors;
        private readonly IDictionary<string, StatementProcessor> statementProcessors;

        public SpiceCodeParser()
        {
            elementProcessors = new Dictionary<char, ElementStatementProcessor>();
            statementProcessors = new Dictionary<string, StatementProcessor>();

            Register(new TranStatementProcessor());
            Register(new OpStatementProcessor());
            Register(new PrintStatementProcessor());
        }

        public void Register(ElementStatementProcessor processor)
        {
            elementProcessors.Add(processor.Discriminator, processor);
        }
        public void Register(StatementProcessor processor)
        {
            statementProcessors.Add(processor.Discriminator, processor);
        }

        public ParserResult ParseInputFile(TokenStream stream)
        {
            Token[] tokens;

            var ctx = new ParsingContext();

            while ((tokens = stream.ReadLogicalLine().ToArray()).Length > 0) // while not EOF
            {
                var firstToken = tokens[0];
                var discriminator = firstToken.Value[0];

                if (char.IsLetter(discriminator)) // element statement
                {
                    ProcessElement(discriminator, tokens, ctx);
                }
                else if (discriminator != '.')
                {
                    ctx.Errors.Add(firstToken.ToErrorInfo($"Unexpected character: '{discriminator}'."));
                }
                else
                {
                    ProcessStatement(tokens, ctx);
                }
            }

            return PostProcess(ctx);
        }

        private ParserResult PostProcess(ParsingContext ctx)
        {
            ProcessDeferredStatements(ctx);

            foreach (var statement in ctx.DeferredStatements)
            {
                ctx.Errors.AddRange(statement.GetErrors());
            }

            var circuitDefinition = ctx.Errors.Count == 0 ? CreateCircuitDefinition(ctx) : null;

            return new ParserResult(circuitDefinition, ctx.PrintStatements, ctx.SimulationStatements, ctx.Errors.OrderBy(e => e.LineNumber).ThenBy(e => e.LineColumn).ToList());
        }

        private static void ProcessDeferredStatements(ParsingContext ctx)
        {
            var deferred = ctx.DeferredStatements.ToList();
            do
            {
                ctx.DeferredStatements.Clear();
                ctx.DeferredStatements.AddRange(deferred);
                deferred.Clear();

                foreach (var statement in ctx.DeferredStatements)
                {
                    if (statement.CanApply(ctx))
                    {
                        statement.Apply(ctx);
                    }
                    else
                    {
                        deferred.Add(statement);
                    }
                }
            } while (deferred.Count < ctx.DeferredStatements.Count);
        }

        private static ElectricCircuitDefinition CreateCircuitDefinition(ParsingContext ctx)
        {
            ElectricCircuitDefinition circuitDefinition = null;
            try
            {
                circuitDefinition = ctx.CircuitBuilder.BuildCircuit();
            }
            catch (Exception e)
            {
                string message;

                switch (e)
                {
                    case NoDcPathToGroundException ex:
                        message =
                            $"Some nodes are not connected to the ground node ({string.Join(", ", ctx.SymbolTable.GetNodeNames(ex.Nodes))})";
                        break;

                    case NotConnectedSubcircuit ex:
                        message =
                            $"No path connecting node sets {string.Join(", ", ex.Components.Select(c => $"({String.Join(", ", ctx.SymbolTable.GetNodeNames(c))})"))}.";
                        break;

                    default:
                        throw;
                }

                ctx.Errors.Add(new ErrorInfo
                {
                    LineColumn = 0,
                    LineNumber = 0,
                    Messsage = message
                });
            }
            return circuitDefinition;
        }

        private void ProcessStatement(Token[] tokens, ParsingContext ctx)
        {
            var discriminator = tokens[0].Value;
            if (statementProcessors.TryGetValue(discriminator, out var proc))
            {
                proc.Process(tokens, ctx);
            }
            else
            {
                ctx.Errors.Add(tokens[0].ToErrorInfo($"Statement invalid or not implemented: {discriminator}."));
            }
        }

        private void ProcessElement(char discriminator, Token[] tokens, ParsingContext ctx)
        {
            if (elementProcessors.TryGetValue(discriminator, out var proc))
            {
                proc.Process(tokens, ctx);
            }
            else
            {
                ctx.Errors.Add(tokens[0].ToErrorInfo($"Element type invalid or not implemented: {discriminator}."));
            }

        }
    }
}