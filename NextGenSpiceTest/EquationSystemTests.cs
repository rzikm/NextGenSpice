using System;
using NextGenSpice.Core.Equations;
using Numerics;
using Xunit;

namespace NextGenSpiceTest
{
    public class EquationSystemTests
    {
        public EquationSystemTests()
        {
            builder = new EquationSystemBuilder();
        }

        private readonly EquationSystemBuilder builder;

        [Fact]
        public void TestCheckSizeConstraintsForConstructor()
        {
            Assert.Throws<ArgumentException>(() => new EquationSystem(new Matrix<double>(4), new double[5]));
        }

        [Fact]
        public void TestReturnsVariableCount()
        {
            builder.AddVariable();
            builder.AddVariable();
            builder.AddVariable();

            Assert.Equal(3, builder.VariablesCount);

            var system = builder.Build();

            Assert.Equal(3, system.VariablesCount);
        }
    }
}