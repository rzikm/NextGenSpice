using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>
    ///     Large signal model for <see cref="SubcircuitElement" />.
    /// </summary>
    public class LargeSignalSubcircuitModel : LargeSignalModelBase<SubcircuitElement>
    {
        private readonly ILargeSignalDeviceModel[] elements;
        private readonly int[] nodeMap;
        private readonly RedirectingEquationEditor redirectingEquationEditor;
        private readonly RedirectingSimulationContext subContext;

        public LargeSignalSubcircuitModel(SubcircuitElement definitionElement,
            IEnumerable<ILargeSignalDeviceModel> elements) :
            base(definitionElement)
        {
            this.elements = elements.ToArray();

            nodeMap = new int[definitionElement.InnerNodeCount + 1];
            redirectingEquationEditor = new RedirectingEquationEditor(nodeMap);
            subContext = new RedirectingSimulationContext(nodeMap);
        }

        /// <summary>
        ///     Set of classes that model this subcircuit.
        /// </summary>
        public IReadOnlyList<ILargeSignalDeviceModel> Elements => elements;

        /// <summary>
        ///     Specifies how often the model should be updated.
        /// </summary>
        public override ModelUpdateMode UpdateMode =>
            elements.Max(e => e.UpdateMode); // act like the most updating element.

        /// <summary>
        ///     Notifies model class that DC bias for given timepoint is established. This method can be used for processing
        ///     circuit equation solution
        ///     for current timepoint.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnDcBiasEstablished(ISimulationContext context)
        {
            base.OnDcBiasEstablished(context);
            subContext.TrueContext = context;

            foreach (var model in elements)
                model.OnDcBiasEstablished(context);
        }

        /// <summary>
        ///     Gets provider instance for specified attribute value or null if no provider for requested parameter exists. For
        ///     example "I" for the current flowing throught the two
        ///     terminal element.
        /// </summary>
        /// <returns>IPrintValueProvider for specified attribute.</returns>
        public override IEnumerable<IDeviceStatsProvider> GetDeviceStatsProviders()
        {
            return Enumerable.Empty<IDeviceStatsProvider>(); // no stats for subcircuit
        }

        /// <summary>
        ///     Allows models to register additional vairables to the linear system equations. E.g. branch current variables. And
        ///     perform other necessary initialization
        /// </summary>
        /// <param name="builder">The equation system builder.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void Initialize(IEquationSystemBuilder builder, ISimulationContext context)
        {
            base.Initialize(builder, context);

            for (var i = 1; i < nodeMap.Length; i++)
                nodeMap[i] = -1;

            for (var i = 0; i < DefinitionElement.TerminalNodes.Length; i++)
                nodeMap[DefinitionElement.TerminalNodes[i]] = DefinitionElement.ConnectedNodes[i];

            for (var i = 1; i < nodeMap.Length; i++)
                nodeMap[i] = nodeMap[i] < 0 ? builder.AddVariable() : nodeMap[i];

            redirectingEquationEditor.TrueEquationEditor = builder;

            foreach (var model in elements)
                model.Initialize(builder, context);
        }

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is called
        ///     once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            redirectingEquationEditor.TrueEquationEditor = equations;
            subContext.TrueContext = context;

            foreach (var model in elements)
                model.ApplyModelValues(redirectingEquationEditor, context);
        }

        /// <summary>
        ///     Applies model values before first DC bias has been established for the first time.
        /// </summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyInitialCondition(IEquationEditor equations, ISimulationContext context)
        {
            redirectingEquationEditor.TrueEquationEditor = equations;
            subContext.TrueContext = context;

            foreach (var model in elements)
                model.ApplyInitialCondition(redirectingEquationEditor, context);
        }

        /// <summary>
        ///     Equation editor with redirection layer for using inside subcircuit model.
        /// </summary>
        private class RedirectingEquationEditor : RedirectorBase, IEquationSystemBuilder
        {
            public RedirectingEquationEditor(int[] nodeMap) : base(nodeMap)
            {
            }

            /// <summary>
            ///     Decorated equation editor.
            /// </summary>
            public IEquationEditor TrueEquationEditor { get; set; }

            /// <summary>
            ///     Count of the variables in the equation.
            /// </summary>
            public int VariablesCount => TrueEquationEditor.VariablesCount;

            /// <summary>
            ///     Adds a value to coefficient on the given row and column of the equation matrix.
            /// </summary>
            /// <param name="row">The row.</param>
            /// <param name="column">The column.</param>
            /// <param name="value">The value to be added to the coefficients.</param>
            public void AddMatrixEntry(int row, int column, double value)
            {
                TrueEquationEditor.AddMatrixEntry(GetMappedIndex(row), GetMappedIndex(column), value);
            }

            /// <summary>
            ///     Adds a value to coefficient on the given position of the right hand side of the equation matrix.
            /// </summary>
            /// <param name="index">Index of the position.</param>
            /// <param name="value">The value.</param>
            public void AddRightHandSideEntry(int index, double value)
            {
                TrueEquationEditor.AddRightHandSideEntry(GetMappedIndex(index), value);
            }

            /// <summary>
            ///     Adds a variable to the equation system. Returns the index of the variable.
            /// </summary>
            /// <returns></returns>
            public int AddVariable()
            {
                return ((IEquationSystemBuilder) TrueEquationEditor).AddVariable();
            }
        }

        /// <summary>
        ///     Simulation context with redirection layer to be used inside subcircuit model.
        /// </summary>
        private class RedirectingSimulationContext : RedirectorBase, ISimulationContext
        {
            public RedirectingSimulationContext(int[] nodeMap) : base(nodeMap)
            {
            }


            /// <summary>
            ///     Decorated simulation context.
            /// </summary>
            public ISimulationContext TrueContext { get; set; }

            /// <summary>
            ///     Number of inner nodes.
            /// </summary>
            public double NodeCount => TrueContext.NodeCount;

            /// <summary>
            ///     Curent timepoint of the simulation.
            /// </summary>
            public double TimePoint => TrueContext.TimePoint;

            /// <summary>
            ///     Last timestep that was used to advance the timepoint.
            /// </summary>
            public double TimeStep => TrueContext.TimeStep;

            /// <summary>
            ///     Gets numerical solution for vairable with given index - either a node voltage or branch current.
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public double GetSolutionForVariable(int index)
            {
                return TrueContext.GetSolutionForVariable(GetMappedIndex(index));
            }

            /// <summary>
            ///     General parameters of the circuit that is simulated.
            /// </summary>
            public CircuitParameters CircuitParameters => TrueContext.CircuitParameters;
        }

        /// <summary>
        ///     Base class for <see cref="RedirectingEquationEditor" /> and <see cref="RedirectingSimulationContext" />
        ///     implementing calculation of redirected index.
        /// </summary>
        private class RedirectorBase
        {
            private readonly int[] nodeMap;

            public RedirectorBase(int[] nodeMap)
            {
                this.nodeMap = nodeMap;
            }

            /// <summary>
            ///     Gets redirected index for using in decorated EquationEditor.
            /// </summary>
            /// <param name="i"></param>
            /// <returns></returns>
            protected int GetMappedIndex(int i)
            {
                return i < nodeMap.Length ? nodeMap[i] : i;
            }
        }
    }
}