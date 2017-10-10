using System;
using NextGenSpice.Circuit;
using NextGenSpice.Elements;
using NextGenSpice.Models;

namespace NextGenSpice.Extensions
{
    public static class CircuitBuilderExtensions
    {
        public static CircuitBuilder AddResistor(this CircuitBuilder builder, double resistance, int n1, int n2)
        {
            builder.AddElement(new ResistorElement(resistance), new[]{n1, n2});
            return builder;
        }

        public static CircuitBuilder AddInductor(this CircuitBuilder builder, double inductance, int n1, int n2)
        {
            builder.AddElement(new InductorElement(inductance),  new[]{n1, n2});
            return builder;
        }
        public static CircuitBuilder AddCapacitor(this CircuitBuilder builder, double capacitance, int n1, int n2)
        {
            builder.AddElement(new CapacitorElement(capacitance), new[] { n1, n2 });
            return builder;
        }
        public static CircuitBuilder AddCurrentSource(this CircuitBuilder builder, double current, int n1, int n2)
        {
            builder.AddElement(new CurrentSourceElement(current), new[] { n1, n2 });
            return builder;
        }
        public static CircuitBuilder AddVoltageSource(this CircuitBuilder builder, double voltage, int n1, int n2)
        {
            builder.AddElement(new VoltageSourceElement(voltage), new[] { n1, n2 });
            return builder;
        }

        public static CircuitBuilder AddDiode(this CircuitBuilder builder, DiodeModelParams param, int n1, int n2)
        {
            builder.AddElement(new DiodeElement(param), new[] { n1, n2 });
            return builder;
        }

        public static CircuitBuilder AddDiode(this CircuitBuilder builder, Action<DiodeModelParams> config, int n1, int n2)
        {
            var param = new DiodeModelParams();
            config(param);
            builder.AddElement(new DiodeElement(param), new[] { n1, n2 });
            return builder;
        }
    }
}