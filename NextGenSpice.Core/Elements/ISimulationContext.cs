namespace NextGenSpice.Core.Elements
{
    public interface ISimulationContext
    {
        double NodeCount { get; }
        double Time { get; }
        double TimeStep { get; }
        double GetSolutionForVariable(int index);
        CircuitParameters CircuitParameters { get; }

    }

    public class CircuitParameters
    {
        /// <summary>
        /// Convergence aid for some devices.
        /// </summary>
        public double MinimalResistance { get; set; } = 1e-12;
    }
}