namespace NextGenSpice.Core.Elements.Parameters
{
    /// <summary>
    ///     Represents set of model parameters for the diode device.
    /// </summary>
    public class DiodeModelParams
    {
        /// <summary>
        ///     Default set of model parameters for the diode.
        /// </summary>
        public static DiodeModelParams Default => new DiodeModelParams();

        /// <summary>
        ///     Set of parameters corresponding to D1N418 diode model.
        /// </summary>
        public static DiodeModelParams D1N4148 => new DiodeModelParams
        {
            SaturationCurrent = 2.52e-9,
            EmissionCoefficient = 1.752,
            TransitTime = 2e-8,
            JunctionCapacitance = 9e-13,
            JunctionGradingCoefficient = 0.25,
            JunctionPotential = 20,
            ReverseBreakdownVoltage = 75,
            SeriesResistance = 0.568,
        };
        
        /// <summary>
        ///     Saturation current (from diode equation) in amperes.
        /// </summary>
        public double SaturationCurrent { get; set; } = 1e-14;

        /// <summary>
        ///     Series (parasitic) resistance in ohms.
        /// </summary>
        public double SeriesResistance { get; set; }

        /// <summary>
        ///     Emission coefficient of the diode, also called Ideality coefficient.
        /// </summary>
        public double EmissionCoefficient { get; set; } = 1;


        public double TransitTime { get; set; }

        /// <summary>
        ///     Zero-bias junction capacitance of the diode in farads.
        /// </summary>
        public double JunctionCapacitance { get; set; }

        /// <summary>
        ///     Potential of the diode junction in volts.
        /// </summary>
        public double JunctionPotential { get; set; } = 1;

        /// <summary>
        ///     Junction grading coefficient.
        /// </summary>
        public double JunctionGradingCoefficient { get; set; } = 0.5;

        /// <summary>
        ///     Also called Energy gap.
        /// </summary>
        public double ActivationEnergy { get; set; } = 1.11;

        /// <summary>
        ///     Exponent for temperature influence on saturation current.
        /// </summary>
        public double SaturationCurrentTemperatureExponent { get; set; } = 3;

        /// <summary>
        ///     Flicker-noise coefficient.
        /// </summary>
        public double FlickerNoiseCoefficient { get; set; }

        /// <summary>
        ///     Flicker-noise exponent.
        /// </summary>
        public double FlickerNoiseExponent { get; set; } = 1;

        /// <summary>
        ///     Forward bias depletion capacitance coefficient.
        /// </summary>
        public double ForwardBiasDepletionCapacitanceCoefficient { get; set; } = 0.5;

        /// <summary>
        ///     Absolute value of the voltage, on which reverse breakdown of the diode occurs.
        /// </summary>
        public double ReverseBreakdownVoltage { get; set; } = double.PositiveInfinity;

        /// <summary>
        ///     Absolute value of current flowing through the diode during reverse breakdown.
        /// </summary>
        public double ReverseBreakdownCurrent { get; set; } = 1e-3;

        /// <summary>
        ///     Nominal temperature in °C for simulation and at which all parameters are assumed to have been measured.
        /// </summary>
        public double NominalTemperature { get; set; } = 27;

        /// <summary>
        ///     Optional parameter - convergence aid for the simulation, if not set, global (circuits) GMIN parameter will be used.
        /// </summary>
        public double? MinimalResistance { get; set; }
    }
}