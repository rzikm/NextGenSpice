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
            var builder = new CircuitBuilder();

//            var deferred = new List<DeferredStatement>();
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

            foreach (var statement in ctx.DeferredStatements)
            {
                ctx.Errors.AddRange(statement.GetErrors());
            }


            ElectricCircuitDefinition circuitDefinition = null;
            try
            {
                circuitDefinition = builder.BuildCircuit();
            }
            catch (Exception e)
            {

                throw;
            }


            return new ParserResult(circuitDefinition, ctx.PrintStatements, ctx.SimulationStatements, ctx.Errors.OrderBy(e => e.LineNumber).ThenBy(e => e.LineColumn).ToList());
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