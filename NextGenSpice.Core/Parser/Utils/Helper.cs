using System;
using System.Collections.Generic;
using System.Linq;

namespace NextGenSpice.Core.Parser.Utils
{
    /// <summary>General helper class for SPICE code parsing.</summary>
    public static class Helper
    {
        private static readonly IDictionary<string, double> modifiers = new Dictionary<string, double>
        {
            ["T"] = 1e12,
            ["G"] = 1e9,
            ["MEG"] = 1e6,
            ["K"] = 1e3,
            ["MIL"] = 25.4e-6, // move it in front of "M" so that it gets examined first when comparing sequentially.
            ["M"] = 1e-3,
            ["U"] = 1e-6,
            ["N"] = 1e-9,
            ["P"] = 1e-12,
            ["F"] = 1e-15
        };

        private static readonly char[] digitchars = new[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.'};

        /// <summary>
        ///     Converts string value into double according to SPICE code guidelines - floating point number with optional
        ///     magnitude suffix. Ignores any characters after maximal parsed preffix.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static double ConvertValue(string s)
        {
            var i = s.LastIndexOfAny(digitchars);

            var suff = s.Substring(i + 1);
            s = s.Substring(0, i + 1);

            if (!double.TryParse(s, out var value)) value = double.NaN;

            // apply modifiers (e.g. 1MEG => 1 000 000)
            foreach (var modifier in modifiers)
            {
                if (!suff.StartsWith(modifier.Key)) continue;

                value *= modifier.Value;
                break;
            }

            return value;
        }

        /// <summary>
        ///     Helper method for getting exact numerical value from the given token, if token does not represent numerical
        ///     value, then corresponding ErrorInfo instance is added to the errors collection.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static double GetNumericValue(this Token t, ICollection<ErrorInfo> errors)
        {
            var val = ConvertValue(t.Value);
            if (double.IsNaN(val)) errors.Add(t.ToErrorInfo(SpiceParserError.NotANumber));
            return val;
        }


        /// <summary>
        ///     Makes ErrorInfo instance with given error code. LineNumber, LineColumn and message args are extracted from the
        ///     token.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public static ErrorInfo ToErrorInfo(this Token t, SpiceParserError errorCode, params object[] args)
        {
            object[] a = args.Length == 0 ? new object[] {t.Value} : args;
            return new ErrorInfo(errorCode, t.LineNumber, t.LineColumn, a);
        }

        /// <summary>
        ///     Helper method for normalizing parameter values e.g. for .MODEL statement or EXP tran function. Starting from
        ///     4th token, removes parentheses and separates tokens to be processed one by one. Adds encountered erros to errors
        ///     collection. E.g. Token set "V 0 1 SIN(0 1 100)" becomes "SIN 0 1 100".
        /// </summary>
        /// <example>Token set "V 0 1 SIN(0 1 100)" becomes "SIN 0 1 100"</example>
        /// <param name="tokens">Array of line tokens.</param>
        /// <param name="startPos">Starting position where to start retokenizing.</param>
        /// <returns></returns>
        public static List<Token> Retokenize(Token[] tokens, int startPos)
        {
            if (startPos < 0 || startPos >= tokens.Length) throw new ArgumentOutOfRangeException(nameof(startPos));

            var last = tokens[tokens.Length - 1];
            var parenthesized = false;
            if (last.Value.EndsWith(")"))
            {
                if (tokens[startPos].Value.Contains('(') ||
                    startPos + 1 < tokens.Length && tokens[startPos + 1].Value.StartsWith("("))
                    parenthesized = true;
            }

            if (!parenthesized)
                return tokens.Skip(startPos).ToList();

            var first = tokens[startPos];
            // params are parenthesized
            last.Value = last.Value.Substring(0, last.Value.Length - 1); // remove last closing parenthesis.
            List<Token> result = new List<Token>();

            // parenthesis inside first token
            var index = first.Value.IndexOf('(');
            if (index >= 0)
            {
                var second = new Token();
                second.LineNumber = first.LineNumber;
                second.LineColumn = first.LineColumn + index + 1;
                second.Value = first.Value.Substring(index + 1);

                first.Value = first.Value.Substring(0, index);

                result.Add(first);
                result.Add(second);
            }
            else // on the beginning of second token
            {
                result.Add(first);
                tokens[startPos + 1].Value = tokens[startPos + 1].Value.Substring(1);
                ++tokens[startPos + 1].LineColumn;
            }

            // add following unmodified tokens
            result.AddRange(tokens.Skip(startPos + 1));

            // remove all empty tokens
            result.RemoveAll(t => t.Value.Length == 0);

            return result;
        }
    }
}