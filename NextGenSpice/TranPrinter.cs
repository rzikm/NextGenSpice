using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpice
{
    class TranPrinter
    {
        private readonly LargeSignalCircuitModel model;

        private readonly List<Action<StringBuilder>> printers;
        private readonly StringBuilder sb;


        public TranPrinter(LargeSignalCircuitModel model)
        {
            this.model = model;
            sb = new StringBuilder();
            printers = new List<Action<StringBuilder>>();
        }

        public List<ErrorInfo> RegisterStatements(IEnumerable<PrintStatementDeprecated> statements)
        {
            List<ErrorInfo> errors = new List<ErrorInfo>();

            foreach (var statement in statements)
            {
                string s = statement.Token.Value;
                var name = s.Substring(1, s.Length - 3);

                // TODO: Check if name is a name of a node


                if (!model.TryGetElement(name, out var element))
                {
                    errors.Add(statement.Token.ToErrorInfo($"Symbol '{name}' is neither an element nor a node."));
                }
                else if (element is ITwoTerminalLargeSignalDeviceModel e)
                {
                    if (s[0] == 'I')
                        printers.Add(bld => bld.Append(e.Current));
                    else
                        printers.Add(bld => bld.Append(e.Voltage));
                }
                else
                {
                    errors.Add(statement.Token.ToErrorInfo($"Element '{name}' must have exactly two terminals to use in .PRINT statement."));
                }

            }

            //            if (!statements.Any())
            //            {
            //                // register all?
            //            }

            return errors;
        }

        public void Print(TextWriter output)
        {
            sb.Clear();
            sb.Append(model.CurrentTimePoint);
            foreach (var printer in printers)
            {
                sb.Append(" ");
                printer(sb);
            }

            output.WriteLine(sb);
        }
    }
}