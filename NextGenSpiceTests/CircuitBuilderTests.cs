using System;
using NextGenSpice.Circuit;
using NextGenSpice.Elements;
using NextGenSpice.Extensions;
using NextGenSpice.Models;
using NUnit.Framework;

namespace NextGenSpiceTests
{
    [TestFixture]
    public class CircuitBuilderTests
    {
        private CircuitBuilder builder;

        [SetUp]
        public void SetUp()
        {
            builder = new CircuitBuilder();
        }

        [Test]
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

        [Test]
        public void TestGeneratesNodesWhenRequestingHighId()
        {
            Assert.AreEqual(0, builder.Nodes.Count);

            builder.AddDiode(new DiodeModelParams(), 5, 0);
            Assert.AreEqual(6, builder.Nodes.Count);


            builder.SetNodeVoltage(4, 0);
            Assert.AreEqual(6, builder.Nodes.Count);
        }

        [Test]
        public void TestThrowOnInvalidNumberOfConnections()
        {
            Assert.Throws<ArgumentException>(() => builder.AddElement(new ResistorElement(3), new[] { 1 }));
            Assert.Throws<ArgumentException>(() => builder.AddElement(new ResistorElement(3), new[] { 1, 2, 3 }));
        }


        [Test]
        public void TestThrowOnNegativeNodeOrVoltage()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.AddDiode(new DiodeModelParams(), -2, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.SetNodeVoltage(1, -2));
        }
    }
}