using System;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.Extensions
{
    public static class CircuitBuilderExtensions
    {
        public static CircuitBuilder AddResistor(this CircuitBuilder builder, int n1, int n2, double resistance, string name = null)
        {
            builder.AddElement(new[]{n1, n2}, new ResistorElement(resistance, name));
            return builder;
        }

        public static CircuitBuilder AddInductor(this CircuitBuilder builder, int n1, int n2, double inductance, double? initialCurrent = null, string name = null)
        {
            builder.AddElement(new[]{n1, n2}, new InductorElement(inductance, initialCurrent, name));
            return builder;
        }
        public static CircuitBuilder AddCapacitor(this CircuitBuilder builder, int n1, int n2, double capacitance, double? initialVoltage = null, string name = null)
        {
            builder.AddElement(new[] { n1, n2 }, new CapacitorElement(capacitance, initialVoltage, name));
            return builder;
        }
        public static CircuitBuilder AddCurrentSource(this CircuitBuilder builder, int n1, int n2, double current, string name = null)
        {
            builder.AddElement(new[] { n1, n2 }, new CurrentSourceElement(current, name));
            return builder;
        }

        public static CircuitBuilder AddCurrentSource(this CircuitBuilder builder, int n1, int n2, SourceBehaviorParams param, string name = null)
        {
            builder.AddElement(new[] { n1, n2 }, new CurrentSourceElement(param, name));
            return builder;
        }

        public static CircuitBuilder AddVoltageSource(this CircuitBuilder builder, int n1, int n2, double voltage, string name = null)
        {
            builder.AddElement(new[] { n1, n2 }, new VoltageSourceElement(voltage, name));
            return builder;
        }

        public static CircuitBuilder AddVoltageSource(this CircuitBuilder builder, int n1, int n2, SourceBehaviorParams param, string name = null)
        {
            builder.AddElement(new[] { n1, n2 }, new VoltageSourceElement(param, name));
            return builder;
        }

        public static CircuitBuilder AddDiode(this CircuitBuilder builder, int n1, int n2, DiodeModelParams param, string name = null)
        {
            builder.AddElement(new[] { n1, n2 }, new DiodeElement(param, name));
            return builder;
        }

        public static CircuitBuilder AddDiode(this CircuitBuilder builder, int n1, int n2, Action<DiodeModelParams> config, string name = null)
        {
            var param = DiodeModelParams.Default;
            config(param);
            builder.AddElement(new[] { n1, n2 }, new DiodeElement(param, name));
            return builder;
        }
    }
}