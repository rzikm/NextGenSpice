using System.Collections.Generic;
using NextGenSpice.Core.Devices;
using NextGenSpice.Parser.Utils;

namespace NextGenSpice.Parser.Statements.Deferring
{
    /// <summary>Class representing spice subcircuit call statement.</summary>
    public class SubcircuitDeviceDeferredStatement : DeferredStatement
    {
        private readonly string deviceName;

        private readonly List<ErrorInfo> errors;
        private readonly Token subcircuitName;
        private readonly int[] terminals;

        private ISubcircuitDefinition model;

        public SubcircuitDeviceDeferredStatement(string deviceName, int[] terminals, Token subcircuitName)
        {
            this.deviceName = deviceName;
            this.terminals = terminals;
            this.subcircuitName = subcircuitName;

            errors = new List<ErrorInfo>();
        }

        /// <summary>Returns true if all prerequisites for the statements have been fulfilled and statement is ready to be applied.</summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool CanApply(ParsingContext context)
        {
            errors.Clear();
            if (!context.SymbolTable.TryGetSubcircuit(subcircuitName.Value, out model))
            {
                errors.Add(subcircuitName.ToErrorInfo(SpiceParserError.NoSuchSubcircuit));
                return false;
            }

            if (model.TerminalNodes.Length != terminals.Length)
            {
                errors.Add(subcircuitName.ToErrorInfo(SpiceParserError.InvalidTerminalCount));
                return false;
            }

            return true;
        }


        /// <summary>Returns set of errors due to which this stetement cannot be processed.</summary>
        /// <returns></returns>
        public override IEnumerable<ErrorInfo> GetErrors()
        {
            return errors;
        }

        /// <summary>Applies the statement in the given context.</summary>
        /// <param name="context"></param>
        public override void Apply(ParsingContext context)
        {
            context.CircuitBuilder.AddDevice(
                terminals,
                new SubcircuitDevice(model,
                    deviceName
                )
            );
        }
    }
}