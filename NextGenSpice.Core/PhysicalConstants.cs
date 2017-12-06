using System;

namespace NextGenSpice.Core
{
    public static class PhysicalConstants
    {
        /// <summary>
        /// Relates average kinetic energy of particles in a gas with the temperature of the gas. k = R/N_A
        /// </summary>
        public static double Boltzmann { get; } = 1.3806485279e-23;

        /// <summary>
        /// Charge of the electron in coulombs.
        /// </summary>
        public static double ElementaryCharge { get; } = 1.602176620898e-19;

        public static double CelsiusToKelvin(double celsius)
        {
            return celsius + 273.15;
        }
    }
}