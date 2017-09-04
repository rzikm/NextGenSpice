using System;
using System.Linq;

namespace NextGenSpice.Circuit
{
    public class CircuitSimulator
    {
        private bool dcBiasEstablished;
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
                for (int i = 0; i < 15; i++)
                {
                    Iterate();
                }
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