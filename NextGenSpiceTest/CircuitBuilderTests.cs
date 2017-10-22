using System;
using NextGenSpice.Circuit;
using NextGenSpice.Elements;
using NextGenSpice.Extensions;
using NextGenSpice.Models;
using Xunit;

namespace NextGenSpiceTests
{
   
    public class CircuitBuilderTests
    {
        private CircuitBuilder builder;


        public CircuitBuilderTests()
        {
            builder = new CircuitBuilder();
        }

        [Fact]
        public void TestThrowsWhenNodeIsNotConnectedToGround()
        {
            builder.AddResistor(1, 1, 0);
            builder.AddResistor(2, 1, 2);
            builder.AddResistor(3, 0, 2);
            builder.AddCurrentSource(5, 1, 0);

            // add element 'far away'
            builder.AddResistor(3, 3, 4);

            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Fact]
        public void TestGeneratesNodesWhenRequestingHighId()
        {
            Assert.Equal(0, builder.Nodes.Count);

            builder.AddDiode(new DiodeModelParams(), 5, 0);
            Assert.Equal(6, builder.Nodes.Count);


            builder.SetNodeVoltage(4, 0);
            Assert.Equal(6, builder.Nodes.Count);
        }

        [Fact]
        public void TestThrowOnInvalidNumberOfConnections()
        {
            Assert.Throws<ArgumentException>(() => builder.AddElement(new Resistor(3), new[] { 1 }));
            Assert.Throws<ArgumentException>(() => builder.AddElement(new Resistor(3), new[] { 1, 2, 3 }));
        }


        [Fact]
        public void TestThrowOnNegativeNodeOrVoltage()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.AddDiode(new DiodeModelParams(), -2, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.SetNodeVoltage(1, -2));
        }
    }
}