using System;
using System.Linq;

namespace NextGenSpice.Circuit
{
    public class CircuitSimulator
    {
        private bool dcBiasEstablished;

        private double epsilon = 1e-10;
        private int maxDcPointIterations = 100;

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
        }

        public void BuildEquationSystem()
        {
            var b = new EquationSystemBuilder();
            for (int i = 0; i < CircuitDefinition.Nodes.Count; i++)
            {
                b.AddVariable();
            }
            foreach (var circuitElement in model.Elements)
            {
                circuitElement.ApplyToEquationsPermanent(b, context);
            }

            equationSystem = b.Build();
        }

        public void EstablishDcBias()
        {
            model = CircuitDefinition.GetDcOperatingPointAnalysisModel();

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
                if (--maxDcPointIterations < 0) break;

                delta = 0;
                var prevVoltages = (double[]) equationSystem.Solution.Clone();

                Iterate();

                for (int i = 0; i < prevVoltages.Length; i++)
                {
                    var d = prevVoltages[i] - equationSystem.Solution[i];
                    delta += d * d;
                }
            } while (delta > epsilon * epsilon);
        }

        private void Iterate()
        {
            UpdateEquationSystem();
            UpdateNodeValues();
            UpdateNonlinearElements();
            DebugPrint();
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
                e.ApplyToEquationsDynamic(equationSystem, context);
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
                element.UpdateLinearizedModel(context);
            }
        }
    }
}