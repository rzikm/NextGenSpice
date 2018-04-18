using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Parser.Statements.Deferring;
using NextGenSpice.Parser.Utils;

namespace NextGenSpice.Parser.Statements.Devices
{
    /// <summary>Class responsible for handling both current and voltage input source statements</summary>
    public abstract class InputSourceStatementProcessor : DeviceStatementProcessor
    {
        private readonly ParameterMapper<AmBehaviorParams> amMapper;
        private readonly ParameterMapper<ConstantBehaviorParams> dcMapper;
        private readonly ParameterMapper<ExponentialBehaviorParams> expMapper;
        private readonly ParameterMapper<PulseBehaviorParams> pulseMapper;
        private readonly ParameterMapper<SffmBehaviorParams> sffmMapper;
        private readonly ParameterMapper<SinusoidalBehaviorParams> sinMapper;

        protected InputSourceStatementProcessor()
        {
            sinMapper = new ParameterMapper<SinusoidalBehaviorParams>();
            expMapper = new ParameterMapper<ExponentialBehaviorParams>();
            pulseMapper = new ParameterMapper<PulseBehaviorParams>();
            amMapper = new ParameterMapper<AmBehaviorParams>();
            sffmMapper = new ParameterMapper<SffmBehaviorParams>();
            dcMapper = new ParameterMapper<ConstantBehaviorParams>();

            InitMappers();

            MinArgs = 3;
        }

        private void InitMappers()
        {
            // parameter order is given by SPICE code specification, see documentation for details

            pulseMapper.Map(c => c.InitialLevel, 0);
            pulseMapper.Map(c => c.PulseLevel, 1);
            pulseMapper.Map(c => c.Delay, 2);
            pulseMapper.Map(c => c.TimeRise, 3);
            pulseMapper.Map(c => c.TimeFall, 4);
            pulseMapper.Map(c => c.PulseWidth, 5);
            pulseMapper.Map(c => c.Period, 6);

            sinMapper.Map(c => c.DcOffset, 0);
            sinMapper.Map(c => c.Amplitude, 1);
            sinMapper.Map(c => c.Frequency, 2);
            sinMapper.Map(c => c.Delay, 3);
            sinMapper.Map(c => c.DampingFactor, 4);
            sinMapper.Map(c => c.PhaseOffset, 5, v => v / 180.0 * Math.PI); // convert from degrees to radians

            expMapper.Map(c => c.InitialLevel, 0);
            expMapper.Map(c => c.PulseLevel, 1);
            expMapper.Map(c => c.RiseDelay, 2);
            expMapper.Map(c => c.RiseTau, 3);
            expMapper.Map(c => c.FallDelay, 4);
            expMapper.Map(c => c.FallTau, 5);

            sffmMapper.Map(c => c.DcOffset, 0);
            sffmMapper.Map(c => c.Amplitude, 1);
            sffmMapper.Map(c => c.FrequencyCarrier, 2);
            sffmMapper.Map(c => c.ModulationIndex, 3);
            sffmMapper.Map(c => c.FrequencySignal, 4);

            amMapper.Map(c => c.SignalAmplitude, 0);
            amMapper.Map(c => c.FrequencyCarrier, 1);
            amMapper.Map(c => c.FrequencyModulation, 2);
            amMapper.Map(c => c.ModulationIndex, 3);
            amMapper.Map(c => c.PhaseOffset, 4, v => v / 180.0 * Math.PI); // convert from degrees to radians
            amMapper.Map(c => c.Delay, 5);

            dcMapper.Map(c => c.Value, 0);
        }

        /// <summary>Processes given set of statements.</summary>
        protected override void DoProcess()
        {
            var name = DeviceName;
            var nodes = GetNodeIndices(1, 2);

            if (RawStatement.Length < 4) return; // nothing more to do here

            DeferredStatement statement;
            if (char.IsDigit(RawStatement[3].Value[0])) // constant source
            {
                var val = GetValue(3);
                statement = GetStatement(name, nodes, new ConstantBehaviorParams {Value = val});
            }
            else // tran function
            {
                var paramTokens = Helper.Retokenize(RawStatement, 3).ToList();
                var param = GetBehaviorParam(paramTokens);
                statement = GetStatement(name, nodes, param);
            }

            if (Errors == 0)
                Context.DeferredStatements.Add(statement);
        }

        /// <summary>Gets behavior parameters for given list of tokens or null if no such transient function exists.</summary>
        /// <param name="paramTokens"></param>
        /// <returns></returns>
        private SourceBehaviorParams GetBehaviorParam(List<Token> paramTokens)
        {
            switch (paramTokens[0].Value)
            {
                case "PULSE":
                    return GetParameterTokens(pulseMapper, paramTokens, 3);

                case "PWL":
                    return GetPwlParams(paramTokens);

                case "EXP":
                    return GetParameterTokens(expMapper, paramTokens, 3);

                case "SIN":
                    return GetParameterTokens(sinMapper, paramTokens, 3);

                case "AM":
                    return GetParameterTokens(amMapper, paramTokens, 3);

                case "SFFM":
                    return GetParameterTokens(sffmMapper, paramTokens, 3);

                case "DC":
                    return GetParameterTokens(dcMapper, paramTokens, 1);

                default:
                    Error(paramTokens[0], SpiceParserError.UnknownTransientFunction);
                    return null;
            }
        }

        /// <summary>Functino responsible for parsing Piece-wise linear behavior of the input source.</summary>
        /// <param name="paramTokens"></param>
        /// <returns></returns>
        private SourceBehaviorParams GetPwlParams(List<Token> paramTokens)
        {
            var par = new PieceWiseLinearBehaviorParams();

            var currentTime = -double.Epsilon;

            var definitionPoints = new Dictionary<double, double>();
            for (var i = 1; i < paramTokens.Count; i += 2)
            {
                if (i >= paramTokens.Count - 2 && paramTokens[i].Value == "R") // repetition
                {
                    var rep = 0.0;
                    if (i < paramTokens.Count - 1)
                    {
                        rep = GetValue(paramTokens[i + 1]);
                        if (!definitionPoints.ContainsKey(rep))
                            Error(paramTokens[i + 1], SpiceParserError.NoBreakpointRepetition);
                    }

                    par.RepeatStart = rep;
                    break;
                }

                if (i == paramTokens.Count - 1) // this timepoint does not have corresponding value
                {
                    Error(paramTokens[i], SpiceParserError.TimePointWithoutValue);
                    break;
                }

                var time = GetValue(paramTokens[i]);
                var value = GetValue(paramTokens[i + 1]);

                if (time < 0) Error(paramTokens[i], SpiceParserError.NegativeTimepoint);
                else if (time <= currentTime) Error(paramTokens[i + 1], SpiceParserError.NonascendingTimepoints);

                definitionPoints[time] = value;

                if (!double.IsNaN(time))
                    currentTime = Math.Max(currentTime, time);
            }

            par.DefinitionPoints = definitionPoints;
            return par;
        }

        /// <summary>Generic function for parsing simple transient function parameters.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mapper"></param>
        /// <param name="paramTokens"></param>
        /// <param name="minArgc"></param>
        /// <returns></returns>
        private T GetParameterTokens<T>(ParameterMapper<T> mapper, List<Token> paramTokens, int minArgc) where T : new()
        {
            mapper.Target = new T();
            for (var i = 1;
                i <= Math.Min(mapper.ByIndexCount, paramTokens.Count - 1);
                i++) // start from 1 bc. first is method identifier
                mapper.Set(i - 1, GetValue(paramTokens[i]));

            if (paramTokens.Count < minArgc || paramTokens.Count > mapper.ByIndexCount + 1)
                Error(paramTokens[0], SpiceParserError.InvalidNumberOfArguments);

            var t = mapper.Target;
            mapper.Target = default(T); // free memory
            return t;
        }

        /// <summary>Factory method for a deferred statement that should be processed later.</summary>
        /// <param name="name"></param>
        /// <param name="nodes"></param>
        /// <param name="par"></param>
        /// <returns></returns>
        protected abstract DeferredStatement GetStatement(string name, int[] nodes, SourceBehaviorParams par);
    }
}