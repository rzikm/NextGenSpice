using System;
using System.Linq;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal;
using Xunit;

namespace NextGenSpiceTest
{
    public class CircuitInstantiationTests
    {
        private readonly ModelInstantiationContext<LargeSignalCircuitModel> context;
        private readonly ElectricCircuitDefinition circuitDefinition;

        public CircuitInstantiationTests()
        {
            circuitDefinition = CircuitGenerator.GetLinearCircuit();
            context = new ModelInstantiationContext<LargeSignalCircuitModel>(new LargeSignalAnalysisModelFactory(), circuitDefinition);
        }

        [Fact]
        public void GetsCachedModel()
        {
            var element = circuitDefinition.Elements.First();

            var model = context.GetModel(element);

            Assert.Equal(model, context.GetModel(element));
        }

        [Fact]
        public void GetsModelByName()
        {
            var modelName = "R1";

            var element = circuitDefinition.Elements.Single(e => e.Name == modelName);
            var model = context.GetModel(modelName);

            Assert.Equal(context.GetModel(element), model);
        }

        [Fact]
        public void ThrowWhenNoSuchNameExists()
        {
            Assert.Throws<ArgumentNullException>(() => context.GetModel((string) null));
            Assert.Throws<ArgumentException>(() => context.GetModel("nonexistant model"));

            Assert.Throws<ArgumentNullException>(() => context.GetModel((ICircuitDefinitionElement) null));
        }
    }
}