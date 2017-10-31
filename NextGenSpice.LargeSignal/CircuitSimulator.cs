using System;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal
{
    public class CircuitSimulator
    {
        public double Epsilon { get; } = 1e-15;
        public int MaxDcPointIterations { get; } = 1000;

        public int IterationCount { get; private set; }
        public double DeltaSquared { get; private set; }

        public CircuitSimulator(LargeSignalCircuitModel model)
        {
            this.Model = model;
            context = new SimulationContext() { NodeVoltages = model.NodeVoltages };
        }
        

        private EquationSystem equationSystem;
        private readonly SimulationContext context;

        public LargeSignalCircuitModel Model { get; }

        public void Simulate(Action<double[]> callback)
        {
            Initialize();

            throw new NotImplementedException();
        }

        private void Initialize()
        {
            BuildEquationSystem();
        }

        private void BuildEquationSystem()
        {
            var b = new EquationSystemBuilder();
            for (int i = 0; i < Model.NodeCount; i++)
            {
                b.AddVariable();
            }
            
            foreach (var element in Model.Elements)
            {
                element.Initialize(b);
            }

            foreach (var circuitElement in Model.LinearElements)
            {
                circuitElement.ApplyLinearModelValues(b, context);
            }

            equationSystem = b.Build();
        }

        public void EstablishDcBias()
        {
            Initialize();
            EstablishDcBias_Internal();
        }

        private void EstablishDcBias_Internal()
        {
            IterationCount = 0;
            DeltaSquared = 0;


            Iterate();

            if (!Model.IsLinear)
            {
                IterateUntilConvergence();
            }
        }

        private void IterateUntilConvergence()
        {
            double delta;
            do
            {
                delta = 0;
                var prevVoltages = (double[])equationSystem.Solution.Clone();

                Iterate();

                for (int i = 0; i < prevVoltages.Length; i++)
                {
                    var d = prevVoltages[i] - equationSystem.Solution[i];
                    delta += d * d;
                }

                if (++IterationCount == MaxDcPointIterations) break;

            } while (delta > Epsilon * Epsilon);

            DeltaSquared = delta;
        }

        private void Iterate()
        {
            UpdateEquationSystem();
            UpdateNodeValues();
            UpdateNonlinearElements();
            //DebugPrint();
        }

        private void UpdateNodeValues()
        {
            equationSystem.Solve();

            for (var i = 0; i < Model.NodeCount; i++)
            {
                Model.NodeVoltages[i] = equationSystem.Solution[i];
            }
        }

        private void UpdateEquationSystem()
        {
            equationSystem.Clear();

            foreach (var e in Model.NonlinearElements)
            {
                e.ApplyNonlinearModelValues(equationSystem, context);
            }

            foreach (var e in Model.TimeDependentElements)
            {
                e.ApplyTimeDependentModelValues(equationSystem, context);
            }
        }

        private void DebugPrint()
        {
            Console.WriteLine("Results:");
            for (var i = 0; i < Model.NodeCount; i++)
            {
                var v = equationSystem.Solution[i];
                Console.WriteLine($"node {i}: {v:##.0000}");
            }
            Console.WriteLine();
        }

        private void UpdateNonlinearElements()
        {
            foreach (var element in Model.NonlinearElements)
            {
                element.UpdateNonlinearModel(context);
            }
        }
    }
}