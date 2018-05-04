using NextGenSpice.LargeSignal.Devices;

namespace NextGenSpice.LargeSignal
{
    /// <summary>Defines properties for getting information about the simulation.</summary>
    public interface ISimulationContext
    {
        /// <summary>Curent timepoint of the simulation.</summary>
        double TimePoint { get; }

        /// <summary>Last timestep that was used to advance the timepoint.</summary>
        double TimeStep { get; }

        /// <summary>General parameters of the circuit that is simulated.</summary>
        SimulationParameters SimulationParameters { get; }

        /// <summary>Specifies whether the Newton-Raphson iterations converged.</summary>
        bool Converged { get; }

        /// <summary>
        ///     Reports that Newton-Raphson iterations for this device did not converge yet and another iteration should be
        ///     made.
        /// </summary>
        /// <param name="device"></param>
        void ReportNotConverged(ILargeSignalDevice device);
    }
}