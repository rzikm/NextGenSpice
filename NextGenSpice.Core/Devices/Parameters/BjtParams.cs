using System;

namespace NextGenSpice.Core.Devices.Parameters
{
    /// <summary>Represents set of parameters for Homo-Junction Bipolar Transistor.</summary>
    public class BjtParams : ICloneable
    {
        /// <summary>Default parameter set for BJT transistor.</summary>
        public static BjtParams Default => new BjtParams();

        /// <summary>Saturation current in ampers.</summary>
        public double SaturationCurrent { get; set; } = 1e-16;

        /// <summary>Forward current emission coefficient.</summary>
        public double ForwardEmissionCoefficient { get; set; } = 1;

        /// <summary>Reverse current emission coefficient.</summary>
        public double ReverseEmissionCoefficient { get; set; } = 1;

        /// <summary>Corner for forward beta high current roll-off in ampers.</summary>
        public double ForwardCurrentCorner { get; set; } = double.PositiveInfinity;

        /// <summary>Corner for reverse beta high current roll-off in ampers.</summary>
        public double ReverseCurrentCorner { get; set; } = double.PositiveInfinity;

        /// <summary>Forward early voltage in Volts.</summary>
        public double ForwardEarlyVoltage { get; set; } = double.PositiveInfinity;

        /// <summary>Reverse early voltage in volts.</summary>
        public double ReverseEarlyVoltage { get; set; } = double.PositiveInfinity;

        /// <summary>Base-emitter leakage saturation current in ampers.</summary>
        public double EmitterSaturationCurrent { get; set; } = 0;

        /// <summary>Base-emitter leakage emission coefficient.</summary>
        public double EmitterSaturationCoefficient { get; set; } = 1.5;

        /// <summary>Base-collector leakage saturation current in ampers.</summary>
        public double CollectorSaturationCurrent { get; set; } = 0;

        /// <summary>Base-collector leakage emission coefficient.</summary>
        public double CollectorSaturationCoefficient { get; set; } = 2;

        /// <summary>Forward beta.</summary>
        public double ForwardBeta { get; set; } = 100;

        /// <summary>Reverse beta.</summary>
        public double ReverseBeta { get; set; } = 1;

        /// <summary>Minimum base resistance for high currents in ohms.</summary>
        public double MinimumBaseResistance { get; set; } = 0;

        /// <summary>Current for base resistance midpoint.</summary>
        public double CurrentBaseResistanceMidpoint { get; set; } = double.PositiveInfinity;

        /// <summary>Collector ohmic resistance.</summary>
        public double CollectorResistance { get; set; } = 0;

        /// <summary>Emitter ohmic resistance.</summary>
        public double EmitterResistance { get; set; } = 0;

        /// <summary>Zero-bias base resistance in ohms (may be high-current dependent).</summary>
        public double BaseResistance { get; set; } = 0;

        /// <summary>Base-emitter zero-bias depletion capacitance in farads.</summary>
        public double EmitterCapacitance { get; set; } = 0;

        /// <summary>Base-emitter junction built-in potential in volts.</summary>
        public double EmitterPotential { get; set; } = 0.75;

        /// <summary>Base-emitter junction exponential factor.</summary>
        public double EmitterExponentialFactor { get; set; } = 0.33;

        /// <summary>Base-collector zero-bias depletion capacitance in farads.</summary>
        public double CollectorCapacitance { get; set; } = 0;

        /// <summary>Base-collector junction built-in potential in volts.</summary>
        public double CollectorPotential { get; set; } = 0.75;

        /// <summary>Base-collector junction exponential factor.</summary>
        public double CollectorExponentialFactor { get; set; } = 0.33;

        /// <summary>Ideal forward transit time in seconds;</summary>
        public double ForwardTransitTime { get; set; } = 0;

        /// <summary>Voltage describing VBC dependence of transit time in volts.</summary>
        public double VbcDependenceOfTransitTime { get; set; } = double.PositiveInfinity;

        /// <summary>High-current parameter for effect on forward transit time in ampers.</summary>
        public double ForwardTransitHighCurrent { get; set; }

        /// <summary>Ideal reverse transit time in seconds.</summary>
        public double ReverseTransitTime { get; set; } = 0;

        /// <summary>Base-substrate zero-bias depletion capacitance in farrads.</summary>
        public double SubstrateCapacitance { get; set; } = 0;

        /// <summary>Base-substrate junction exponential factor.</summary>
        public double SubstrateExponentialFactor { get; set; } = 0;

        /// <summary>Forward and reverse beta temperature exponent.</summary>
        public double TemperatureExponentBeta { get; set; } = 0;

        /// <summary>Energy gap for temperature effect on saturation current in eV.</summary>
        public double EnergyGap { get; set; } = 1.11;

        /// <summary>Temperature exponent for effect on saturation current.</summary>
        public double TemperatureExponentSaturationCurrent { get; set; } = 3;

        /// <summary>Flicker-noise coefficient.</summary>
        public double FlickerNoiseCoeffitient { get; set; } = 0;

        /// <summary>Flicker-noise exponent.</summary>
        public double FlickerNoiseExponent { get; set; } = 1;

        /// <summary>Coefficient for forward-bias depletion capacitance formula.</summary>
        public double ForwardBiasDepletionCoefficient { get; set; } = 0.5;

        /// <summary>Parameter measurement temperature in degrees celsius.</summary>
        public double NominalTemperature { get; set; } = 26.85;

        /// <summary>True if this is a PNP BJT model, otherwise this is a NPN model.</summary>
        public bool IsPnp { get; set; }

        /// <summary>Creates a new object that is a copy of the current instance.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}