namespace NextGenSpice.Core.Elements
{
    public class DiodeModelParams
    {
        public static DiodeModelParams Default => new DiodeModelParams();

        public static DiodeModelParams D1N4148 => new DiodeModelParams()
        {
            SaturationCurrent = 4.352E-9,
            EmissionCoefficient = 1.906,
            TransitTime = 5e-9,
            JunctionCapacitance = 7.048E-13,
            JunctionGradingCoefficient = 0.285,
            JunctionPotential = 0.869,
            ReverseBreakdownVoltage = 110,
            ReverseBreakdownCurrent = 0.0001,
            SeriesResistance = 0.6458,

        };

//        public static DiodeModelParams D1N4148 => new DiodeModelParams()
//        {
//            SaturationCurrent = 35e-12,
//            EmissionCoefficient = 1.24,
//            TransitTime = 5e-9,
//            JunctionCapacitance = 4e-12,
//            JunctionGradingCoefficient = 0.285,
//            JunctionPotential = 0.6,
//            ReverseBreakdownVoltage = 75
//        };

        private DiodeModelParams()
        {

        }

        /// <summary>
        /// Saturation current (from diode equation) in amperes.
        /// </summary>
        public double SaturationCurrent { get; set; } = 1e-14;

        /// <summary>
        /// Series (parasitic) resistance in ohms.
        /// </summary>
        public double SeriesResistance { get; set; }
        
        /// <summary>
        /// Emission coefficient of the diode, also called Ideality coefficient.
        /// </summary>
        public double EmissionCoefficient { get; set; } = 1;


        public double TransitTime { get; set; }

        /// <summary>
        /// Zero-bias junction capacitance of the diode in farads.
        /// </summary>
        public double JunctionCapacitance { get; set; }

        /// <summary>
        /// Potential of the diode junction in volts.
        /// </summary>
        public double JunctionPotential { get; set; } = 1;


        public double JunctionGradingCoefficient { get; set; } = 0.5;

        /// <summary>
        /// Also called Energy gap.
        /// </summary>
        public double ActivationEnergy { get; set; } = 1.11;


        public double SaturationCurrentTemperatureExponent { get; set; } = 3;
        public double FlickerNoiseCoefficient { get; set; }
        public double FlickerNoiseExponent { get; set; } = 1;


        public double ForwardBiasDepletionCapacitanceCoefficient { get; set; } = 0.5;

        /// <summary>
        /// Absolute value of the voltage, on which reverse breakdown of the diode occurs.
        /// </summary>
        public double ReverseBreakdownVoltage { get; set; } = double.PositiveInfinity;

        /// <summary>
        /// Absolute value of current flowing through the diode during reverse breakdown.
        /// </summary>
        public double ReverseBreakdownCurrent { get; set; } = 1e-3;

        /// <summary>
        /// Nominal temperature in °C for simulation and at which all parameters are assumed to have been measured.
        /// </summary>
        public double Temperature { get; set; } = 27;

        /// <summary>
        /// Optional parameter - convergence aid for the simulation, if not set, global (circuits) GMIN parameter will be used.
        /// </summary>
        public double? MinimalResistance { get; set; }


        //TODO: consider moving this thing elsewhere
        /// <summary>
        /// Initial guess of the voltage across the diode.
        /// </summary>
        public double Vd { get; set; } = 0.9;
    }
}