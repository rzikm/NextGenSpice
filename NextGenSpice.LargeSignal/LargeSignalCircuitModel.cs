#define dd_precision
//#define qd_precision


using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.Core.Exceptions;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal.Models;
using Numerics;
using Numerics.Precision;

namespace NextGenSpice.LargeSignal
{
    public class LargeSignalCircuitModel : IAnalysisCircuitModel<ILargeSignalDeviceModel>
    {
        private class SimulationContext : ISimulationContext
        {
            public SimulationContext(int nodeCount)
            {
                NodeCount = nodeCount;
                CircuitParameters = new CircuitParameters();
            }

            public double NodeCount { get; }
            public double Time { get; set; }
            public double TimeStep { get; set; }

            public double GetSolutionForVariable(int index)
            {
                return EquationSystem.Solution[index];
            }

            public CircuitParameters CircuitParameters { get; }

#if qd_precision
            public QdEquationSystem EquationSystem { get; set; }
#elif dd_precision
            public DdEquationSystem EquationSystem { get; set; }
#else
            public EquationSystem EquationSystem { get; set; }
#endif
        }

        private SimulationContext context;

#if qd_precision
        private QdEquationSystem equationSystem;
#elif dd_precision
        private DdEquationSystem equationSystem;
#else
        private EquationSystem equationSystem;
#endif

        public LargeSignalCircuitModel(IEnumerable<double> initialVoltages, List<ILargeSignalDeviceModel> elements)
        {
            NodeVoltages = initialVoltages.ToArray();
            Elements = elements;

            constElements = elements.Where(e => !e.IsNonlinear && !e.IsTimeDependent).ToArray();
            linearTimeDependentElements = elements.Where(e => !e.IsNonlinear && e.IsTimeDependent).ToArray();
            nonlinearElements = elements.Where(e => e.IsNonlinear).ToArray();

            elementLookup = elements.Where(e => !string.IsNullOrEmpty(e.Name)).ToDictionary(e => e.Name);
        }

        public double[] NodeVoltages { get; }
        public int NodeCount => NodeVoltages.Length;
        public IReadOnlyList<ILargeSignalDeviceModel> Elements { get; }

        private readonly IReadOnlyList<ILargeSignalDeviceModel> constElements;
        private readonly IReadOnlyList<ILargeSignalDeviceModel> nonlinearElements;
        private readonly IReadOnlyList<ILargeSignalDeviceModel> linearTimeDependentElements;

        private readonly Dictionary<string, ILargeSignalDeviceModel> elementLookup;

        public ILargeSignalDeviceModel GetElement(string name)
        {
            return elementLookup[name];
        }

        public bool TryGetElement(string name, out ILargeSignalDeviceModel value)
        {
            return elementLookup.TryGetValue(name, out value);
        }

        public bool IsLinear => !nonlinearElements.Any();


        // iteration dependent variables

        public double NonlinearIterationEpsilon { get; set; } = 1e-4;
        public int MaxDcPointIterations { get; set; } = 1000;
        public double MaxTimeStep { get; set; } = 1e-6;

        public double TimestepAbsoluteEpsilon { get; set; }
        public double TimestepRelativeEpsilon { get; set; }


        public int IterationCount { get; private set; }
        public double DeltaSquared { get; private set; }


        public double CurrentTimePoint => context?.Time ?? 0.0;

        public void Simulate(Action<double[]> callback)
        {
            EnsureInitialized();

            while (true)
            {
                EstablishDcBias_Internal(e => e.ApplyModelValues(equationSystem, context));
                callback(NodeVoltages);
            }
        }

        public void AdvanceInTime(double milliseconds)
        {
            if (milliseconds < 0) throw new ArgumentOutOfRangeException(nameof(milliseconds));
            if (context == null) EstablishDcBias();

            while (milliseconds > 0)
            {
                var step = 2 * Math.Min(MaxTimeStep, milliseconds);

                var timePoint = context.Time;
                //                do
                //                {
                step /= 2;
                context.Time = timePoint + step;
                context.TimeStep = step;
                EstablishDcBias_Internal(e => e.ApplyModelValues(equationSystem, context));
                //                } while (!EstablishDcBias_Internal(e => e.ApplyModelValues(equationSystem, context)));

                milliseconds -= step;
            }
        }

        private void EnsureInitialized()
        {
            if (context != null) return;

            context = new SimulationContext(NodeCount);
            BuildEquationSystem();
            context.EquationSystem = equationSystem;
        }

        private void BuildEquationSystem()
        {
#if qd_precision
            var b = new QdEquationSystemBuilder();
#elif dd_precision
            var b = new DdEquationSystemBuilder();
#else
            var b = new EquationSystemBuilder();
#endif
            for (var i = 0; i < NodeCount; i++)
                b.AddVariable();

            foreach (var element in Elements)
                element.RegisterAdditionalVariables(b, context);

            foreach (var element in constElements)
                element.ApplyModelValues(b, context);

            equationSystem = b.Build();
        }

        public void EstablishDcBias()
        {
            EnsureInitialized();

            // TODO: really reinitialize?
            context.TimeStep = 0;
            context.Time = 0;

            if (!EstablishDcBias_Internal(e => e.ApplyInitialCondition(equationSystem, context)))
                throw new NonConvergenceException();
        }

        private bool EstablishDcBias_Internal(Action<ILargeSignalDeviceModel> updater)
        {
            IterationCount = 0;
            DeltaSquared = 0;

            equationSystem.Clear();

            UpdateEquationSystem(updater, linearTimeDependentElements);
            equationSystem.Backup();
            UpdateEquationSystem(updater, nonlinearElements);

            UpdateNodeValues();
            //            DebugPrint();
            if (!IsLinear && !IterateUntilConvergence(updater))
                return false;

            PostProcess();

            return true;
        }

        private bool IterateUntilConvergence(Action<ILargeSignalDeviceModel> updater)
        {
            double delta;

            do
            {
                delta = 0;
                var prevVoltages = (double[]) equationSystem.Solution.Clone();

                equationSystem.Restore();

                UpdateEquationSystem(updater, nonlinearElements);
                UpdateNodeValues();
                //            DebugPrint();

                for (var i = 0; i < prevVoltages.Length; i++)
                {
                    var d = prevVoltages[i] - equationSystem.Solution[i];
                    delta += d * d;
                }

                if (++IterationCount == MaxDcPointIterations) return false;
            } while (delta > NonlinearIterationEpsilon * NonlinearIterationEpsilon);

            DeltaSquared = delta;

            return true;
        }


        private void PostProcess()
        {
            foreach (var el in Elements)
                el.OnDcBiasEstablished(context);
        }

        private void UpdateNodeValues()
        {
            // ensure ground has 0 voltage
            var m = equationSystem.Matrix;

#if qd_precision
            for (int i = 0; i < m.SideLength; i++)
            {
                m[i, 0] = qd_real.Zero;
                m[0, i] = qd_real.Zero;
            }

            m[0, 0] = new qd_real(1);
            equationSystem.RightHandSide[0] = qd_real.Zero;
#elif dd_precision
            for (var i = 0; i < m.Size; i++)
            {
                m[i, 0] = dd_real.Zero;
                m[0, i] = dd_real.Zero;
            }

            m[0, 0] = new dd_real(1);
            equationSystem.RightHandSide[0] = dd_real.Zero;
#else
            for (int i = 0; i < m.SideLength; i++)
            {
                m[i, 0] = 0;
                m[0, i] = 0;
            }

            m[0, 0] = 1;
            equationSystem.RightHandSide[0] = 0;
#endif

            equationSystem.Solve();

            for (var i = 0; i < NodeCount; i++)
                NodeVoltages[i] = equationSystem.Solution[i];
        }

        private void UpdateEquationSystem(Action<ILargeSignalDeviceModel> updater,
            IEnumerable<ILargeSignalDeviceModel> elements)
        {
            foreach (var e in elements) updater(e);
        }
    }
}