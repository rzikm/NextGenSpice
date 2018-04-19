using NextGenSpice.LargeSignal.NumIntegration;

namespace NextGenSpice.LargeSignal
{
    /// <summary>Class for aggregating device independent parameters for the simulation.</summary>
    public class CircuitParameters
    {
        /// <summary>Convergence aid for some devices.</summary>
        public double MinimalResistance { get; set; } = 1e-12;

        /// <summary>Factory for preffered integration method for circuit devices.</summary>
        public IIntegrationMethodFactory IntegrationMethodFactory { get; set; } =
            new SimpleIntegrationMethodFactory(() => new GearIntegrationMethod(2));
    }
}