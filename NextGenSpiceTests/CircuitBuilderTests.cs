using System;
using NextGenSpice.Circuit;
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
            builder.AddElement(new RezistorElement(1), 1, 0);
            builder.AddElement(new RezistorElement(2), 1, 2);
            builder.AddElement(new RezistorElement(3), 0, 2);
            builder.AddElement(new CurrentSourceElement(5), 1, 0);

            // add element 'far away'
            builder.AddElement(new RezistorElement(3), 3, 4);

            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Test]
        public void TestGeneratesNodesWhenRequestingHighId()
        {
            Assert.AreEqual(0, builder.Nodes.Count);

            builder.GetNode(5);
            Assert.AreEqual(6, builder.Nodes.Count);


            builder.GetNode(4);
            Assert.AreEqual(6, builder.Nodes.Count);
        }

        [Test]
        public void TestThrowOnInvalidNumberOfConnections()
        {
            Assert.Throws<ArgumentException>(() => builder.AddElement(new CapacitorElement(3), 1));
            Assert.Throws<ArgumentException>(() => builder.AddElement(new CapacitorElement(3), 1,2,3));
        }


        [Test]
        public void TestThrowOnNegativeNodeId()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.AddElement(new CapacitorElement(3), -1,2));
        }
    }
}