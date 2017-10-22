using System;
using System.Linq;
using NextGenSpice.Elements;
using NextGenSpice.Equations;

namespace NextGenSpice.Circuit
{
    public class CircuitSimulator
    {
        public double Epsilon { get; } = 1e-15;
        public int MaxDcPointIterations { get; } = 1000;

        public int IterationCount { get; private set; }
        public double DeltaSquared { get; private set; }

        public CircuitSimulator(ElectricCircuitDefinition circuitDefinition)
        {
            this.CircuitDefinition = circuitDefinition;
            context = new SimulationContext();
        }

        private IEquationSystem equationSystem;
        public ElectricCircuitDefinition CircuitDefinition { get; }
        private readonly SimulationContext context;

        private ICircuitModel model;

        public void Simulate(Action<double[]> callback) 
        {
            EnsureInitialized();

            throw new NotImplementedException();
        }

        private void EnsureInitialized()
        {
            if (model == null)
            {
                model = CircuitDefinition.GetLargeSignalModel();
                foreach (var element in model.Elements)
                {
                    element.Initialize();
                }
            }
        }

        private void BuildEquationSystem()
        {
            var b = new EquationSystemBuilder();
            for (int i = 0; i < CircuitDefinition.Nodes.Count; i++)
            {
                b.AddVariable();
            }
            foreach (var circuitElement in model.LinearElements)
            {
                circuitElement.ApplyLinearModelValues(b, context);
            }

            equationSystem = b.Build();
        }

        public void EstablishDcBias()
        {
            EnsureInitialized();
            IterationCount = 0;
            DeltaSquared = 0;

            BuildEquationSystem();

            Iterate();

            if (!model.IsLinear)
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
                var prevVoltages = (double[]) equationSystem.Solution.Clone();
                
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

            for (var i = 0; i < CircuitDefinition.Nodes.Count; i++)
            {
                CircuitDefinition.Nodes[i].Voltage = equationSystem.Solution[i];
            }
        }

        private void UpdateEquationSystem()
        {
            equationSystem.Clear();

            foreach (var e in model.NonlinearElements)
            {
                e.ApplyNonlinearModelValues(equationSystem, context);
            }

            foreach (var e in model.TimeDependentElements)
            {
                e.ApplyTimeDependentModelValues(equationSystem, context);
            }
        }

        private void DebugPrint()
        {
            Console.WriteLine("Results:");
            for (var i = 0; i < CircuitDefinition.Nodes.Count; i++)
            {
                var v = equationSystem.Solution[i];
                Console.WriteLine($"node {i}: {v:##.0000}");
            }
            Console.WriteLine();
        }

        private void UpdateNonlinearElements()
        {
            foreach (var element in model.NonlinearElements)
            {
                element.UpdateNonlinearModel(context);
            }
        }
    }
}