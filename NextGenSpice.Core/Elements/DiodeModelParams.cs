namespace NextGenSpice.Core.Elements
{
    public class DiodeModelParams
    {
        public static DiodeModelParams Default => new DiodeModelParams();

        private DiodeModelParams()
        {

        }

        /// <summary>
        /// Saturation current (from diode equation) in amperes.
        /// </summary>
        public double SaturationCurrent { get; set; } = 1e-15;
        public double ThermalVoltage { get; set; } = 0.025875;

        /// <summary>
        /// Series (parasitic) resistance in ohms.
        /// </summary>
        public double SeriesResistance { get; set; }
        
        public double EmissionCoefficient { get; set; } = 1;
        public double TransitTime { get; set; }
        public double JunctionCapacitance { get; set; }
        public double JunctionPotential { get; set; } = 1;
        public double JunctionGradingCoefficient { get; set; } = 0.5;
        public double ActivationEnergy { get; set; } = 1.11;
        public double SaturationCurrentTemperatureExponent { get; set; } = 3;
        public double FlickerNoiseCoefficient { get; set; }
        public double FlickerNoiseExponent { get; set; } = 1;
        public double ForwardBiasDepletionCapacitanceCoefficient { get; set; } = 0.5;
        public double ReverseBreakdownVoltage { get; set; } = double.PositiveInfinity;
        public double ReverseBreakdownCurrent { get; set; } = 1e-3;


        //TODO: consider moving this thing elsewhere
        public double Vd { get; set; } = 0.9;
    }
}