using System;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Elements.Parameters;

namespace NextGenSpice.Core.Extensions
{
    /// <summary>
    ///     Helper class for adding simple devices using the circuit builder.
    /// </summary>
    public static class CircuitBuilderExtensions
    {
        /// <summary>
        ///     Adds a resistor between specified nodes.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="n1">Positive node of the device.</param>
        /// <param name="n2">Negative node of the device.</param>
        /// <param name="resistance">Resistance of the resistor in ohms.</param>
        /// <param name="name">Name of the device.</param>
        /// <returns></returns>
        public static CircuitBuilder AddResistor(this CircuitBuilder builder, int n1, int n2, double resistance,
            string name = null)
        {
            builder.AddElement(new[] {n1, n2}, new ResistorElement(resistance, name));
            return builder;
        }

        /// <summary>
        ///     Adds an inductor between specified nodes.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="n1">Positive node of the device.</param>
        /// <param name="n2">Negative node of the device.</param>
        /// <param name="inductance">Inductance of the inductor in henry.</param>
        /// <param name="initialCurrent">Initial current across the inductor.</param>
        /// <param name="name">Name of the device.</param>
        /// <returns></returns>
        public static CircuitBuilder AddInductor(this CircuitBuilder builder, int n1, int n2, double inductance,
            double? initialCurrent = null, string name = null)
        {
            builder.AddElement(new[] {n1, n2}, new InductorElement(inductance, initialCurrent, name));
            return builder;
        }

        /// <summary>
        ///     Adds a capacitor between specified nodes.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="n1">Positive node of the device.</param>
        /// <param name="n2">Negative node of the device.</param>
        /// <param name="capacitance">Capacitance of the capacitor in farads.</param>
        /// <param name="initialVoltage">Initial voltage across the capacitor.</param>
        /// <param name="name">Name of the device.</param>
        /// <returns></returns>
        public static CircuitBuilder AddCapacitor(this CircuitBuilder builder, int n1, int n2, double capacitance,
            double? initialVoltage = null, string name = null)
        {
            builder.AddElement(new[] {n1, n2}, new CapacitorElement(capacitance, initialVoltage, name));
            return builder;
        }

        /// <summary>
        ///     Adds a constant current source between specified nodes.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="n1">Positive node of the device.</param>
        /// <param name="n2">Negative node of the device.</param>
        /// <param name="current">Current of the source in ampers.</param>
        /// <param name="name">Name of the device.</param>
        /// <returns></returns>
        public static CircuitBuilder AddCurrentSource(this CircuitBuilder builder, int n1, int n2, double current,
            string name = null)
        {
            builder.AddElement(new[] {n1, n2}, new CurrentSourceElement(current, name));
            return builder;
        }

        /// <summary>
        ///     Adds a behaviored current source between specified nodes.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="n1">Positive node of the device.</param>
        /// <param name="n2">Negative node of the device.</param>
        /// <param name="param">Parameters of the desired behavior.</param>
        /// <param name="name">Name of the device.</param>
        /// <returns></returns>
        public static CircuitBuilder AddCurrentSource(this CircuitBuilder builder, int n1, int n2,
            SourceBehaviorParams param, string name = null)
        {
            builder.AddElement(new[] {n1, n2}, new CurrentSourceElement(param, name));
            return builder;
        }

        /// <summary>
        ///     Adds a constant voltage source between specified nodes.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="n1">Positive node of the device.</param>
        /// <param name="n2">Negative node of the device.</param>
        /// <param name="voltage">Voltage of the source in volts.</param>
        /// <param name="name">Name of the device.</param>
        /// <returns></returns>
        public static CircuitBuilder AddVoltageSource(this CircuitBuilder builder, int n1, int n2, double voltage,
            string name = null)
        {
            builder.AddElement(new[] {n1, n2}, new VoltageSourceElement(voltage, name));
            return builder;
        }

        /// <summary>
        ///     Adds a behaviored voltage source between specified nodes.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="n1">Positive node of the device.</param>
        /// <param name="n2">Negative node of the device.</param>
        /// <param name="param">Parameters of the desired behavior.</param>
        /// <param name="name">Name of the device.</param>
        /// <returns></returns>
        public static CircuitBuilder AddVoltageSource(this CircuitBuilder builder, int n1, int n2,
            SourceBehaviorParams param, string name = null)
        {
            builder.AddElement(new[] {n1, n2}, new VoltageSourceElement(param, name));
            return builder;
        }

        /// <summary>
        ///     Adds a diode between specified nodes.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="n1">Positive node of the device.</param>
        /// <param name="n2">Negative node of the device.</param>
        /// <param name="param">Model parameters of the device.</param>
        /// <param name="name">Name of the device.</param>
        /// <returns></returns>
        public static CircuitBuilder AddDiode(this CircuitBuilder builder, int n1, int n2, DiodeModelParams param,
            string name = null)
        {
            builder.AddElement(new[] {n1, n2}, new DiodeElement(param, name));
            return builder;
        }

        /// <summary>
        ///     Adds a diode between specified nodes.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="n1">Positive node of the device.</param>
        /// <param name="n2">Negative node of the device.</param>
        /// <param name="config">Function for configuring device model parameters.</param>
        /// <param name="name">Name of the device.</param>
        /// <returns></returns>
        public static CircuitBuilder AddDiode(this CircuitBuilder builder, int n1, int n2,
            Action<DiodeModelParams> config, string name = null)
        {
            var param = DiodeModelParams.Default;
            config(param);
            builder.AddElement(new[] {n1, n2}, new DiodeElement(param, name));
            return builder;
        }
    }
}