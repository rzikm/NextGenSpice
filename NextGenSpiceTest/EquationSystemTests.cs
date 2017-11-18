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
        public void TestBindsEquivalent()
        {
            builder.AddVariable();
            builder.AddVariable();
            builder.AddVariable();
            builder.AddVariable();

            Random r = new Random(42);

            for (int i = 0; i < builder.VariablesCount; i++)
            {
                for (int j = 0; j < builder.VariablesCount; j++)
                {
                    var value = r.Next(100);
                    builder.AddMatrixEntry(i, j, value);
                    builder.AddRightHandSideEntry(i, i * value);
                }
            }

            var system = builder.Build();

            Assert.Throws<ArgumentOutOfRangeException>(() => system.BindEquivalent(-1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => system.BindEquivalent(5, 0));

            system.BindEquivalent(0, 1);
            var solution = system.Solve();
            Assert.Equal(solution[0], solution[1], new DoubleComparer(double.Epsilon));

            system.Clear();
            system.BindEquivalent(1,2);
            solution = system.Solve();
            Assert.Equal(solution[2], solution[1], new DoubleComparer(double.Epsilon));
        }

        [Fact]
        public void TestCheckSizeConstraintsForConstructor()
        {
            Assert.Throws<ArgumentException>(() => new EquationSystem(new Array2DWrapper(4), new double[5] ));
        }

    }
}