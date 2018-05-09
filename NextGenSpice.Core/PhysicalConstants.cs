namespace NextGenSpice.Core
{
    /// <summary>Helper class for aggregating physics-related constants.</summary>
    public static class PhysicalConstants
    {
        /// <summary>Relates average kinetic energy of particles in a gas with the temperature of the gas. k = R/N_A</summary>
        public static double Boltzmann { get; } = 1.3806485279e-23;

        /// <summary>Charge of the electron in coulombs.</summary>
        public static double DevicearyCharge { get; } = 1.602176620898e-19;

        /// <summary>Converts temperature from degrees Celsius to Kelvins.</summary>
        /// <param name="celsius"></param>
        /// <returns></returns>
        public static double CelsiusToKelvin(double celsius)
        {
            return celsius + 273.15;
        }
    }
}