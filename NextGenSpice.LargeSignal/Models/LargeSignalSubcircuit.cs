﻿using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Devices;
using NextGenSpice.Numerics.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Large signal model for <see cref="SubcircuitDevice" />.</summary>
    public class LargeSignalSubcircuit : LargeSignalDeviceBase<SubcircuitDevice>, ILargeSignalSubcircuit
    {
        private readonly ILargeSignalDevice[] devices;
        private readonly int[] nodeMap;
        private readonly RedirectingSimulationContext subContext;

        public LargeSignalSubcircuit(SubcircuitDevice definitionDevice,
            IEnumerable<ILargeSignalDevice> devices) :
            base(definitionDevice)
        {
            this.devices = devices.ToArray();

            nodeMap = new int[definitionDevice.InnerNodeCount + 1];
            ;
            subContext = new RedirectingSimulationContext(nodeMap);
        }

        /// <summary>Set of classes that model this subcircuit.</summary>
        public IReadOnlyList<ILargeSignalDevice> Devices => devices;

        /// <summary>Specifies how often the model should be updated.</summary>
        public override ModelUpdateMode UpdateMode =>
            devices.Max(e => e.UpdateMode); // act like the most updating device.

        /// <summary>
        ///     Notifies model class that DC bias for given timepoint is established. This method can be used for processing
        ///     circuit equation solution for current timepoint.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnDcBiasEstablished(ISimulationContext context)
        {
            base.OnDcBiasEstablished(context);
            subContext.TrueContext = context;

            foreach (var model in devices)
                model.OnDcBiasEstablished(context);
        }

        /// <summary>
        ///     Gets provider instance for specified attribute value or null if no provider for requested parameter exists.
        ///     For example "I" for the current flowing throught the two terminal device.
        /// </summary>
        /// <returns>IPrintValueProvider for specified attribute.</returns>
        public override IEnumerable<IDeviceStatsProvider> GetDeviceStatsProviders()
        {
            return Enumerable.Empty<IDeviceStatsProvider>(); // no stats for subcircuit
        }

        /// <summary>Performs necessary initialization of the device, like mapping to the equation system.</summary>
        /// <param name="adapter">The equation system builder.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void Initialize(IEquationSystemAdapter adapter, ISimulationContext context)
        {
            for (var i = 1; i < nodeMap.Length; i++)
                nodeMap[i] = -1;

            for (var i = 0; i < DefinitionDevice.TerminalNodes.Length; i++)
                nodeMap[DefinitionDevice.TerminalNodes[i]] = DefinitionDevice.ConnectedNodes[i];

            for (var i = 1; i < nodeMap.Length; i++)
                nodeMap[i] = nodeMap[i] < 0 ? adapter.AddVariable() : nodeMap[i];

            var decorator = new RedirectingEquationEditor(nodeMap, adapter);
            foreach (var model in devices)
                model.Initialize(decorator, context);
        }

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
        ///     called once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(ISimulationContext context)
        {
            subContext.TrueContext = context;

            foreach (var model in devices)
                model.ApplyModelValues(context);
        }

        /// <summary>Applies model values before first DC bias has been established for the first time.</summary>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyInitialCondition(ISimulationContext context)
        {
            subContext.TrueContext = context;

            foreach (var model in devices)
                model.ApplyInitialCondition(context);
        }

        /// <summary>Equation adapter with redirection layer for using inside subcircuit model.</summary>
        private class RedirectingEquationEditor : RedirectorBase, IEquationSystemAdapter
        {
            private readonly IEquationSystemAdapter decoreated;

            public RedirectingEquationEditor(int[] nodeMap, IEquationSystemAdapter decoreated) : base(nodeMap)
            {
                this.decoreated = decoreated;
            }


            /// <summary>Adds a new variable to the equation system and returns the index of the variable;</summary>
            /// <returns></returns>
            public int AddVariable()
            {
                return decoreated.AddVariable();
            }

            /// <summary>Returns proxy class for coefficient at given coordinates in the equation matrix.</summary>
            /// <param name="row">Row coordinate.</param>
            /// <param name="column">Column coordinate.</param>
            /// <returns></returns>
            public IEquationSystemCoefficientProxy GetMatrixCoefficientProxy(int row, int column)
            {
                return decoreated.GetMatrixCoefficientProxy(GetMappedIndex(row), GetMappedIndex(column));
            }

            /// <summary>Returns proxy class for coefficient at given row in the right hand side vector.</summary>
            /// <param name="row">Row coordinate.</param>
            /// <returns></returns>
            public IEquationSystemCoefficientProxy GetRightHandSideCoefficientProxy(int row)
            {
                return decoreated.GetRightHandSideCoefficientProxy(GetMappedIndex(row));
            }

            /// <summary>Returns proxy class for the i-th variable of the solution.</summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public IEquationSystemSolutionProxy GetSolutionProxy(int index)
            {
                return decoreated.GetSolutionProxy(GetMappedIndex(index));
            }
        }

        /// <summary>Simulation context with redirection layer to be used inside subcircuit model.</summary>
        private class RedirectingSimulationContext : RedirectorBase, ISimulationContext
        {
            public RedirectingSimulationContext(int[] nodeMap) : base(nodeMap)
            {
            }


            /// <summary>Decorated simulation context.</summary>
            public ISimulationContext TrueContext { get; set; }

            /// <summary>Number of inner nodes.</summary>
            public double NodeCount => TrueContext.NodeCount;

            /// <summary>Curent timepoint of the simulation.</summary>
            public double TimePoint => TrueContext.TimePoint;

            /// <summary>Last timestep that was used to advance the timepoint.</summary>
            public double TimeStep => TrueContext.TimeStep;

            /// <summary>General parameters of the circuit that is simulated.</summary>
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

            /// <summary>Gets redirected index for using in decorated EquationEditor.</summary>
            /// <param name="i"></param>
            /// <returns></returns>
            protected int GetMappedIndex(int i)
            {
                return i < nodeMap.Length ? nodeMap[i] : i;
            }
        }
    }
}