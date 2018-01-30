using System;
using NextGenSpice.Core.Equations;
using NextGenSpice.Core.Helpers;
using Numerics;
using Xunit;

namespace NextGenSpiceTest
{
   
    public class EquationSystemTests
    {
        private readonly EquationSystemBuilder builder;

        public EquationSystemTests()
        {
            builder = new EquationSystemBuilder();
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

        [Fact]
        public void TestCheckSizeConstraintsForConstructor()
        {
            Assert.Throws<ArgumentException>(() => new EquationSystem(new Array2DWrapper<double>(4), new double[5] ));
        }

    }
}