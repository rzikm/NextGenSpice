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
}