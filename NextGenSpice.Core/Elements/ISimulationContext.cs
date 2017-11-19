namespace NextGenSpice.Core.Elements
{
    public interface ISimulationContext
    {
        double NodeCount { get; }
        double Time { get; set; }
        double Timestep { get; set; }
        double GetSolutionForVariable(int index);
    }
}