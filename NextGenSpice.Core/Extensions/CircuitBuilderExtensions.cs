using System;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.Extensions
{
    public static class CircuitBuilderExtensions
    {
        public static CircuitBuilder AddResistor(this CircuitBuilder builder, int n1, int n2, double resistance)
        {
            builder.AddElement(new[]{n1, n2}, new ResistorElement(resistance));
            return builder;
        }

        public static CircuitBuilder AddInductor(this CircuitBuilder builder, int n1, int n2, double inductance, double initialCurrent = 0)
        {
            builder.AddElement(new[]{n1, n2}, new InductorElement(inductance, initialCurrent));
            return builder;
        }
        public static CircuitBuilder AddCapacitor(this CircuitBuilder builder, int n1, int n2, double capacitance, double initialVoltage = 0)
        {
            builder.AddElement(new[] { n1, n2 }, new CapacitorElement(capacitance, initialVoltage));
            return builder;
        }
        public static CircuitBuilder AddCurrentSource(this CircuitBuilder builder, int n1, int n2, double current)
        {
            builder.AddElement(new[] { n1, n2 }, new CurrentSourceElement(current));
            return builder;
        }
        public static CircuitBuilder AddVoltageSource(this CircuitBuilder builder, int n1, int n2, double voltage)
        {
            builder.AddElement(new[] { n1, n2 }, new VoltageSourceElement(voltage));
            return builder;
        }

        public static CircuitBuilder AddDiode(this CircuitBuilder builder, int n1, int n2, DiodeModelParams param)
        {
            builder.AddElement(new[] { n1, n2 }, new DiodeElement(param));
            return builder;
        }

        public static CircuitBuilder AddDiode(this CircuitBuilder builder, int n1, int n2, Action<DiodeModelParams> config)
        {
            var param = DiodeModelParams.Default;
            config(param);
            builder.AddElement(new[] { n1, n2 }, new DiodeElement(param));
            return builder;
        }
    }
}