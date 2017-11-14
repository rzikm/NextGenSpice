using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpice.LargeSignal
{
    public class LargeSignalCircuitModel : IAnalysisCircuitModel<ILargeSignalDeviceModel>
    {
        public LargeSignalCircuitModel(IEnumerable<double> initialVoltages, List<ILargeSignalDeviceModel> elements)
        {
            NodeVoltages = initialVoltages.ToArray();
            Elements = elements;
            LinearElements = elements.OfType<ILinearLargeSignalDeviceModel>().ToList();
            NonlinearElements = elements.OfType<INonlinearLargeSignalDeviceModel>().ToList();
            TimeDependentElements = elements.OfType<ITimeDependentLargeSignalDeviceModel>().ToList();
        }

        public double[] NodeVoltages { get; }
        public int NodeCount => NodeVoltages.Length;
        public IReadOnlyList<ILinearLargeSignalDeviceModel> LinearElements { get; }
        public IReadOnlyList<INonlinearLargeSignalDeviceModel> NonlinearElements { get; }
        public IReadOnlyList<ITimeDependentLargeSignalDeviceModel> TimeDependentElements { get; }
        public bool IsLinear => NonlinearElements.Count == 0;
        public IReadOnlyList<ILargeSignalDeviceModel> Elements { get; }

        public double Epsilon { get; } = 1e-15;
        public int MaxDcPointIterations { get; set; } = 1000;

        public double MaxTimeStep { get; set; } = 1e-6;

        public int IterationCount { get; private set; }
        public double DeltaSquared { get; private set; }

        private SimulationContext context;
        private EquationSystem equationSystem;

        public void Simulate(Action<double[]> callback)
        {
            EnsureInitialized();

            while (true)
            {
                EstablishDcBias_Internal();
                callback(NodeVoltages);
                UpdateTimeDependentElements();
            }
        }

        public void AdvanceInTime(double milliseconds)
        {
            if (milliseconds < 0) throw new ArgumentOutOfRangeException(nameof(milliseconds));
            EnsureInitialized();
            while (milliseconds > 0)
            {
                var step = Math.Min(MaxTimeStep, milliseconds);
                milliseconds -= step;

                AdvanceInTime_Internal(step);
            }
        }

        private void AdvanceInTime_Internal(double step)
        {
            context.Timestep = step;

            UpdateTimeDependentElements();
            EstablishDcBias_Internal();

            context.Time += step;
        }

        private void EnsureInitialized()
        {
            if (context != null) return;

            BuildEquationSystem();
            context = new SimulationContext(NodeCount, equationSystem);
        }

        private void BuildEquationSystem()
        {
            var b = new EquationSystemBuilder();
            for (int i = 0; i < NodeCount; i++)
            {
                b.AddVariable();
            }

            foreach (var element in Elements)
            {
                element.Initialize(b);
            }

            foreach (var circuitElement in LinearElements)
            {
                circuitElement.ApplyLinearModelValues(b, context);
            }

            equationSystem = b.Build();
        }

        public void EstablishDcBias()
        {
            EnsureInitialized();

            // TODO: really reinitialize?
            context.Timestep = 0;
            context.Time = 0;

            if (!EstablishDcBias_Internal())
                throw new NonConvergenceException();
        }

        private bool EstablishDcBias_Internal()
        {
            IterationCount = 0;
            DeltaSquared = 0;


            Iterate_DcBias();
            if (!IsLinear && !IterateUntilConvergence())
            {
                return false;
            }

            //DistributeCurrent();

            return true;
        }

        private bool IterateUntilConvergence()
        {
            double delta;
            do
            {
                delta = 0;
                var prevVoltages = (double[])equationSystem.Solution.Clone();

                Iterate_DcBias();

                for (int i = 0; i < prevVoltages.Length; i++)
                {
                    var d = prevVoltages[i] - equationSystem.Solution[i];
                    delta += d * d;
                }

                if (++IterationCount == MaxDcPointIterations) return false;

            } while (delta > Epsilon * Epsilon);

            DeltaSquared = delta;

            return true;
        }
        

        private void Iterate_DcBias()
        {
            UpdateEquationSystem();
            UpdateNodeValues();
            UpdateNonlinearElements();
            //DebugPrint();
        }

        private void UpdateNodeValues()
        {
            equationSystem.Solve();

            for (var i = 0; i < NodeCount; i++)
            {
                NodeVoltages[i] = equationSystem.Solution[i];
            }
        }

        private void UpdateEquationSystem()
        {
            equationSystem.Clear();

            foreach (var e in NonlinearElements)
            {
                e.ApplyNonlinearModelValues(equationSystem, context);
            }

            foreach (var e in TimeDependentElements)
            {
                e.ApplyTimeDependentModelValues(equationSystem, context);
            }
        }

        private void DebugPrint()
        {
            Console.WriteLine("Results:");
            for (var i = 0; i < NodeCount; i++)
            {
                var v = equationSystem.Solution[i];
                Console.WriteLine($"node {i}: {v:##.0000}");
            }
            Console.WriteLine();
        }

        private void UpdateNonlinearElements()
        {
            foreach (var element in NonlinearElements)
            {
                element.UpdateNonlinearModel(context);
            }
        }

        private void UpdateTimeDependentElements()
        {
            foreach (var element in TimeDependentElements)
            {
                element.AdvanceTimeDependentModel(context);
            }
        }
    }
}