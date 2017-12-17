using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace NextGenSpice
{
    public static class ConvertorHelpers
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
            var i = s.LastIndexOfAny(new[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.'});

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
            if (double.IsNaN(val)) errors.Add(new ErrorInfo() {LineColumn=t.Line, LineNumber = t.Line, Messsage = $"Cannot convert '{t.Value}' to a numeric value"});
            return val;
        }
    }
}