using System;
using System.Collections.Generic;
using System.Linq;

namespace NextGenSpice
{
    public class SpiceCodeParser
    {
        private readonly IDictionary<char, ElementStatementProcessor> elementProcessors;

        public SpiceCodeParser()
        {
            elementProcessors = new Dictionary<char, ElementStatementProcessor>();
        }

        public void Register(ElementStatementProcessor processor)
        {
            elementProcessors.Add(processor.Discriminator, processor);
        }

        public ParserResult ParseInputFile(TokenStream stream)
        {
            var tab = new SymbolTable();
            Token[] tokens;

            var elemStatements = new List<ElementStatement>();
            var printStatements = new List<PrintStatement>();
            var simStatements = new List<SimulationStatement>();

            var errors = new List<ErrorInfo>();

            while ((tokens = stream.ReadLogicalLine().ToArray()).Length > 0) // while not EOF
            {
                var discriminator = tokens[0].Value[0];
                if (char.IsLetter(discriminator)) // element statement
                    elemStatements.Add(ProcessElement(discriminator, tokens, tab));
                if (discriminator != '.') errors.Add(new ErrorInfo { LineNumber = tokens[0].Line, LineColumn = tokens[0].Char, Messsage = $"Unexpected character: '{discriminator}'." });
                else
                {
                    switch (tokens[0].Value)
                    {
                        case ".TRAN":
                            ProcessTran(tokens, simStatements, errors);
                            break;

                        case ".PRINT":
                            ProcessPrint(tokens, printStatements, errors);
                            break;

                        default:
                            errors.Add(new ErrorInfo { LineNumber = tokens[0].Line, LineColumn = tokens[0].Char, Messsage = $"Unsupported statement: '{tokens[0].Value}'." });
                            break;
                    }
                }
            }
            return new ParserResult(elemStatements, printStatements, simStatements, errors);
        }

        private void ProcessTran(Token[] tokens, List<SimulationStatement> simStatements, List<ErrorInfo> errors)
        {
            ParameterMapper<SimulationStatement> mapper = new ParameterMapper<SimulationStatement>();
            mapper.Target = new SimulationStatement();
            mapper.Map(c => c.TimeStep, 1);
            mapper.Map(c => c.StopTime, 2);
            mapper.Map(c => c.StartTime, 3);
            mapper.Map(c => c.TMax, 4);

            if (tokens.Length < 3)
            {
                errors.Add(new ErrorInfo { LineNumber = tokens[0].Line, LineColumn = tokens[0].Char, Messsage = $"Too few arguments for {tokens[0].Value} statement." });
            }

            if (tokens.Length > 5)
            {
                errors.Add(new ErrorInfo { LineNumber = tokens[0].Line, LineColumn = tokens[0].Char, Messsage = $"Too many arguments for {tokens[0].Value} statement." });
            }

            for (int i = 1; i < tokens.Length; i++)
            {
                var t = tokens[i];
                var s = t.Value;

                mapper.Set(i, t.GetNumericValue(errors));
            }

            mapper.Target.AnalysisType = "TRAN";

            simStatements.Add(mapper.Target);
        }

        private void ProcessPrint(Token[] tokens, List<PrintStatement> printStatements, List<ErrorInfo> errors)
        {
            if (tokens.Length < 3)
            {
                errors.Add(new ErrorInfo { LineNumber = tokens[0].Line, LineColumn = tokens[0].Char, Messsage = $"Too few arguments for {tokens[0].Value} statement." });
                return;
            }

            var analysisType = tokens[1].Value;
            if (analysisType != "TRAN")
            {
                errors.Add(new ErrorInfo { LineNumber = tokens[1].Line, LineColumn = tokens[1].Char, Messsage = $"Unrecognized analysis type: '{analysisType}'." });
            }

            for (int i = 2; i < tokens.Length; i++)
            {
                var t = tokens[i];
                var s = t.Value;

                // expected token in format V(element), I(element), V(node)
                if (s.Length > 3 && (s[0] == 'I' || s[0] == 'V') && s[1] == '(' && s[s.Length - 1] == ')')
                {
                    printStatements.Add(new PrintStatement { AnalysisType = analysisType, Header = s });
                }
                else
                {
                    errors.Add(new ErrorInfo { LineNumber = t.Line, LineColumn = t.Char, Messsage = $"Unrecognized variable: '{s}'." });
                }
            }
        }

        private ElementStatement ProcessElement(char discriminator, Token[] tokens, SymbolTable tab)
        {
            return !elementProcessors.TryGetValue(discriminator, out var proc)
                ? new ErrorElementStatement(new List<ErrorInfo>
                {
                    new ErrorInfo
                    {
                        LineNumber = tokens[0].Line,
                        LineColumn = tokens[0].Char,
                        Messsage = $"Element type invalid or not implemented: {discriminator}."
                    }
                })
                : proc.Process(tokens, tab);
        }
    }

    public class ParserResult
    {
        public ParserResult(IEnumerable<ElementStatement> elementStatements, IEnumerable<PrintStatement> printStatements, IEnumerable<SimulationStatement> simulationStatements, IEnumerable<ErrorInfo> errors)
        {
            ElementStatements = elementStatements;
            PrintStatements = printStatements;
            SimulationStatements = simulationStatements;
            Errors = errors;
        }

        public IEnumerable<ElementStatement> ElementStatements { get; }
        public IEnumerable<PrintStatement> PrintStatements { get; }
        public IEnumerable<SimulationStatement> SimulationStatements { get; }
        public IEnumerable<ErrorInfo> Errors { get; set; }
    }

    public class SimulationStatement
    {
        public string AnalysisType { get; set; }
        public double StopTime { get; set; }
        public double TimeStep { get; set; }
        public double TMax { get; set; }
        public double StartTime { get; set; }
    }
}