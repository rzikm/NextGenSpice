using System.Collections.Generic;
using NextGenSpice.Parser;

namespace NextGenSpice.Utils
{
    /// <summary>
    /// General helper class for SPICE code parsing.
    /// </summary>
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
            ["F"] = 1e-15,
        };

        /// <summary>
        /// Converts string value into double according to SPICE code guidelines - floating point number with optional magnitude suffix. Ignores any characters after maximal parsed preffix.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static double ConvertValue(string s)
        {
            //TODO: use more sophisticated algorithm for determining maximum valid preffix
            var i = s.LastIndexOfAny(new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.' });

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
        /// Helper method for getting exact numerical value from the given token, if token does not represent numerical value, then corresponding ErrorInfo instance is added to the errors collection.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static double GetNumericValue(this Token t, ICollection<ErrorInfo> errors)
        {
            double val = ConvertValue(t.Value);
            if (double.IsNaN(val)) errors.Add(t.ToErrorInfo($"Cannot convert '{t.Value}' to a numeric value"));
            return val;
        }

        /// <summary>
        /// Makes ErrorInfo instance with given message that has location set according to the given token.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ErrorInfo ToErrorInfo(this Token t, string message)
        {
            return new ErrorInfo()
            {
                LineColumn = t.LineColumn,
                LineNumber = t.LineNumber,
                Messsage = message
            };
        }

        /// <summary>
        /// Helper method for normalizing parameter values e.g. for .MODEL statement or EXP tran function. Starting from 4th token, removes parentheses and separates tokens to be processed one by one.
        /// Adds encountered erros to errors collection. E.g. Token set "V 0 1 SIN(0 1 100)" becomes "SIN 0 1 100".
        /// </summary>
        /// <example>
        /// Token set "V 0 1 SIN(0 1 100)" becomes "SIN 0 1 100"
        /// </example> 
        /// <param name="tokens"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static List<Token> Retokenize(Token[] tokens, List<ErrorInfo> errors)
        {
            // TODO: reimplement so that it correctly handles parentheses (currently, D 1 (111) does not couse errors)
            List<Token> result = new List<Token>();

            var parentheses = false;

            for (var t = 3; t < tokens.Length; t++)
            {
                var line = tokens[t].LineNumber;
                var col = tokens[t].LineColumn;
                var s = tokens[t].Value;

                int i;
                if (!parentheses && (i = s.IndexOf('(')) >= 0)
                {
                    parentheses = true;

                    if (i > 0)
                        result.Add(new Token
                        {
                            LineNumber = line,
                            LineColumn = col,
                            Value = s.Substring(0, i)
                        });

                    col += i + 1;
                    s = s.Substring(i + 1);
                }

                if (parentheses && (i = s.IndexOf(')')) >= 0)
                {
                    if (i > 0)
                        result.Add(new Token
                        {
                            LineNumber = line,
                            LineColumn = col,
                            Value = s.Substring(0, i)
                        });

                    col += i;
                    if (t < tokens.Length - 1)
                        errors.Add(tokens[t + 1].ToErrorInfo("Unexpected tokens after end of statement."));

                    return result;
                }

                if (s.Length > 0)
                    result.Add(new Token
                    {
                        LineNumber = line,
                        LineColumn = col,
                        Value = s
                    });
            }

            var last = tokens[tokens.Length - 1];
            if (parentheses) // unterminated parentheses
            {
                var t = new Token
                {
                    LineNumber = last.LineNumber,
                    LineColumn = last.LineColumn,
                    Value = last.Value
                };
                t.LineColumn += t.Value.Length;
                t.Value = "";

                errors.Add(t.ToErrorInfo("Unterminated transient function"));
            }

            return result;
        }
    }
}