using System;
using NextGenSpice.LargeSignal.NumIntegration;

namespace NextGenSpice.LargeSignal
{
    /// <summary>Class for aggregating device independent parameters for the simulation.</summary>
    public class SimulationParameters
    {
        private IIntegrationMethodFactory integrationMethodFactory =
//            new SimpleIntegrationMethodFactory(() => new GearIntegrationMethod(2));
//            new SimpleIntegrationMethodFactory(() => new BackwardEulerIntegrationMethod());
            new SimpleIntegrationMethodFactory(() => new TrapezoidalIntegrationMethod());

        /// <summary>Convergence aid for some devices.</summary>
        public double MinimalResistance { get; set; } = 1e-12;

        /// <summary>Relative tolerance for Newton-Raphson iterations convergence check.</summary>
        public double RelativeTolerance { get; set; } = 1e-3;

        /// <summary>Absolute tolerance for Newton-Raphson iterations convergence check.</summary>
        public double AbsolutTolerane { get; set; } = 1e-11;

        /// <summary>Factory for preffered integration method for circuit devices.</summary>
        public IIntegrationMethodFactory IntegrationMethodFactory
        {
            get => integrationMethodFactory;
            set => integrationMethodFactory = value ?? throw new ArgumentNullException();
        }
    }
}