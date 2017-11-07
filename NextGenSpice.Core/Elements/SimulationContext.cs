namespace NextGenSpice.Core.Elements
{
    public class SimulationContext
    {
        public double Time { get; set; }
        public double Timestep { get; set; }
        public double[] EquationSolution { get; set; }
    }
}