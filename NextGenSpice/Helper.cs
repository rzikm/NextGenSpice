using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;

namespace NextGenSpice
{
    public static class Helper
    {
        private static readonly IDictionary<string, double> modifiers = new Dictionary<string, double>
        {
            ["T"] = 1e12,
            ["G"] = 1e9,
            ["MEG"] = 1e6,
            ["K"] = 1e3,
            ["MIL"] = 25.4e-6,
            ["M"] = 1e-3,
            ["U"] = 1e-6,
            ["N"] = 1e-9,
            ["P"] = 1e-12,
            ["F"] = 1e-15,
        };

        public static double ConvertValue(string s)
        {
            var i = s.LastIndexOfAny(new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.' });

            var suff = s.Substring(i + 1);
            s = s.Substring(0, i + 1);

            if (!double.TryParse(s, out var value)) value = double.NaN;

            foreach (var modifier in modifiers)
            {
                if (suff.StartsWith(modifier.Key))
                {
                    value *= modifier.Value;
                    break;
                }
            }

            return value;
        }

        public static double GetNumericValue(this Token t, List<ErrorInfo> errors)
        {
            double val = ConvertValue(t.Value);
            if (double.IsNaN(val)) errors.Add(new ErrorInfo() { LineColumn = t.LineNumber, LineNumber = t.LineNumber, Messsage = $"Cannot convert '{t.Value}' to a numeric value" });
            return val;
        }

        public static ErrorInfo ToErrorInfo(this Token t, string message)
        {
            return new ErrorInfo()
            {
                LineColumn = t.LineColumn,
                LineNumber = t.LineNumber,
                Messsage = message
            };
        }

        // TODO: reimplement so that it correctly handles parentheses (currently, D 1 (111) does not couse errors)
        public static List<Token> Retokenize(Token[] tokens, List<ErrorInfo> errors)
        {
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