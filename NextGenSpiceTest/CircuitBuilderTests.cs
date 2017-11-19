using System;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Extensions;
using Xunit;

namespace NextGenSpiceTest
{
   
    public class CircuitBuilderTests
    {
        private readonly CircuitBuilder builder;


        public CircuitBuilderTests()
        {
            builder = new CircuitBuilder();
        }

        [Fact]
        public void TestThrowsWhenNodeIsNotConnectedToGround()
        {
            builder.AddResistor(1, 0, 1);
            builder.AddResistor(1, 2, 2);
            builder.AddResistor(0, 2, 3);
            builder.AddCurrentSource(1, 0, 5);

            // add element 'far away'
            builder.AddResistor(3, 4, 3);

            Assert.Throws<InvalidOperationException>(() => builder.BuildCircuit());
        }

        [Fact]
        public void TestGeneratesNodesWhenRequestingHighId()
        {
            Assert.Equal(0, builder.NodeCount);

            builder.AddDiode(0, 5, DiodeModelParams.Default);
            Assert.Equal(6, builder.NodeCount);


            builder.SetNodeVoltage(4, 0);
            Assert.Equal(6, builder.NodeCount);
        }

        [Fact]
        public void TestThrowOnInvalidNumberOfConnections()
        {
            Assert.Throws<ArgumentException>(() => builder.AddElement(new[] { 1 }, new ResistorElement(3)));
            Assert.Throws<ArgumentException>(() => builder.AddElement(new[] { 1, 2, 3 }, new ResistorElement(3)));
        }


        [Fact]
        public void TestThrowOnNegativeNodeOrVoltage()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.AddDiode(0, -2, DiodeModelParams.Default));
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.SetNodeVoltage(1, -2));
        }

        [Fact]
        public void ThrowsWhenAddingSameDeviceTwice()
        {
            var device = new ResistorElement(5);
            builder.AddElement(new []{1, 2}, device);
            Assert.Throws<InvalidOperationException>(() => builder.AddElement(new[] {1, 2}, device));
        }
    }
}