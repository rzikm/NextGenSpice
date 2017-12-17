using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Elements;

namespace NextGenSpice
{
    public abstract class InputSourceStatementProcessor : ElementStatementProcessor
    {
        private readonly ParameterMapper<SinusoidalBehaviorParams> sinMapper;
        private readonly ParameterMapper<ExponentialBehaviorParams> expMapper;
        private readonly ParameterMapper<PulseBehaviorParams> pulseMapper;
        private readonly ParameterMapper<AmBehaviorParams> amMapper;
        private readonly ParameterMapper<SffmBehaviorParams> sffmMapper;

        protected InputSourceStatementProcessor()
        {
            sinMapper = new ParameterMapper<SinusoidalBehaviorParams>();
            expMapper = new ParameterMapper<ExponentialBehaviorParams>();
            pulseMapper = new ParameterMapper<PulseBehaviorParams>();
            amMapper = new ParameterMapper<AmBehaviorParams>();
            sffmMapper = new ParameterMapper<SffmBehaviorParams>();

            InitMappers();
        }

        private void InitMappers()
        {
            pulseMapper.Map(c => c.Value1, 0);
            pulseMapper.Map(c => c.Value2, 1);
            pulseMapper.Map(c => c.Delay, 2);
            pulseMapper.Map(c => c.TimeRise, 3);
            pulseMapper.Map(c => c.TimeFall, 4);
            pulseMapper.Map(c => c.PulseWidth, 5);
            pulseMapper.Map(c => c.Period, 6);

            sinMapper.Map(c => c.BaseValue, 0);
            sinMapper.Map(c => c.Amplitude, 1);
            sinMapper.Map(c => c.Frequency, 2);
            sinMapper.Map(c => c.Delay, 3);
            sinMapper.Map(c => c.DampingFactor, 4);
            sinMapper.Map(c => c.Phase, 5, v => v / 180.0 * Math.PI);

            expMapper.Map(c => c.Value1, 0);
            expMapper.Map(c => c.Value2, 1);
            expMapper.Map(c => c.RiseDelay, 2);
            expMapper.Map(c => c.TauRise, 3);
            expMapper.Map(c => c.FallDelay, 4);
            expMapper.Map(c => c.TauFall, 5);

            sffmMapper.Map(c => c.BaseValue, 0);
            sffmMapper.Map(c => c.Amplitude, 1);
            sffmMapper.Map(c => c.FrequencyCarrier, 2);
            sffmMapper.Map(c => c.ModilationIndex, 3);
            sffmMapper.Map(c => c.FrequencySignal, 4);

            amMapper.Map(c => c.SignalAmplitude, 0);
            amMapper.Map(c => c.FrequencyCarrier, 1);
            amMapper.Map(c => c.FrequencyModulation, 2);
            amMapper.Map(c => c.Offset, 3);
            amMapper.Map(c => c.Delay, 4);
        }

        protected override ElementStatement DoProcess(Token[] tokens, List<ErrorInfo> errors)
        {
            if (tokens.Length < 4)
            {
                errors.Add(InvalidNumberOfArguments(tokens[0]));
                return null;
            }

            var name = DeclareElement(tokens[0], errors);
            var nodes = GetNodeIndices(tokens, 1, 2, errors);

            if (char.IsDigit(tokens[3].Value[0])) // constant source
            {
                var val = GetValue(tokens[3], errors);
                return GetStatement(name, nodes, new ConstantBehaviorParams { Value = val });
            }

            // tran function

            var paramTokens = GetParameterTokens(tokens, errors);
            var param = GetBehaviorParam(paramTokens, errors);
            if (paramTokens.Count < 3 && param != null)
            {
                errors.Add(Error(paramTokens[0], $"Too few arguments for transient function '{paramTokens[0].Value}'"));
                return null;
            }

            return GetStatement(name, nodes, param);
        }

        private SourceBehaviorParams GetBehaviorParam(List<Token> paramTokens, List<ErrorInfo> errors)
        {
            switch (paramTokens[0].Value)
            {
                case "PULSE":
                    return GetParameterTokens(pulseMapper, paramTokens, errors);

                case "PWL":
                    return GetPwlParams(paramTokens, errors);

                case "EXP":
                    return GetParameterTokens(expMapper, paramTokens, errors);

                case "SIN":
                    return GetParameterTokens(sinMapper, paramTokens, errors);

                case "AM":
                    return GetParameterTokens(amMapper, paramTokens, errors);

                case "SFFM":
                    return GetParameterTokens(sffmMapper, paramTokens, errors);

                default:
                    errors.Add(Error(paramTokens[0], $"Unknown transient source: '{paramTokens[0].Value}'"));
                    return null;
            }
        }

        private SourceBehaviorParams GetPwlParams(List<Token> paramTokens, List<ErrorInfo> errors)
        {
            var par = new PieceWiseLinearBehaviorParams();

            var currentTime = -double.Epsilon;

            var definitionPoints = new Dictionary<double, double>();
            for (int i = 1; i < paramTokens.Count; i += 2)
            {
                if (i >= paramTokens.Count - 2 && paramTokens[i].Value == "R") // repetition
                {
                    var rep = 0.0;
                    if (i < paramTokens.Count - 1)
                    {
                        rep = GetValue(paramTokens[i + 1], errors);
                        if (!definitionPoints.ContainsKey(rep))
                            errors.Add(Error(paramTokens[i + 1], "Repetition point must be equal to a function breakpoint."));
                    }
                  
                    par.RepeatStart = rep;
                    break;
                }

                if (i == paramTokens.Count - 1) // this timepoint does not have corresponding value
                {
                    errors.Add(Error(paramTokens[i], "Timpeoint without corresponding value."));
                    break;
                }

                var time = GetValue(paramTokens[i], errors);
                var value = GetValue(paramTokens[i + 1], errors);

                if (time < 0) errors.Add(Error(paramTokens[i], "Timepoints must be nonnegative."));
                else if (time <= currentTime) errors.Add(Error(paramTokens[i + 1], "Timepoints must be ascending."));

                definitionPoints[time] = value;

                if (!double.IsNaN(time))
                    currentTime = Math.Max(currentTime, time);
            }

            par.DefinitionPoints = definitionPoints;
            return par;
        }

        private T GetParameterTokens<T>(ParameterMapper<T> mapper, List<Token> paramTokens, List<ErrorInfo> errors) where T : new()
        {
            mapper.Target = new T();
            for (int i = 1; i < Math.Min(mapper.ByIndexCount, paramTokens.Count); i++) // start from 1 bc. first is method identifier
            {
                mapper.Set(i - 1, GetValue(paramTokens[i], errors));
            }

            if (paramTokens.Count > mapper.ByIndexCount + 1)
            {
                errors.Add(Error(paramTokens[mapper.ByIndexCount], $"Too many arguments for transient source '{paramTokens[0].Value}'"));
            }

            var t = mapper.Target;
            mapper.Target = default(T);
            return t;
        }

        private List<Token> GetParameterTokens(Token[] tokens, List<ErrorInfo> errors)
        {
            List<Token> paramTokens = new List<Token>();

            var parentheses = false;


            for (var t = 3; t < tokens.Length; t++)
            {
                var line = tokens[t].Line;
                var col = tokens[t].Char;
                var s = tokens[t].Value;

                int i;
                if (!parentheses && (i = s.IndexOf('(')) >= 0)
                {
                    parentheses = true;

                    if (i > 0)
                        paramTokens.Add(new Token()
                        {
                            Line = line,
                            Char = col,
                            Value = s.Substring(0, i)
                        });

                    col += i + 1;
                    s = s.Substring(i + 1);
                }

                if (parentheses && (i = s.IndexOf(')')) >= 0)
                {
                    if (i > 0)
                        paramTokens.Add(new Token()
                        {
                            Line = line,
                            Char = col,
                            Value = s.Substring(0, i)
                        });

                    col += i;
                    if (t < tokens.Length - 1)
                        errors.Add(Error(tokens[t + 1], "Unexpected tokens after end of statement."));
                    if (paramTokens.Count < 2) errors.Add(Error(new Token()
                    {
                        Line = line,
                        Char = col,
                        Value = ")"
                    }, "Unexpected end of parameter list."));

                    return paramTokens;
                }

                if (s.Length > 0)
                    paramTokens.Add(new Token()
                    {
                        Line = line,
                        Char = col,
                        Value = s
                    });
            }

            var last = tokens[tokens.Length - 1];
            if (parentheses) // unterminated parentheses
            {
                var t = new Token()
                {
                    Line = last.Line,
                    Char = last.Char,
                    Value = last.Value
                };
                t.Char += t.Value.Length;
                t.Value = "";

                errors.Add(Error(t, "Unterminated transient function"));
            }

            return paramTokens;
        }

        protected abstract ElementStatement GetStatement(string name, int[] nodes, SourceBehaviorParams par);
    }
}