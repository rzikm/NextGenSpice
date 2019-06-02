using System;
using System.Linq;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Devices.Parameters;
using NextGenSpice.Core.Exceptions;
using NextGenSpice.Core.Extensions;
using Xunit;

namespace NextGenSpice.Test
{
    public class CircuitBuilderTests
    {
        public CircuitBuilderTests()
        {
            builder = new CircuitBuilder();
        }

        private readonly CircuitBuilder builder;

        [Fact]
        public void CanAddClonedDevice()
        {
            var device = new Resistor(5);
            builder.AddDevice(new[] {1, 2}, device);
            builder.AddDevice(new[] {1, 2}, device.Clone());
        }

        [Fact]
        public void TestGeneratesNodesWhenRequestingHighId()
        {
            Assert.Equal(1, builder.NodeCount);

            builder.AddDiode(0, 5, DiodeParams.Default);
            Assert.Equal(6, builder.NodeCount);


            builder.SetNodeVoltage(4, 0);
            Assert.Equal(6, builder.NodeCount);
        }

        [Fact]
        public void TestThrowOnInvalidNumberOfConnections()
        {
            Assert.Throws<ArgumentException>(() => builder.AddDevice(new[] {1}, new Resistor(3)));
            Assert.Throws<ArgumentException>(() => builder.AddDevice(new[] {1, 2, 3}, new Resistor(3)));
        }


        [Fact]
        public void TestThrowOnNegativeNodeOrVoltage()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.AddDiode(0, -2, DiodeParams.Default));
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.SetNodeVoltage(1, -2));
        }

        [Fact]
        public void TestThrowsWhenFloatingNodeInSubcircuit()
        {
            builder.AddResistor(1, 0, 1);
            builder.AddResistor(1, 2, 2);
            builder.AddResistor(0, 2, 3);
            builder.AddCurrentSource(1, 0, 5);

            // add device 'far away'
            builder.AddResistor(3, 4, 3);

//            builder.BuildSubcircuit(new []{1,4});
            Assert.Throws<NotConnectedSubcircuitException>(() => builder.BuildSubcircuit(new[] {1, 4}));
        }

        [Fact]
        public void TestThrowsWhenNodeIsNotConnectedToGround()
        {
            builder.AddResistor(1, 0, 1);
            builder.AddResistor(1, 2, 2);
            builder.AddResistor(0, 2, 3);
            builder.AddCurrentSource(1, 0, 5);

            // add device 'far away'
            builder.AddResistor(3, 4, 3);

            //            builder.BuildCircuit();
            Assert.Throws<NoDcPathToGroundException>(() => builder.BuildCircuit());
        }

        [Fact]
        public void ThrowsWhenAddingDeviceWithDuplicateName()
        {
            var elem1 = new Resistor(5, "R1");
            var elem2 = new Resistor(5, "R1");

            builder.AddDevice(new[] {1, 2}, elem1);
            Assert.Throws<InvalidOperationException>(() => builder.AddDevice(new[] {1, 2}, elem2));
        }

        [Fact]
        public void ThrowsWhenAddingSameDeviceTwice()
        {
            var device = new Resistor(5);
            builder.AddDevice(new[] {1, 2}, device);
            Assert.Throws<InvalidOperationException>(() => builder.AddDevice(new[] {1, 2}, device));
        }

        [Fact]
        public void ThrowsWhenCurrentSourceCutset()
        {
            // a cutset
            builder.AddCurrentSource(2, 3, 1, "I2");
            builder.AddCurrentSource(0, 1, 1, "I1");

            // some other devices
            builder.AddVoltageSource(1, 2, 1, "V4");
            builder.AddCurrentSource(1, 4, 1, "I3"); // cannot be in cutset
            builder.AddResistor(4, 2, 1, "R2");
            builder.AddResistor(0, 3, 1, "R");

            var devices = Assert.Throws<CurrentBranchCutsetException>(() => builder.BuildCircuit()).Devices;

            Assert.Equal(new[] {"I1", "I2"}, devices.Select(e => e.Tag).OrderBy(s => s));
        }

        [Fact]
        public void ThrowsWhenVoltageSourceCycle()
        {
            // a cycle
            builder.AddVoltageSource(0, 2, 1, "V2");
            builder.AddVoltageSource(0, 1, 1, "V1");
            builder.AddVoltageSource(1, 2, 1, "V3");

            // some other devices
            builder.AddVoltageSource(0, 3, 1, "V4");
            builder.AddResistor(2, 3, 1, "R");

            var devices = Assert.Throws<VoltageBranchCycleException>(() => builder.BuildCircuit()).Devices;

            Assert.Equal(new[] {"V1", "V2", "V3"}, devices.Select(e => e.Tag).OrderBy(s => s));
        }
    }
}