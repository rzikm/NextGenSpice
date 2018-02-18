using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.Circuit
{
    /// <summary>
    ///     Defines properties for getting information about the simulation.
    /// </summary>
    public interface ISimulationContext
    {
        /// <summary>
        ///     Number of inner nodes.
        /// </summary>
        double NodeCount { get; }

        /// <summary>
        ///     Curent timepoint of the simulation.
        /// </summary>
        double TimePoint { get; }

        /// <summary>
        ///     Last timestep that was used to advance the timepoint.
        /// </summary>
        double TimeStep { get; }

        /// <summary>
        ///     General parameters of the circuit that is simulated.
        /// </summary>
        CircuitParameters CircuitParameters { get; }

        /// <summary>
        ///     Gets numerical solution for vairable with given index - either a node voltage or branch current.
        /// </summary>
        /// <param name="index">Index of the variable.</param>
        /// <returns></returns>
        double GetSolutionForVariable(int index);
    }
}