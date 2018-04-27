using System;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Devices.Parameters;

namespace NextGenSpice.Core.Extensions
{
    /// <summary>Helper class for adding simple devices using the circuit builder.</summary>
    public static class CircuitBuilderExtensions
    {
        /// <summary>Adds a resistor between specified nodes.</summary>
        /// <param name="builder">The builder.</param>
        /// <param name="n1">Positive node of the device.</param>
        /// <param name="n2">Negative node of the device.</param>
        /// <param name="resistance">Resistance of the resistor in ohms.</param>
        /// <param name="name">Name of the device.</param>
        /// <returns></returns>
        public static CircuitBuilder AddResistor(this CircuitBuilder builder, int n1, int n2, double resistance,
            object tag = null)
        {
            builder.AddDevice(new[] {n1, n2}, new ResistorDevice(resistance, tag));
            return builder;
        }

        /// <summary>Adds an inductor between specified nodes.</summary>
        /// <param name="builder">The builder.</param>
        /// <param name="n1">Positive node of the device.</param>
        /// <param name="n2">Negative node of the device.</param>
        /// <param name="inductance">Inductance of the inductor in henry.</param>
        /// <param name="initialCurrent">Initial current across the inductor.</param>
        /// <param name="name">Name of the device.</param>
        /// <returns></returns>
        public static CircuitBuilder AddInductor(this CircuitBuilder builder, int n1, int n2, double inductance,
            double? initialCurrent = null, object tag = null)
        {
            builder.AddDevice(new[] {n1, n2}, new InductorDevice(inductance, initialCurrent, tag));
            return builder;
        }

        /// <summary>Adds a capacitor between specified nodes.</summary>
        /// <param name="builder">The builder.</param>
        /// <param name="n1">Positive node of the device.</param>
        /// <param name="n2">Negative node of the device.</param>
        /// <param name="capacitance">Capacitance of the capacitor in farads.</param>
        /// <param name="initialVoltage">Initial voltage across the capacitor.</param>
        /// <param name="name">Name of the device.</param>
        /// <returns></returns>
        public static CircuitBuilder AddCapacitor(this CircuitBuilder builder, int n1, int n2, double capacitance,
            double? initialVoltage = null, object tag = null)
        {
            builder.AddDevice(new[] {n1, n2}, new CapacitorDevice(capacitance, initialVoltage, tag));
            return builder;
        }

        /// <summary>Adds a constant current source between specified nodes.</summary>
        /// <param name="builder">The builder.</param>
        /// <param name="n1">Positive node of the device.</param>
        /// <param name="n2">Negative node of the device.</param>
        /// <param name="current">Current of the source in ampers.</param>
        /// <param name="name">Name of the device.</param>
        /// <returns></returns>
        public static CircuitBuilder AddCurrentSource(this CircuitBuilder builder, int n1, int n2, double current,
            object tag = null)
        {
            builder.AddDevice(new[] {n1, n2}, new CurrentSourceDevice(current, tag));
            return builder;
        }

        /// <summary>Adds a behaviored current source between specified nodes.</summary>
        /// <param name="builder">The builder.</param>
        /// <param name="n1">Positive node of the device.</param>
        /// <param name="n2">Negative node of the device.</param>
        /// <param name="param">Parameters of the desired behavior.</param>
        /// <param name="name">Name of the device.</param>
        /// <returns></returns>
        public static CircuitBuilder AddCurrentSource(this CircuitBuilder builder, int n1, int n2,
            SourceBehaviorParams param, object tag = null)
        {
            builder.AddDevice(new[] {n1, n2}, new CurrentSourceDevice(param, tag));
            return builder;
        }

        /// <summary>Adds a constant voltage source between specified nodes.</summary>
        /// <param name="builder">The builder.</param>
        /// <param name="n1">Positive node of the device.</param>
        /// <param name="n2">Negative node of the device.</param>
        /// <param name="voltage">Voltage of the source in volts.</param>
        /// <param name="name">Name of the device.</param>
        /// <returns></returns>
        public static CircuitBuilder AddVoltageSource(this CircuitBuilder builder, int n1, int n2, double voltage,
            object tag = null)
        {
            builder.AddDevice(new[] {n1, n2}, new VoltageSourceDevice(voltage, tag));
            return builder;
        }

        /// <summary>Adds a behaviored voltage source between specified nodes.</summary>
        /// <param name="builder">The builder.</param>
        /// <param name="n1">Positive node of the device.</param>
        /// <param name="n2">Negative node of the device.</param>
        /// <param name="param">Parameters of the desired behavior.</param>
        /// <param name="name">Name of the device.</param>
        /// <returns></returns>
        public static CircuitBuilder AddVoltageSource(this CircuitBuilder builder, int n1, int n2,
            SourceBehaviorParams param, object tag = null)
        {
            builder.AddDevice(new[] {n1, n2}, new VoltageSourceDevice(param, tag));
            return builder;
        }


        /// <summary>Adds a resistor between specified nodes.</summary>
        /// <param name="builder">The builder.</param>
        /// <param name="nodes">Array of connections of the subcircuit.</param>
        /// <param name="subcircuit">Resistance of the resistor in ohms.</param>
        /// <param name="name">Name of the device.</param>
        /// <returns></returns>
        public static CircuitBuilder AddSubcircuit(this CircuitBuilder builder, int[] nodes, ISubcircuitDefinition subcircuit,
            object tag = null)
        {
            builder.AddDevice(nodes, new SubcircuitDevice(subcircuit, tag));
            return builder;
        }

        /// <summary>Adds a diode between specified nodes.</summary>
        /// <param name="builder">The builder.</param>
        /// <param name="n1">Positive node of the device.</param>
        /// <param name="n2">Negative node of the device.</param>
        /// <param name="param">Model parameters of the device.</param>
        /// <param name="name">Name of the device.</param>
        /// <returns></returns>
        public static CircuitBuilder AddDiode(this CircuitBuilder builder, int n1, int n2, DiodeParams param,
            object tag = null)
        {
            builder.AddDevice(new[] {n1, n2}, new DiodeDevice(param, tag));
            return builder;
        }

        /// <summary>Adds a diode between specified nodes.</summary>
        /// <param name="builder">The builder.</param>
        /// <param name="n1">Positive node of the device.</param>
        /// <param name="n2">Negative node of the device.</param>
        /// <param name="config">Function for configuring device model parameters.</param>
        /// <param name="name">Name of the device.</param>
        /// <returns></returns>
        public static CircuitBuilder AddDiode(this CircuitBuilder builder, int n1, int n2,
            Action<DiodeParams> config, object tag = null)
        {
            var param = DiodeParams.Default;
            config(param);
            builder.AddDevice(new[] {n1, n2}, new DiodeDevice(param, tag));
            return builder;
        }
    }
}