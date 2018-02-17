using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Elements;
using NextGenSpice.Utils;

namespace NextGenSpice.Parser.Statements.Deferring
{
    /// <summary>
    ///     Class representing spice subcircuit call statement.
    /// </summary>
    public class SubcircuitElementDeferredStatement : DeferredStatement
    {
        private readonly string elementName;

        private readonly List<ErrorInfo> errors;
        private readonly Token subcircuitName;
        private readonly int[] terminals;

        private SubcircuitElement model;

        public SubcircuitElementDeferredStatement(string elementName, int[] terminals, Token subcircuitName)
        {
            this.elementName = elementName;
            this.terminals = terminals;
            this.subcircuitName = subcircuitName;

            errors = new List<ErrorInfo>();
        }

        /// <summary>
        ///     Returns true if all prerequisites for the statements have been fulfilled and statement is ready to be applied.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool CanApply(ParsingContext context)
        {
            errors.Clear();
            if (!context.SymbolTable.TryGetSubcircuit(subcircuitName.Value, out model))
            {
                errors.Add(subcircuitName.ToErrorInfo($"Subcircuit does not exist '{subcircuitName.Value}'"));
                return false;
            }
            if (model.TerminalNodes.Length != terminals.Length)
            {
                errors.Add(subcircuitName.ToErrorInfo(
                    $"Subcircuit has wrong number of terminals '{subcircuitName.Value}'"));
                return false;
            }

            return true;
        }


        /// <summary>
        ///     Returns set of errors due to which this stetement cannot be processed.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<ErrorInfo> GetErrors()
        {
            return errors;
        }

        /// <summary>
        ///     Applies the statement in the given context.
        /// </summary>
        /// <param name="context"></param>
        public override void Apply(ParsingContext context)
        {
            context.CircuitBuilder.AddElement(
                terminals,
                new SubcircuitElement(
                    model.InnerNodeCount,
                    model.TerminalNodes, 
                    model.Elements.Select(e => e.Clone()),
                    elementName
                )
            );
        }
    }
}