namespace NextGenSpice.Core.Elements
{
    public interface ISimulationContext
    {
        double NodeCount { get; }
        double Time { get; }
        double Timestep { get; }
        double GetSolutionForVariable(int index);
    }
}