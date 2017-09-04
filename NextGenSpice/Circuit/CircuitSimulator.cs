using System;
using System.Linq;

namespace NextGenSpice.Circuit
{
    public class CircuitSimulator
    {
        private bool dcBiasEstablished;

        private double epsilon = 1e-10;
        private int maxDcPointIterations = 100;

        public CircuitSimulator(ElectricCircuit circuit)
        {
            this.equationSystem = new CircuitEquationSystem(circuit.Nodes.Count);
            this.circuit = circuit;
        }

        private ICircuitEquationSystem equationSystem;
        private ElectricCircuit circuit;
        

        public void EstablishDcBias()
        {
            Iterate();

            if (!circuit.IsLinear)
            {
                double delta;
                do
                {
                    if (--maxDcPointIterations < 0) break;

                    delta = 0;
                    var prevVoltages = (double[]) equationSystem.NodeVoltages.Clone();

                    Iterate();

                    for (int i = 0; i < prevVoltages.Length; i++)
                    {
                        var d = prevVoltages[i] - equationSystem.NodeVoltages[i];
                        delta += d * d;
                    }
                } while (delta > epsilon * epsilon);
            }
        }

        private void Iterate()
        {
            equationSystem.Clear();

            foreach (var e in circuit.Elements)
            {
                e.ApplyToEquations(equationSystem);
            }

            equationSystem.Solve();

            for (var i = 0; i < circuit.Nodes.Count; i++)
            {
                circuit.Nodes[i].Voltage = equationSystem.NodeVoltages[i];
            }

            Console.WriteLine("Results:");
            for (var i = 0; i < equationSystem.NodeVoltages.Length; i++)
            {
                var v = equationSystem.NodeVoltages[i];
                Console.WriteLine($"node {i}: {v:##.0000}");
            }
            Console.WriteLine();

            foreach (var element in circuit.NonlinearCircuitElements)
            {
                element.UpdateLinearizedModel();
            }
        }
    }
}