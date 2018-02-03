using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;
using Xunit;

namespace NextGenSpiceTest
{
    public class CircuitInstantiationTests
    {
        public CircuitInstantiationTests()
        {
            circuitDefinition = CircuitGenerator.GetLinearCircuit();
            var modelCreators =
                new Dictionary<Type, Func<ICircuitDefinitionElement, IModelInstantiationContext<LargeSignalCircuitModel>
                    , IAnalysisDeviceModel<LargeSignalCircuitModel>>>
                {
                    [typeof(ResistorElement)] =
                    (e, ctx) => new LargeSignalResistorModel((ResistorElement) e),
                    [typeof(VoltageSourceElement)] =
                    (e, ctx) => new LargeSignalVoltageSourceModel((VoltageSourceElement) e, null)
                };


            context = new ModelInstantiationContext<LargeSignalCircuitModel>(modelCreators,
                new Dictionary<Type, Func<object, IModelInstantiationContext<LargeSignalCircuitModel>, object>>(),
                circuitDefinition);
        }

        private readonly ModelInstantiationContext<LargeSignalCircuitModel> context;
        private readonly ElectricCircuitDefinition circuitDefinition;

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