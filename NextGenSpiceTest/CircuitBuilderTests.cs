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
            Assert.Equal(0, builder.NodeCount);

            builder.AddDiode(DiodeModelParams.Default, 5, 0);
            Assert.Equal(6, builder.NodeCount);


            builder.SetNodeVoltage(4, 0);
            Assert.Equal(6, builder.NodeCount);
        }

        [Fact]
        public void TestThrowOnInvalidNumberOfConnections()
        {
            Assert.Throws<ArgumentException>(() => builder.AddElement(new ResistorElement(3), new[] { 1 }));
            Assert.Throws<ArgumentException>(() => builder.AddElement(new ResistorElement(3), new[] { 1, 2, 3 }));
        }


        [Fact]
        public void TestThrowOnNegativeNodeOrVoltage()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.AddDiode(DiodeModelParams.Default, -2, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.SetNodeVoltage(1, -2));
        }
    }
}