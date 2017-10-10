using System;
using NextGenSpice.Circuit;
using NextGenSpice.Equations;
using NextGenSpice.Helpers;
using NUnit.Framework;

namespace NextGenSpiceTests
{
    [TestFixture]
    public class EquationSystemTests
    {
        private IEquationSystemBuilder builder;
        private ICircuitModel circuit;

        [SetUp]
        public void SetUp()
        {
            builder = new EquationSystemBuilder();
        }

        [Test]
        public void TestReturnsVariableCount()
        {
            builder.AddVariable();
            builder.AddVariable();
            builder.AddVariable();

            Assert.AreEqual(3, builder.VariablesCount);

            var system = builder.Build();

            Assert.AreEqual(3, system.VariablesCount);
        }

        [Test]
        public void TestBindsEquivalent()
        {
            builder.AddVariable();
            builder.AddVariable();
            builder.AddVariable();

            Random r = new Random();

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
            Assert.AreEqual(solution[0], solution[1]);
        }

        [Test]
        public void TestCheckSizeConstraintsForConstructor()
        {
            Assert.Throws<ArgumentException>(() => new EquationSystem(new Array2DWrapper(4), new double[5] ));
        }

    }
}